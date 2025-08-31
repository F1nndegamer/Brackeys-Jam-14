using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private ShopItemSO shopItemSO;
    [SerializeField] private Image image;
    [SerializeField] private Outline outline;

    private bool clicked = false;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clicked = !clicked;
        if (clicked)
        {
            UIManager.Instance.GetCanvas<CanvasShop>().SetSelectedShopItem(this);
            PanelItemInformation.Instance.ShowSelected(shopItemSO);
        }
        else
        {
            UIManager.Instance.GetCanvas<CanvasShop>().SetSelectedShopItem(null);
        }
        outline.enabled = clicked;

        transform.DOKill();
        transform.DOPunchScale(Vector3.one * 0.15f, 0.25f, 10, 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PanelItemInformation.Instance.ShowHover(shopItemSO);

        transform.DOKill();
        transform.DOScale(originalScale * 1.1f, 0.15f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!clicked)
        {
            PanelItemInformation.Instance.Hide();
        }

        transform.DOKill();
        transform.DOScale(originalScale, 0.15f).SetEase(Ease.OutBack);
    }
    public void Deselect()
    {
        clicked = false;
        outline.enabled = clicked;
        transform.DOKill();
    }
    public ShopItemSO GetShopItemSO()
    {
        return shopItemSO;
    }

    private void OnValidate()
    {
        if (shopItemSO != null)
        {
            image.sprite = shopItemSO.sprite;
        }
    }
    public void UpdateSpriteAfterPurchase()
    {
        if (shopItemSO.itemType == ItemType.CardboardBox)
        {
            return;
        }
        if (shopItemSO.purchasedSprite != null)
        {
            image.sprite = shopItemSO.purchasedSprite;

            image.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 1);
        }
    }
}
