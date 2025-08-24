using UnityEngine;

public interface IDetectable
{
    Vector2 GetPosition();
    bool IsHidden { get; }
}
