using UnityEngine;

public class Row : MonoBehaviour
{
    [SerializeField] private Transform enemySpawn;
    [SerializeField] private Transform playerJump;



    public Transform GetTransform() => playerJump;

    public Transform GetSpawnPoint() => enemySpawn;
}
