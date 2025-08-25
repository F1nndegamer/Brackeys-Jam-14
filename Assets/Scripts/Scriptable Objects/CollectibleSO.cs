using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleSO", menuName = "Scriptable Objects/CollectibleSO")]
public class CollectibleSO : ScriptableObject
{
    public string collectibleName;
    public Sprite sprite;
}
