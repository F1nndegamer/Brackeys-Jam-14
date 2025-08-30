using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Singleton<Player>
{
    public event Action<CollectibleSO, int> OnCollectibleAmountChanged;
    public event Action<ItemType, int> OnConsumableUsed;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [HideInInspector] public float RangeMultiplier = 1f;
    [SerializeField] List<Sprite> playerSprites;
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask collectibleLayer; // assign in inspector
    [SerializeField] private ParticleSystem smokeBombFX;
    [SerializeField] private int Strikes = 5;
    public float MultiplierChangeRate = 0.2f;
    private Rigidbody2D rb;
    private bool isRunning;
    private SpriteRenderer sprite;
    private Dictionary<CollectibleSO, int> collectibleCounts = new();
    private Dictionary<CollectibleSO, int> tempCollectibles = new();
    private Animator anim;
    private float footstepTimer = 0;
    private float footstepTimerMax = 0.25f;
    private bool isSmokeBombUnlocked;
    private int noOfSmokeBombUseLeft;
    private bool isCardboardBoxUnlocked;
    private int noOfCardboardBoxUseLeft;
    private float speedBoost = 0;
    private int collectibleCountForWeight;
    private float collectibleWeight = 0.1f;
    Vector2 spawnPos;
    private void Awake()
    {
        spawnPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        isRunning = false;
        anim = GetComponent<Animator>();
        GameInput.Instance.OnInteract += TryCollect;
        GameInput.Instance.OnSmokeBomb += TryUseSmokeBomb;
        GameInput.Instance.OnCardboardBox += TryUseCardboardBox;
        GetComponent<PlayerDetectable>().IsHidden = false;
    }

    private void TryUseSmokeBomb()
    {
        if (!isSmokeBombUnlocked) return;
        if (noOfSmokeBombUseLeft <= 0) return;

        noOfSmokeBombUseLeft--;
        OnConsumableUsed?.Invoke(ItemType.SmokeBomb, noOfSmokeBombUseLeft);
        Destroy(Instantiate(smokeBombFX, transform.position, Quaternion.identity), 3f);
    }
    private void TryUseCardboardBox()
    {
        if (!isCardboardBoxUnlocked) return;
        if (noOfCardboardBoxUseLeft <= 0) return;
        noOfCardboardBoxUseLeft--;
        OnConsumableUsed?.Invoke(ItemType.CardboardBox, noOfCardboardBoxUseLeft);
        anim.SetBool("CardboardBox", true);
    }
    public void Respawn()
    {
        transform.position = new Vector3(spawnPos.x, spawnPos.y, -2f);
        ResetConsumable();
        isRunning = false;
    }
    public void OnDetected()
    {
        Strikes--;
        UIManager.Instance.GetCanvas<CanvasGameplay>().UpdateStrikes(Strikes);
        Debug.Log($"Player detected! Strikes left: {Strikes}");
        if (Strikes <= 0)
        {
            WinManager.Instance.Die();
        }
    }
    private void Update()
    {
        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepTimerMax)
        {
            footstepTimer = 0;

            if (GameInput.Instance.GetMovementVector().sqrMagnitude > 0.1f)
            {
                float volume = 1f;
                anim.SetBool("CardboardBox", false);
                Debug.Log("Moving");
                if (GameInput.Instance.GetCrouchHeld())
                {
                    footstepTimerMax = 0.35f;
                    SoundManager.Instance.PlayFootstepCrouchSound(transform.position, volume);
                }
                else if (GameInput.Instance.GetRunHeld())
                {
                    footstepTimerMax = 0.15f;
                    SoundManager.Instance.PlayFootstepWalkSound(transform.position, volume);
                }
                else
                {
                    footstepTimerMax = 0.25f;
                    SoundManager.Instance.PlayFootstepWalkSound(transform.position, volume);
                }
            }
        }

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

        float currentSpeed = walkSpeed + speedBoost - collectibleCountForWeight * collectibleWeight;
        sprite.sprite = playerSprites[0];
        if (GameInput.Instance.GetCrouchHeld())
        {
            currentSpeed = crouchSpeed + speedBoost - collectibleCountForWeight * collectibleWeight;
            if (playerSprites.Count > 2) sprite.sprite = playerSprites[2];
            multipier -= MultiplierChangeRate;
            isRunning = false;
            if (anim != null)
            {
                anim.SetBool("Walking", false);
                anim.SetBool("Running", false);
                GetComponent<PlayerDetectable>().IsHidden = true;
            }
        }

        else if (GameInput.Instance.GetRunHeld())
        {
            currentSpeed = runSpeed + speedBoost - collectibleCountForWeight * collectibleWeight;
            if (playerSprites.Count > 1) sprite.sprite = playerSprites[1];
            multipier += MultiplierChangeRate;
            isRunning = true;
            if (anim != null)
            {
                anim.SetBool("Walking", false);
                anim.SetBool("Running", true);
                GetComponent<PlayerDetectable>().IsHidden = false;
            }
        }
        else
        {
            if (anim != null)
            {
                anim.SetBool("Walking", true);
                anim.SetBool("Running", false);
                isRunning = false;
                GetComponent<PlayerDetectable>().IsHidden = false;
            }
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
    public bool CanAfford(ShopItemSO shopItemSO)
    {
        foreach (var requirement in shopItemSO.cost)
        {
            if (!collectibleCounts.ContainsKey(requirement.collectible) ||
                collectibleCounts[requirement.collectible] < requirement.amount)
            {
                Debug.Log($"Cannot Afford {shopItemSO.itemName}");
                return false;
            }
        }
        return true;
    }
    public void Purchase(ShopItemSO shopItemSO)
    {
        if (!CanAfford(shopItemSO)) return;

        foreach (var requirement in shopItemSO.cost)
        {
            collectibleCounts[requirement.collectible] -= requirement.amount;
            OnCollectibleAmountChanged?.Invoke(requirement.collectible, collectibleCounts[requirement.collectible]);
        }
        switch (shopItemSO.itemType)
        {
            case ItemType.Shoes:
                speedBoost = shopItemSO.stat;
                break;
            case ItemType.Bag:
                break;
            case ItemType.Bicep:
                collectibleWeight = shopItemSO.stat;
                break;
            case ItemType.SmokeBomb:
                UIManager.Instance.GetCanvas<CanvasGameplay>().UnlockConsumable(shopItemSO.itemType, (int)shopItemSO.stat);
                isSmokeBombUnlocked = true;
                noOfSmokeBombUseLeft = (int)shopItemSO.stat;
                break;
            case ItemType.CardboardBox:
                UIManager.Instance.GetCanvas<CanvasGameplay>().UnlockConsumable(shopItemSO.itemType, (int)shopItemSO.stat);
                isCardboardBoxUnlocked = true;
                noOfCardboardBoxUseLeft = 1;
                break;
        }
        Debug.Log($"Bought {shopItemSO.itemName}!");
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
                collectibleCountForWeight++;

                // Add to temp list
                if (!tempCollectibles.ContainsKey(collectible.collectibleSO))
                    tempCollectibles[collectible.collectibleSO] = 0;

                tempCollectibles[collectible.collectibleSO]++;
                OnCollectibleAmountChanged?.Invoke(collectible.collectibleSO, GetTotalCollectibleCount(collectible.collectibleSO));

                break; // only collect one at a time
            }
        }
    }
    public void ClearTempCollectibles()
    {
        tempCollectibles.Clear();
        collectibleCountForWeight = 0; // reset weight for collected items in this life
    }

    public void CommitTempCollectibles()
    {
        foreach (var kvp in tempCollectibles)
        {
            if (!collectibleCounts.ContainsKey(kvp.Key))
                collectibleCounts[kvp.Key] = 0;

            collectibleCounts[kvp.Key] += kvp.Value;
        }
        tempCollectibles.Clear();
    }
    public void HandleCollectibleCollected(CollectibleSO collectibleSO)
    {
        if (!tempCollectibles.ContainsKey(collectibleSO))
            tempCollectibles[collectibleSO] = 0;

        tempCollectibles[collectibleSO]++;
        ObjectiveManager.Instance.CheckCurrentObjective(collectibleSO);
        OnCollectibleAmountChanged?.Invoke(collectibleSO, GetTotalCollectibleCount(collectibleSO));
        Debug.Log($"Player now has {GetTotalCollectibleCount(collectibleSO)} of {collectibleSO}");
    }
    public int GetTotalCollectibleCount(CollectibleSO collectibleSO)
    {
        int total = 0;
        if (collectibleCounts.ContainsKey(collectibleSO))
            total += collectibleCounts[collectibleSO];
        if (tempCollectibles.ContainsKey(collectibleSO))
            total += tempCollectibles[collectibleSO];
        return total;
    }
    public int GetCollectibleCount(CollectibleSO collectibleSO)
    {
        return collectibleCounts.ContainsKey(collectibleSO) ? collectibleCounts[collectibleSO] : 0;
    }
    private void ResetConsumable()
    {
        noOfSmokeBombUseLeft = 1;
    }
}
