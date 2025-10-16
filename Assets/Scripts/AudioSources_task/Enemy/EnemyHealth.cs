using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health = 10;
    [SerializeField] private int killReward = 0;

    private Enemy enemy;
    private HighlightOnHit highlightOnHit;


    void Awake() { enemy = GetComponent<Enemy>(); highlightOnHit = GetComponent<HighlightOnHit>(); } 


    public void TakeDamage(float damage_)
    {
        health -= damage_;
        if (highlightOnHit) highlightOnHit.Highlight();
        if (health <= 0) { enemy.animator.SetTrigger("die"); enemy.canMove = false; Destroy(gameObject, 5f); }
    }

    public float GetHealth() => health;

    void OnDestroy()
    {
        PlayerAudio.Instance.PlaySound("killpop");
        PlayerController.Instance.ChangeCoins(killReward);
        WavesSpawner.Instance.spawnedZombies.Remove(gameObject);
    }
}
