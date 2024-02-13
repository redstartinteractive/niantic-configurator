using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Product Definition")]
public class ProductDefinition : ScriptableObject
{
    public string DisplayName;
    public MaterialDefinition[] Materials;
    public MeshRenderer[] Models;
}
