using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Атака")]

    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float damage = 1;
    [SerializeField] private float attackCooldown = 0.15f;

    bool readyToAttack = true;
    bool seePlayer = false;

    private EnemyHealth enemyHealth;
    private Enemy enemy;


    void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        HandleAttack();
    }

    void HandleAttack()
    {
        if (enemyHealth.GetHealth() <= 0) return;
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, playerMask);
        foreach (Collider hit in hits)
        {
            if (hit.GetComponentInParent<PlayerController>()) seePlayer = true;
            else seePlayer = false;

            if (seePlayer && readyToAttack)
            {
                StartCoroutine(hitPlayer());
                hit.GetComponentInParent<PlayerController>().TakeDamage(damage);

                
            }
        }
        if (hits.Length <= 0) seePlayer = false;

        enemy.animator.SetBool("attack", seePlayer);
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

    public bool PlayerInRange() => seePlayer;


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
