// Particle System üzerinde çalýþacak bir helper
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Particle2DCollisionSetup : MonoBehaviour
{
    public LayerMask Occluders; // Vision2D ile ayný maskeyi ver

    void Awake()
    {
        var ps = GetComponent<ParticleSystem>();
        var col = ps.collision;

        col.enabled = true;
        col.type = ParticleSystemCollisionType.World;    // Dünya ile çarpýþ
        col.mode = ParticleSystemCollisionMode.Collision2D; // 2D fizik
        col.collidesWith = Occluders;                    // <<< Ayný layer mask
        col.sendCollisionMessages = true;                // Ýstersen callback
        col.quality = ParticleSystemCollisionQuality.High;
        col.bounce = 0f;
        col.dampen = 0.2f;
        col.lifetimeLoss = 0f; // çarpýnca yok olmasýný istersen > 0
    }

    void OnParticleCollision(GameObject other)
    {
        // Çarptýðý objeye göre efekt/hasar/iz býrakma vs.
        // Vision2D tarafýna tekrar LOS kontrolü gerekmeyebilir, çünkü zaten çarptý.
    }
}
