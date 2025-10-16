using System.Collections;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public static PlayerAudio Instance;

    [SerializeField] private AudioSource playerSource;
    [SerializeField] private AudioSource musicSource;

    [Space(5)]

    [SerializeField] private AudioClip[] peaHit;
    [SerializeField] private AudioClip gameStart;
    [SerializeField] private AudioClip gameLose;
    [SerializeField] private AudioClip gameWin;
    [SerializeField] private AudioClip waveSound;
    [SerializeField] private AudioClip shoot;
    [SerializeField] private AudioClip killPop;
    [SerializeField] private AudioClip jump;


    [Space(5)]

    [SerializeField] private AudioDistortionFilter audioDistortionFilter;
    [SerializeField] private float cooldown = 0.5f;

    bool isDistortion = false;



    void Start()
    {
        Instance = this;
    }
    
    public void MakeDistortion()
    {
        if (isDistortion) return;

        StartCoroutine(DistortionBtw());
    }


    public void PlaySound(string name)
    {
        playerSource.pitch = Random.Range(0.95f, 1.05f); // для того, чтобы один и тот же звук отличался хоть немного
        switch (name.ToLower())
        {
            case "pea_hit": playerSource.PlayOneShot(peaHit[Random.Range(0, peaHit.Length)]); break;
            case "start": playerSource.PlayOneShot(gameStart); break;
            case "lose": playerSource.PlayOneShot(gameLose); break;
            case "win": playerSource.PlayOneShot(gameWin); break;
            case "wave": playerSource.PlayOneShot(waveSound); break;
            case "shoot": playerSource.PlayOneShot(shoot); break;
            case "killpop": playerSource.PlayOneShot(killPop); break;
            case "jump": playerSource.PlayOneShot(jump); break;
        }
    }

    IEnumerator DistortionBtw()
    {
        isDistortion = true;
        audioDistortionFilter.enabled = true;

        float timer = cooldown;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        audioDistortionFilter.enabled = false;
        isDistortion = false;

    }
}
