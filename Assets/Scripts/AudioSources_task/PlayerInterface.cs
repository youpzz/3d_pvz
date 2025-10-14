using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInterface : MonoBehaviour
{
    public static PlayerInterface Instance;
    [SerializeField] private Image cooldownFill;


    [SerializeField] private TMP_Text coinsText;


    [SerializeField] private GameObject upgradesPanel;

    [SerializeField] private GameObject endgamePanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button restartButton;


    public bool isPanelOpened = false;

    void Start()
    {
        Instance = this;

        restartButton.onClick.AddListener(RestartGame);
    }





    

    void Update()
    {
        UpdateInterface();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            upgradesPanel.SetActive(!upgradesPanel.activeSelf);
            Cursor.visible = upgradesPanel.activeSelf;
            Cursor.lockState = upgradesPanel.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
            isPanelOpened = upgradesPanel.activeSelf;

        }
        


    }

    void UpdateInterface()
    {
        if (cooldownFill) cooldownFill.fillAmount = PlayerController.Instance.GetCooldownTime();
        if (coinsText) coinsText.text = PlayerController.Instance.GetCoins().ToString();
    }

    public void TriggerPanel(GameObject _panel) {_panel.SetActive(!_panel.activeSelf); isPanelOpened = upgradesPanel.activeSelf; }

    void RestartGame() { Time.timeScale = 1f; SceneManager.LoadScene(0); }

    public void EndGame()
    {
        resultText.text = $"You survived {WavesSpawner.Instance.GetWave()} waves!";
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (WavesSpawner.Instance.GetWave() <= 8) PlayerAudio.Instance.PlaySound("lose");
        else PlayerAudio.Instance.PlaySound("win");
        endgamePanel.SetActive(true);
    }


}
