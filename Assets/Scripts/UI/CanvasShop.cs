using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CanvasShop : UICanvas
{
    [Header("References")]
    [SerializeField] private RectTransform shadyShopImageTransform;
    [SerializeField] private RectTransform shadyShopTextTransform;
    [SerializeField] private RectTransform shopItemContainerTransform;
    [SerializeField] private Button buyButton;
    [SerializeField] private RectTransform continueButtonRect;
    [SerializeField] private ShopItem[] shopItemArray;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.8f;
    [SerializeField] private float rotateDuration = 0.4f;
    [SerializeField] private float rotateAmount = 1f;

    [Header("Collection")]
    [SerializeField] private TextMeshProUGUI[] collectibleTextArray;
    [SerializeField] private CollectibleSOArray collectibleSOArray;

    public static CanvasShop Instance;
    private Vector3 imageStartPos;
    private ShopItem selectedShopItem;
    private void Awake()
    {
        Instance = this;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            if (Player.Instance.CanAfford(selectedShopItem.GetShopItemSO()))
            {
                Player.Instance.Purchase(selectedShopItem.GetShopItemSO());
                selectedShopItem.UpdateSpriteAfterPurchase();
            }
            else
            {
                buyButton.transform.DOShakePosition(0.4f);
            }
        });
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        shadyShopImageTransform.DOKill();
        shadyShopTextTransform.DOKill();
        dialogueText.DOKill();
        foreach (var item in collectibleTextArray)
        {
            item.DOKill();
        }
        shopItemContainerTransform.DOKill();
        buyButton.transform.DOKill();
        continueButtonRect.DOKill();

        shadyShopImageTransform.localRotation = Quaternion.identity;
        shadyShopTextTransform.localRotation = Quaternion.identity;

        Vector3 imageTargetPos = shadyShopImageTransform.anchoredPosition;
        Vector3 shopItemTargetPos = shopItemContainerTransform.anchoredPosition;
        Vector3 buyButtonTargetPos = buyButton.GetComponent<RectTransform>().anchoredPosition;
        Vector3 continueButtonTargetPos = continueButtonRect.anchoredPosition;

        // start them off-screen
        shadyShopImageTransform.anchoredPosition = imageTargetPos + new Vector3(-600f, 0, 0);
        shopItemContainerTransform.anchoredPosition = shopItemTargetPos + new Vector3(0, 600f, 0);
        buyButton.GetComponent<RectTransform>().anchoredPosition = buyButtonTargetPos + new Vector3(0, -300f, 0);
        continueButtonRect.anchoredPosition = continueButtonTargetPos + new Vector3(0, -300f, 0);

        shadyShopImageTransform.DOAnchorPos(imageTargetPos, slideDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                shadyShopImageTransform.DORotate(new Vector3(0, 0, rotateAmount), rotateDuration)
                    .SetEase(Ease.OutSine);

                shadyShopTextTransform.DORotate(new Vector3(0, 0, -rotateAmount), rotateDuration)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() =>
                    {
                        StartCoroutine(TypeDialogue("Need some gear?"));
                    });
            });

        shopItemContainerTransform.DOAnchorPos(shopItemTargetPos, 0.7f)
            .SetEase(Ease.OutBack);

        buyButton.GetComponent<RectTransform>().DOAnchorPos(buyButtonTargetPos, 0.6f)
            .SetEase(Ease.OutBack);
        continueButtonRect.DOAnchorPos(continueButtonTargetPos, 0.8f)
            .SetEase(Ease.OutBack);

        UpdateCollecion();
    }


    private IEnumerator TypeDialogue(string fullText)
    {
        dialogueText.text = "";
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.04f);
        }
    }
    public void TextDialogue(string newText)
    {
        StartCoroutine(TypeDialogue(newText));
    }
    public void SetSelectedShopItem(ShopItem shopItem)
    {
        selectedShopItem = shopItem;
        foreach (ShopItem item in shopItemArray)
        {
            if (item == selectedShopItem) continue;
            item.Deselect();
        }
    }
    public bool IsAnyShopItemSelected()
    {
        return selectedShopItem != null;
    }
    public void UpdateCollecion()
    {
        int i = 0;
        foreach (var item in collectibleSOArray.collectibleSOArray)
        { 
            int count = Player.Instance.GetCollectibleCount(item);
            collectibleTextArray[i].text = count.ToString();
            i++;
            if (i == 4) return;
        }
    }
}
