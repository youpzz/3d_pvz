using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeElement : MonoBehaviour
{
    private Button upgradeButton;

    [SerializeField] private int price = 0;
    [SerializeField] private float damageBonus = 0;
    [SerializeField] private float cooldownBonus = 0;

    [Header("UI")]

    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text cooldownText;

    void Awake()
    {
        upgradeButton = GetComponent<Button>();
        upgradeButton.onClick.AddListener(Upgrade);
    }


    void Upgrade()
    {
        if (PlayerController.Instance.GetCoins() >= price)
        {
            
            PlayerController.Instance.ChangeCoins(-price);
            if (cooldownBonus != 0) PlayerController.Instance.ReduceCooldown(cooldownBonus);
            if (damageBonus != 0) PlayerController.Instance.AddDamage(damageBonus);
            price = Mathf.RoundToInt(price * 1.5f);
        }
    }

    void FixedUpdate()
    {
        upgradeButton.interactable = PlayerController.Instance.GetCoins() >= price;
        priceText.text = price.ToString();
        damageText.text = $"Damage Bonus: {damageBonus}";
        cooldownText.text = $"Reload Speed Bonus: {cooldownBonus}";
    }
}
