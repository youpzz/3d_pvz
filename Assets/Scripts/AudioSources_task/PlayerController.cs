using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Stats")]

    [SerializeField] private float Health = 3f;
    [SerializeField] private float Damage = 1;
    [SerializeField] private int Coins = 0;


    [Header("Attack Settings")]

    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject peaPrefab;
    [SerializeField] private float shootCooldown = 1.5f;
    [SerializeField] private float shootLatency = 0.5f;


    [Header("Move Settings")]

    [SerializeField] private Row[] rows;
    [SerializeField] private int currentRow = 3;
    [Space(5)]

    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private float moveLatency = 0.25f;

    private Transform moveTarget;


    bool isMoving = false;
    bool canShoot = true;
    bool canMove = true;

    private float cooldownFill = 0;



    void Start()
    {
        Instance = this;
        cooldownFill = 1;
    }

    void Update()
    {
        if (PlayerInterface.Instance.isPanelOpened) return; 
        HandleInput();
        Move();
    }


    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) ChangeRow(-1);
        if (Input.GetKeyDown(KeyCode.D)) ChangeRow(1);
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) Shoot();
    }


    void Shoot()
    {
        if (isMoving || !canShoot || !canMove ) return;

        StartCoroutine(ShootLatency());

    }

    IEnumerator ShootLatency()
    {
        PlayerAnimator.Instance.SetTrigger("attack");
        canShoot = false;
        canMove = false;
        float timer = shootLatency;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;

        }
        Pea pea = Instantiate(peaPrefab, shootPoint.transform.position, Quaternion.identity).GetComponent<Pea>();
        pea.SetDamage(Damage);
        PlayerAudio.Instance.PlaySound("shoot");
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        canMove = true;
        canShoot = false;
        float timer = shootCooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            cooldownFill = 1 - (timer / shootCooldown);
            yield return null;
        }

        canShoot = true;
    }


    void ChangeRow(int value)
    {
        Debug.Log($"Двигаюсь {value}");
        if (isMoving) return;
        currentRow += value;
        if (currentRow <= 0) currentRow = 0;
        if (currentRow >= rows.Length) currentRow = rows.Length - 1;
        moveTarget = rows[currentRow].GetTransform();
        StartCoroutine(MoveLat());
        PlayerAnimator.Instance.SetTrigger("jump");
        PlayerAudio.Instance.PlaySound("jump");


    }


    IEnumerator MoveLat()
    {
        float timer = moveLatency;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isMoving = true;
    }

    void Move()
    {
        if (!isMoving || moveTarget == null) return;

        float distance = Vector2.Distance(transform.position, moveTarget.position);
        if (distance > stopDistance)
        {
            float t = Mathf.Clamp01(speed * Time.deltaTime * (distance + 1f)); // вот тут немного навайбкодил, конкретно эту строчку, ибо почему бы и нет :)
            transform.position = Vector3.Lerp(transform.position, moveTarget.position, t);
        }
        else
        {
            transform.position = moveTarget.position;
            isMoving = false;
            moveTarget = null;
        }
    }

    public void TakeDamage(float damage_)
    {
        Health -= damage_;
        if (Health <= 0)
        {
            PlayerInterface.Instance.EndGame(); 
        }
    }

    public float GetCooldownTime() => cooldownFill;

    public int GetCoins() => Coins;
    public int ChangeCoins(int amount) => Coins += amount;

    public void AddDamage(float amount) => Damage += amount;
    public void ReduceCooldown(float amount) => shootCooldown -= amount;

}
