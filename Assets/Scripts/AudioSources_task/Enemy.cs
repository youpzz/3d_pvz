using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform moveTarget;

    [Header("Настройки")]

    [Header("Основное")]
    [SerializeField] private float health = 10;
    [SerializeField] private float damage = 1;
    [SerializeField] private float attackCooldown = 0.15f;
    [SerializeField] private int killReward = 0;

    [Header("Атака")]

    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask playerMask;

    [Header("Передвижение")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDistance = 0.02f;

    [Header("Звук")]

    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] idleSounds;
    [SerializeField] private AudioClip[] eatSound;

    [SerializeField] private float minIdleTime = 5;
    [SerializeField] private float maxIdleTime = 10;



    private Animator animator;

    bool canMove = true;
    bool seePlayer = false;
    bool readyToAttack = true;


    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(RandomIdleSound());

    }

    void Update()
    {

        HandleMovement();
        HandleAttack();
    }


    void HandleMovement()
    {
        if (moveTarget && canMove && !seePlayer)
        {
            float distance = Vector3.Distance(transform.position, moveTarget.position);
            if (distance > stopDistance)
            {
                Vector3 dir = (moveTarget.position - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
            }
            else
            {
                PlayerInterface.Instance.EndGame();
            }
        }
    }

    void HandleAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, playerMask);
        foreach (Collider hit in hits)
        {
            if (hit.GetComponentInParent<PlayerController>()) seePlayer = true;
            else seePlayer = false;

            if (seePlayer && readyToAttack)
            {
                StartCoroutine(hitPlayer());
                hit.GetComponentInParent<PlayerController>().TakeDamage(damage);
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(eatSound[Random.Range(0, eatSound.Length)]);
            }
        }
        if (hits.Length <= 0) seePlayer = false;
        
        animator.SetBool("attack", seePlayer);
    }

    IEnumerator hitPlayer()
    {
        readyToAttack = false;
        float timer = attackCooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        readyToAttack = true;
    }

    IEnumerator RandomIdleSound()
    {

        float timer = Random.Range(minIdleTime, maxIdleTime);

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (health > 0)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Length)]);
            StartCoroutine(RandomIdleSound());
        }

    }








    void OnDestroy()
    {
        PlayerAudio.Instance.PlaySound("killpop");
        PlayerController.Instance.ChangeCoins(killReward);
        WavesSpawner.Instance.spawnedZombies.Remove(gameObject);
    }


    public void TakeDamage(float damage_)
    {
        health -= damage_;
        if (health <= 0) { animator.SetTrigger("die"); canMove = false; Destroy(gameObject, 5f); }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public void SetTarget(Transform target_) => moveTarget = target_;



}
