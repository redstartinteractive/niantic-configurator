using UnityEngine;

public class Product : MonoBehaviour
{
    [SerializeField] private ProductDefinition productDefinition;

    public ProductDefinition Definition => productDefinition;
    public Rigidbody Rigidbody { get; private set; }

    private MeshRenderer meshRenderer;
    private int currentMaterialIndex;
    public int CurrentMaterialIndex => currentMaterialIndex;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Start()
    {
        SetModel(0);
        SetMaterial(0);
    }

    public void SetMaterial(int materialIndex)
    {
        MaterialDefinition matDefinition = Definition.Materials[materialIndex];
        if(!matDefinition)
        {
            Debug.LogWarning($"Material Definition not found for object {name} at index {materialIndex}");
            return;
        }

        meshRenderer.material = matDefinition.Material;
        currentMaterialIndex = materialIndex;
    }

    public void SetModel(int modelIndex)
    {
        MeshRenderer modelPrefab = Definition.Models[modelIndex];
        if(!modelPrefab)
        {
            Debug.LogWarning($"Model not found for object {name} at index {modelIndex}");
            return;
        }

        // Remove any existing child objects
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // Set new reference to meshRenderer to be able to set material
        meshRenderer = Instantiate(modelPrefab, transform);
        SetMaterial(currentMaterialIndex);
    }
}
