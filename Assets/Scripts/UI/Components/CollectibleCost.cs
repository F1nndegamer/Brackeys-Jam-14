using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleCost : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;

    public void Initialize(Sprite icon, int cost)
    {
        iconImage.sprite = icon;
        costText.text = cost.ToString();
    }
}
