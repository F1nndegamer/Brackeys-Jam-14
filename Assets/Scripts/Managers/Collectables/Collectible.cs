using UnityEngine;
using DG.Tweening;
using System;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float collectAnimDuration = 0.5f;
    [SerializeField] public CollectibleSO collectibleSO;

    private bool isCollected = false;

    public void Collect(Transform collector)
    {
        if (isCollected) return;
        isCollected = true;

        Debug.Log($"Collected {collectibleSO.collectibleName}!");
        AllCollectibles.Instance.totalCollectibles--;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack))
           .Append(transform.DOScale(0.5f, 0.3f).SetEase(Ease.InBack));

        seq.Join(transform.DOMove(collector.position, collectAnimDuration).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            Destroy(gameObject);
            Player.Instance.HandleCollectibleCollected(collectibleSO);
        });
    }
}
