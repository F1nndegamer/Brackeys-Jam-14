// Particle System �zerinde �al��acak bir helper
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Particle2DCollisionSetup : MonoBehaviour
{
    public LayerMask Occluders; // Vision2D ile ayn� maskeyi ver

    void Awake()
    {
        var ps = GetComponent<ParticleSystem>();
        var col = ps.collision;

        col.enabled = true;
        col.type = ParticleSystemCollisionType.World;    // D�nya ile �arp��
        col.mode = ParticleSystemCollisionMode.Collision2D; // 2D fizik
        col.collidesWith = Occluders;                    // <<< Ayn� layer mask
        col.sendCollisionMessages = true;                // �stersen callback
        col.quality = ParticleSystemCollisionQuality.High;
        col.bounce = 0f;
        col.dampen = 0.2f;
        col.lifetimeLoss = 0f; // �arp�nca yok olmas�n� istersen > 0
    }

    void OnParticleCollision(GameObject other)
    {
        // �arpt��� objeye g�re efekt/hasar/iz b�rakma vs.
        // Vision2D taraf�na tekrar LOS kontrol� gerekmeyebilir, ��nk� zaten �arpt�.
    }
}
