using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartGame;

    private float sfxVolume = 0.75f;
    private float musicVolume = 0.5f;
    void Start()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        soundSlider.onValueChanged.AddListener(SetSfxVolume);
        continueButton.onClick.AddListener(ContinueGame);
        restartGame.onClick.AddListener(RestartGame);
        LoadSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }


    public void SetSfxVolume(float amount)
    {
        if (amount <= 0) { audioMixer.SetFloat("sfx", -80f); }
        float volumeDB = Mathf.Log10(amount * 0.2f) * 20;
        audioMixer.SetFloat("sfx", volumeDB);
        sfxVolume = amount;
        PlayerPrefs.SetFloat("sfx", sfxVolume);
    }

    public void SetMusicVolume(float amount)
    {
        if (amount <= 0) { audioMixer.SetFloat("music", -80f); }
        float volumeDB = Mathf.Log10(amount * 0.2f) * 20;
        audioMixer.SetFloat("music", volumeDB);
        musicVolume = amount;
        PlayerPrefs.SetFloat("music", musicVolume);
    }


    void ContinueGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pausePanel.SetActive(false);

    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(0);
    } 
    

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("sfx"))
        {
            sfxVolume = PlayerPrefs.GetFloat("sfx");
            Debug.Log($"Значение звука: {sfxVolume} ");
            soundSlider.value = sfxVolume;
            float sfxDb_ = Mathf.Log10(sfxVolume * 0.2f) * 20;
            audioMixer.SetFloat("sfx", sfxDb_);
        }
        if (PlayerPrefs.HasKey("music"))
        {
            musicVolume = PlayerPrefs.GetFloat("music");
            Debug.Log($"Значение музыки: {musicVolume} ");
            musicSlider.value = musicVolume;
            float musicDb_ = Mathf.Log10(musicVolume * 0.2f) * 20;
            audioMixer.SetFloat("music", musicDb_);
        }
    }
    

    
}
