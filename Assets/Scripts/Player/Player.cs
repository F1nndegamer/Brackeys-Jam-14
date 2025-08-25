using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [HideInInspector] public float RangeMultiplier = 1f;
    [SerializeField] List<Sprite> playerSprites;
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask collectibleLayer; // assign in inspector

    public float MultiplierChangeRate = 0.2f;
    private Rigidbody2D rb;
    private bool isRunning;
    private SpriteRenderer sprite;
    public static Player Instance;
    private Dictionary<CollectibleSO, int> collectibleCounts = new();
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Instance = this;
        isRunning = false;
        GameInput.Instance.OnInteract += TryCollect;
    }

    private void Update()
    {
        if (Physics2D.OverlapCircle(transform.position, interactRadius, collectibleLayer))
        {
            UIManager.Instance.GetCanvas<CanvasGameplay>().ShowInteractPrompt();
        }
        else
        {
            UIManager.Instance.GetCanvas<CanvasGameplay>().HideInteractPrompt();
        }
    }
    private void FixedUpdate()
    {
        float multipier = 1f;
        Vector2 inputVector = GameInput.Instance.GetMovementVector();

        if (inputVector.sqrMagnitude > 1f)
            inputVector = inputVector.normalized;

        float currentSpeed = walkSpeed;
        sprite.sprite = playerSprites[0];
        if (GameInput.Instance.GetCrouchHeld())
        {
            currentSpeed = crouchSpeed;
            if (playerSprites.Count > 2) sprite.sprite = playerSprites[2];
            multipier -= MultiplierChangeRate;
            isRunning = false;
        }
        else if (GameInput.Instance.GetRunHeld())
        {
            currentSpeed = runSpeed;
            if (playerSprites.Count > 1) sprite.sprite = playerSprites[1];
            multipier += MultiplierChangeRate;
            isRunning = true;
        }
        RangeMultiplier = multipier;
        rb.MovePosition(rb.position + inputVector * currentSpeed * Time.fixedDeltaTime);
        NoiseEmitter noiseEmitter = GetComponent<NoiseEmitter>();
        if (noiseEmitter != null)
        {
            noiseEmitter.Emit(isRunning);
        }

        if (inputVector != Vector2.zero)
        {
            float angle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }
    private void TryCollect()
    {
        // Cast a circle around player
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, collectibleLayer);

        foreach (Collider2D hit in hits)
        {
            Collectible collectible = hit.GetComponent<Collectible>();
            if (collectible != null)
            {
                collectible.Collect(transform);
                break; // only collect one at a time
            }
        }
    }
    public void HandleCollectibleCollected(CollectibleSO collectibleSO)
    {
        if (!collectibleCounts.ContainsKey(collectibleSO))
            collectibleCounts[collectibleSO] = 0;

        collectibleCounts[collectibleSO]++;
        UIManager.Instance.GetCanvas<CanvasGameplay>().HandleCollectibleCollected(collectibleSO, collectibleCounts[collectibleSO]);

        Debug.Log($"Player now has {collectibleCounts[collectibleSO]} of {collectibleSO}");
    }
    public int GetCollectibleCount(CollectibleSO collectibleSO)
    {
        return collectibleCounts.ContainsKey(collectibleSO) ? collectibleCounts[collectibleSO] : 0;
    }
}
