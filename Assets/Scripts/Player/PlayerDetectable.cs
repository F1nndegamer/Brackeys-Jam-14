using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDetectable : MonoBehaviour, IDetectable
{
    public bool IsHidden { get; private set; }
    public Vector2 GetPosition() => transform.position;
    public void SetHidden(bool hidden) => IsHidden = hidden;
}
