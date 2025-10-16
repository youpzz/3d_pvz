using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightOnHit : MonoBehaviour
{
    [SerializeField] private float cooldown = 0.1f;

    private SkinnedMeshRenderer[] meshRenderers;

    private List<Material> defaultMaterials = new List<Material>();
    private Material hightlightMaterial;

    private bool isHighlighting = false;


    void Start()
    {
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        GetMaterials();

    }


    void GetMaterials()
    {
        for (int i = 0; i < meshRenderers.Length; i++) defaultMaterials.Add(meshRenderers[i].material);
        hightlightMaterial = GameManager.Instance.GetHighlightedMaterial();
    }
    
    void SetMaterials(bool isHighlighted)
    {
        for (int i = 0; i < meshRenderers.Length; i++) meshRenderers[i].material = isHighlighted ? hightlightMaterial : defaultMaterials[i];
    }

    public void Highlight()
    {
        if (!isHighlighting) StartCoroutine(highlightIt());
    }
    
    IEnumerator highlightIt()
    {
        Debug.Log("Начало подсветки");
        isHighlighting = true;
        SetMaterials(isHighlighting);
        float timer = cooldown;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        isHighlighting = false;
        SetMaterials(isHighlighting);
        Debug.Log("Конец подсветки");


    }
}
