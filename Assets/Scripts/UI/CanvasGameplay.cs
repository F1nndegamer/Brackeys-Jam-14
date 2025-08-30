using TMPro;
using UnityEngine;
using DG.Tweening;

public class CanvasGameplay : UICanvas
{
    [SerializeField] private TextMeshProUGUI[] collectibleTextArray;
    [SerializeField] private CollectibleSOArray collectibleSOArray;
    [SerializeField] private GameObject interactPrompt;

    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private RectTransform raccoonImageRect;
    [SerializeField] private TextMeshProUGUI dialogueText;
    private string[] introDialogue = new string[]
    {
        "One two one two. Do you copy? Name's Rascal, your partner in crime tonight.",
        "Listen carefully, kid. This street's full of houses, but we�ll only hit one at a time.",
        "Your job? Slip inside, nab whatever's marked as tonight�s prize, and get out clean.",
        "However, if you're feeling it, you can stay as long as you want and grab as much stuffs as you'd like",
    };
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private RectTransform objectRect;

    [SerializeField] private GameObject smokeBombCounter;
    [SerializeField] private TextMeshProUGUI smokeBombCountText;
    [SerializeField] private GameObject cardboardBoxCounter;
    [SerializeField] private TextMeshProUGUI cardboardBoxCountText;
    [SerializeField] private GameObject HeartParent;
    private int currentDialogueIndex = 0;
    private Tween typingTween;
    private bool isTyping = false;
    private const string IntroPlayedKey = "IntroDialoguePlayed";
    

    private void Awake()
    {
        Player.Instance.OnCollectibleAmountChanged += Player_OnCollectibleAmountChanged;
        Player.Instance.OnConsumableUsed += Player_OnConsumableUsed;
        bool hasPlayedIntro = PlayerPrefs.GetInt(IntroPlayedKey, 0) == 1;
        if (hasPlayedIntro)
        {
            dialogueCanvasGroup.alpha = 0;
            objectRect.localScale = Vector3.zero;
            objectRect.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            objectiveText.text = $"This house's objective:\nA {ObjectiveManager.Instance.GetCurrentObjective().collectibleName}";
            return;
        }
        dialogueCanvasGroup.alpha = 0;
        dialogueCanvasGroup.DOFade(1, 0.4f).SetEase(Ease.OutQuad);

        Vector2 originalPos = raccoonImageRect.anchoredPosition;
        Vector2 hiddenPos = originalPos + new Vector2(-800f, 0f);
        raccoonImageRect.anchoredPosition = hiddenPos;

        raccoonImageRect.DOAnchorPos(originalPos, 0.6f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => ShowDialogue(currentDialogueIndex));
        objectRect.localScale = Vector3.zero;
    }
    public void UpdateStrikes(int strikes)
    {
        for (int i = 0; i < HeartParent.transform.childCount; i++)
        {
            HeartParent.transform.GetChild(i).gameObject.SetActive(i < strikes);
        }
    }
    public void Reset()
    {
        objectiveText.text = $"This house's objective:\nA {ObjectiveManager.Instance.GetCurrentObjective().collectibleName}";
    }
    private void Update()
    {
        if (dialogueCanvasGroup.alpha > 0 && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Instantly finish current sentence
                typingTween.Kill();
                dialogueText.text = introDialogue[currentDialogueIndex];
                isTyping = false;
            }
            else
            {
                // Go to next sentence
                currentDialogueIndex++;
                if (currentDialogueIndex < introDialogue.Length)
                {
                    ShowDialogue(currentDialogueIndex);
                }
                else
                {

                    PlayerPrefs.SetInt(IntroPlayedKey, 1);
                    PlayerPrefs.Save();
                    raccoonImageRect.DOAnchorPos(raccoonImageRect.anchoredPosition + new Vector2(-800f, 0f), 0.6f).SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            dialogueCanvasGroup.DOFade(0, 0.4f).SetEase(Ease.InQuad).OnComplete(() =>
                            {
                                objectRect.localScale = Vector3.zero;
                                objectRect.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                                objectiveText.text = $"This house's objective:\nA {ObjectiveManager.Instance.GetCurrentObjective().collectibleName}";
                            });
                        });
                }
            }
        }
    }
    public void FinishObjective()
    {
        objectRect.localScale = Vector3.zero;
        objectRect.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        objectiveText.text = "Objective grabbed\nYou can escape now";
    }
    private void ShowDialogue(int index)
    {
        dialogueText.text = "";
        isTyping = true;

        string fullText = introDialogue[index];
        typingTween = DOTween.To(() => 0, x =>
        {
            dialogueText.text = fullText.Substring(0, x);
        }, fullText.Length, 1.5f)
        .SetEase(Ease.Linear)
        .OnComplete(() => isTyping = false);

        // Animate raccoon while text is typing
        raccoonImageRect.DOKill();
        raccoonImageRect.localRotation = Quaternion.identity;

        // Small shaking effect while text animates
        raccoonImageRect.DOShakePosition(1.5f, strength: new Vector2(6f, 6f), vibrato: 15, randomness: 90, snapping: false, fadeOut: true);
        raccoonImageRect.DOShakeRotation(1.5f, strength: new Vector3(0, 0, 6f), vibrato: 10, randomness: 45, fadeOut: true);
    }


    private void Player_OnConsumableUsed(ItemType itemType, int count)
    {
        TextMeshProUGUI textUI = null;
        GameObject counter = null;

        switch (itemType)
        {
            case ItemType.SmokeBomb:
                textUI = smokeBombCountText;
                counter = smokeBombCounter;

                break;
            case ItemType.CardboardBox:
                textUI = cardboardBoxCountText;
                counter = cardboardBoxCounter;
                break;
        }

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
        GameObject counter = null;

        switch (itemType)
        {
            case ItemType.SmokeBomb:
                counter = smokeBombCounter;

                break;
            case ItemType.CardboardBox:
                counter = cardboardBoxCounter;
                break;
        }
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
