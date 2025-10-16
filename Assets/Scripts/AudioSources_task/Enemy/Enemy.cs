using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform moveTarget;

    public Animator animator;


    [Header("Передвижение")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDistance = 0.02f;


    

    public bool canMove = true;

    private EnemyAttack enemyAttack;
    
    


    void Start()
    {
        animator = GetComponent<Animator>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    void Update()
    {
        HandleMovement();
    }


    void HandleMovement()
    {
        if (moveTarget && canMove && !enemyAttack.PlayerInRange())
        {
            float distance = Vector3.Distance(transform.position, moveTarget.position);
            if (distance > stopDistance)
            {
                Vector3 dir = (moveTarget.position - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
            }
            else
            {
                canMove = false;
                PlayerInterface.Instance.EndGame();
            }
        }
    }
    public void SetTarget(Transform target_) => moveTarget = target_;



}
