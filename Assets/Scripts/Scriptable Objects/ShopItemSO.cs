using UnityEngine;

[System.Serializable]
public class CollectibleRequirement
{
    public CollectibleSO collectible;
    public int amount;
}
public enum ItemType
{
    StatBoost = 0,
    SmokeBomb = 1,
}
[CreateAssetMenu(fileName = "ShopItemSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public string description;
    public Sprite sprite;
    public CollectibleRequirement[] cost;
}
