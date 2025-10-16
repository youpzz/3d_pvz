using UnityEngine;

public class Pea : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifeTime = 5f;

    [SerializeField] private GameObject[] hitParticle;

    void Start() => Destroy(gameObject, lifeTime);

    void Update() => transform.Translate(Vector3.forward * speed * Time.deltaTime);

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<EnemyHealth>())
        {
            
            EnemyHealth enemy = collision.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemy.GetHealth() <= 0) return;

            enemy.TakeDamage(damage);
            if (hitParticle.Length > 0) Instantiate(hitParticle[Random.Range(0, hitParticle.Length)], transform.position, Quaternion.identity);
            PlayerAudio.Instance.PlaySound("pea_hit");
            PlayerAudio.Instance.MakeDistortion(); // для тз дисторшен включаем
            Destroy(gameObject);

        }
    }

    public void SetDamage(float Damage_) => damage = Damage_;
    public void SetLifeTime(float time) => lifeTime = time;
    
}
