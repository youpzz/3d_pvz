using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesSpawner : MonoBehaviour
{
    public static WavesSpawner Instance;
    [SerializeField] private int wave = 1;

    [SerializeField] private GameObject zombie;

    public List<GameObject> spawnedZombies = new List<GameObject>();



    [SerializeField] private Row[] rows;


    [Header("settings")]

    [SerializeField] private float waveCooldown = 5f;
    [SerializeField] private float spawnCooldown = 2f;


    private int zombiesToSpawn = 0;


    bool isWaveEnded = false;


    void Start()
    {
        Instance = this;
    }


    void Update()
    {
        if (spawnedZombies.Count <= 0 && !isWaveEnded)
        {
            StartCoroutine(StartWave());
        }
    }



    IEnumerator StartWave()
    {
        wave++;
        isWaveEnded = true;
        zombiesToSpawn = wave;

        float timer = waveCooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(SpawnZombies());
        PlayerAudio.Instance.PlaySound("wave");

    }

    IEnumerator SpawnZombies()
    {
        float timer = spawnCooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        int randomRow = Random.Range(0, rows.Length);
        GameObject zombie_ = Instantiate(zombie, rows[randomRow].GetSpawnPoint().position, Quaternion.identity);
        zombie_.GetComponent<Enemy>().SetTarget(rows[randomRow].GetTransform());
        spawnedZombies.Add(zombie_);
        isWaveEnded = false;

        zombiesToSpawn--;
        if (zombiesToSpawn > 0) StartCoroutine(SpawnZombies());


    }

    public int GetWave() => wave;


}
