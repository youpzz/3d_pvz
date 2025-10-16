using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Material highlightMaterial;



    void Awake()
    {
        Instance = this;
    }



    public Material GetHighlightedMaterial() => highlightMaterial;
}
