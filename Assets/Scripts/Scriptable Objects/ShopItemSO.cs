using UnityEngine;

[System.Serializable]
public class CollectibleRequirement
{
    public CollectibleSO collectible;
    public int amount;
}
public enum ItemType
{
    Shoes = 0,
    Bag = 1,
    Bicep = 2,
    SmokeBomb = 3,
    CardboardBox = 4,
}
[CreateAssetMenu(fileName = "ShopItemSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    public ItemType itemType;
    public float stat;
    public string itemName;
    public string description;
    public Sprite sprite;
    public CollectibleRequirement[] cost;
}
