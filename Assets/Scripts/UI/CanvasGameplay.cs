using TMPro;
using UnityEngine;
using DG.Tweening;

public class CanvasGameplay : UICanvas
{
    [SerializeField] private TextMeshProUGUI[] collectibleTextArray;
    [SerializeField] private CollectibleSOArray collectibleSOArray;
    [SerializeField] private GameObject interactPrompt;
    private void Awake()
    {
        Player.Instance.OnCollectibleAmountChanged += Player_OnCollectibleAmountChanged;
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
    public void ShowInteractPrompt()
    {
        interactPrompt.SetActive(true);
    }
    public void HideInteractPrompt()
    {
        interactPrompt.SetActive(false);
    }
}
