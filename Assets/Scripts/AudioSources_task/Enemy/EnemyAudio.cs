using System.Collections;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Звук")]


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] idleSounds;
    [SerializeField] private AudioClip[] eatSound;

    [SerializeField] private float minIdleTime = 5;
    [SerializeField] private float maxIdleTime = 10;

    private EnemyHealth enemyHealth;


    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        StartCoroutine(RandomIdleSound());
    }

    IEnumerator RandomIdleSound()
    {
        float timer = Random.Range(minIdleTime, maxIdleTime);

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (enemyHealth.GetHealth() > 0)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Length)]);
            StartCoroutine(RandomIdleSound());
        }

    }

    public void PlayEatSound()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(eatSound[Random.Range(0, eatSound.Length)]);
    }
}
