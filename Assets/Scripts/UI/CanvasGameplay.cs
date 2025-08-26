using TMPro;
using UnityEngine;
using DG.Tweening;

public class CanvasGameplay : UICanvas
{
    [SerializeField] private TextMeshProUGUI[] collectibleTextArray;
    [SerializeField] private CollectibleSOArray collectibleSOArray;
    [SerializeField] private GameObject interactPrompt;
    
    [SerializeField] private GameObject[] consumableCounterArray; //Must be in order with enum ItemType (Smoke Bomb -> ...)
    [SerializeField] private TextMeshProUGUI[] consumableCountTextArray; //Must be in order with enum ItemType (Smoke Bomb -> ...)
    private void Awake()
    {
        Player.Instance.OnCollectibleAmountChanged += Player_OnCollectibleAmountChanged;
        Player.Instance.OnConsumableUsed += Player_OnConsumableUsed;
    }

    private void Player_OnConsumableUsed(ItemType itemType, int count)
    {
        TextMeshProUGUI textUI = consumableCountTextArray[(int)itemType - 1];
        GameObject counter = consumableCounterArray[(int)itemType - 1];
        //Minus one because ItemType has a non-consumable type at index 0

        textUI.text = count.ToString();

        RectTransform rect = counter.GetComponent<RectTransform>();
        rect.DOKill();
        rect.localScale = Vector3.one;

        rect.DOPunchScale(Vector3.one * 0.2f, 0.25f, 4, 0.8f);
    }

    private void Player_OnCollectibleAmountChanged(CollectibleSO collectibleSO, int count)
    {
        int index = collectibleSOArray.GetIndex(collectibleSO);
        TextMeshProUGUI textUI = collectibleTextArray[index];

        textUI.text = count.ToString();
        textUI.transform.DOKill();
        textUI.transform.localScale = Vector3.one;

        textUI.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 0.8f);
    }
    public void UnlockConsumable(ItemType itemType)
    {
        GameObject counter = consumableCounterArray[(int)itemType - 1];
        RectTransform rect = counter.GetComponent<RectTransform>();

        Vector2 originalPos = rect.anchoredPosition;
        Vector2 hiddenPos = originalPos + new Vector2(400f, 0f);

        rect.anchoredPosition = hiddenPos;
        counter.SetActive(true);

        rect.DOAnchorPos(originalPos, 0.5f)
            .SetEase(Ease.OutBack);
    }
    public void ShowInteractPrompt()
    {
        interactPrompt.SetActive(true);
    }
    public void HideInteractPrompt()
    {
        interactPrompt.SetActive(false);
    }
}
