using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;

    private Rigidbody2D rb;
    private bool isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isRunning = false;
    }
    private void FixedUpdate()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVector();

        if (inputVector.sqrMagnitude > 1f)
            inputVector = inputVector.normalized;

        float currentSpeed = walkSpeed;

        if (GameInput.Instance.GetCrouchHeld())
        {
            currentSpeed = crouchSpeed;
            isRunning = false;
        }
        else if (GameInput.Instance.GetRunHeld())
        {
            currentSpeed = runSpeed;
            isRunning = true;
        }

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
