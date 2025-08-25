using UnityEngine;

[System.Serializable]
public class CollectibleRequirement
{
    public CollectibleSO collectible;
    public int amount;
}

[CreateAssetMenu(fileName = "ShopItemSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite sprite;
    public CollectibleRequirement[] cost;
}
