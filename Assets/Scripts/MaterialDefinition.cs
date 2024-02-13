using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Material Definition")]
public class MaterialDefinition : ScriptableObject {
    public string DisplayName;
    public Material Material;
}
