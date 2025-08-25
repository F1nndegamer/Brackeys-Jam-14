using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;

public class PanelItemInformation : Singleton<PanelItemInformation>
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private Image itemImage;
    [SerializeField] private CollectibleCost collectibleCostPrefab;
    [SerializeField] private Transform collectibleCostParent;
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float moveOffset = 100f; // how far from the right it starts

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ShopItemSO currentShopItemSO;
    private Vector2 originalAnchoredPos; // cache starting pos
    private List<CollectibleCost> collectibleCostList = new();

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalAnchoredPos = rectTransform.anchoredPosition;
    }
    public void ShowSelected(ShopItemSO shopItemSO)
    {
        Show(shopItemSO);
    }
    public void ShowHover(ShopItemSO shopItemSO)
    {
        if (currentShopItemSO == shopItemSO)
        {
            Debug.Log("Same Item");
            return;
        }
        if (UIManager.Instance.GetCanvas<CanvasShop>().IsAnyShopItemSelected())
        {
            Debug.Log("One Item Selected");
            return;
        }

        Show(shopItemSO);
    }
    private void Show(ShopItemSO shopItemSO)
    {
        currentShopItemSO = shopItemSO;
        gameObject.SetActive(true);
        SetShopItemSO(shopItemSO);
        ClearCollectibleCost();
        foreach (CollectibleRequirement collectibleRequirement in shopItemSO.cost)
        {
            CollectibleCost collectibleCost = Instantiate(collectibleCostPrefab, collectibleCostParent);
            collectibleCostList.Add(collectibleCost);
            collectibleCost.Initialize(collectibleRequirement.collectible.sprite, collectibleRequirement.amount);
        }
        // Kill old tweens (avoid stacking)
        rectTransform.DOKill();
        canvasGroup.DOKill();

        // Reset state
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = originalAnchoredPos + new Vector2(moveOffset, 0);

        // Animate fade + move
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeDuration));
        seq.Join(rectTransform.DOAnchorPos(originalAnchoredPos, moveDuration)
            .SetEase(Ease.OutCubic));
    }
    private void SetShopItemSO(ShopItemSO shopItemSO)
    {
        itemNameText.text = shopItemSO.itemName;
        itemDescriptionText.text = shopItemSO.description;
        itemImage.sprite = shopItemSO.sprite;
    }
    private void ClearCollectibleCost()
    {
        foreach (var collectibleCost in collectibleCostList)
        {
            Destroy(collectibleCost.gameObject);
        }
        collectibleCostList.Clear();
    }
    public void Hide()
    {
        if (UIManager.Instance.GetCanvas<CanvasShop>().IsAnyShopItemSelected()) return;

        currentShopItemSO = null;
        gameObject.SetActive(false);
    }
}
