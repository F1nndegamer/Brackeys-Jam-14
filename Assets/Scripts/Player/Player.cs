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
    public float MultiplierChangeRate = 0.2f;
    private Rigidbody2D rb;
    private bool isRunning;
    private SpriteRenderer sprite;
    public static Player Instance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Instance = this;
        isRunning = false;
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
}
