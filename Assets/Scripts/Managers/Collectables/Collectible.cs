using UnityEngine;
using DG.Tweening;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float collectAnimDuration = 0.5f;
    [SerializeField] public CollectibleSO collectibleSO;

    private bool isCollected = false;
    private Transform collector;
    private float followTimer = 0f;
    private bool startMoving;

    public void Collect(Transform collectorTransform)
    {
        if (isCollected) return;
        isCollected = true;
        collector = collectorTransform;
        SoundManager.Instance.PlayCoinSFX(transform.position);
        Debug.Log($"Collected {collectibleSO.collectibleName}!");
        AllCollectibles.Instance.totalCollectibles--;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        transform.DOScale(0.5f, collectAnimDuration).SetEase(Ease.InBack);

        followTimer = 0f;
        Invoke(nameof(SetStartMoving), 0.5f);
    }
    private void SetStartMoving()
    {
        startMoving = true;
    }
    private void Update()
    {
        if (isCollected && collector != null && startMoving)
        {
            followTimer += Time.deltaTime;

            // Smooth follow over time
            transform.position = Vector3.Lerp(transform.position, collector.position, followTimer);

            if (Vector3.Distance(transform.position, collector.position) <= 0.1f)
            {
                Destroy(gameObject);
                Player.Instance.HandleCollectibleCollected(collectibleSO);
            }
        }
    }
}
