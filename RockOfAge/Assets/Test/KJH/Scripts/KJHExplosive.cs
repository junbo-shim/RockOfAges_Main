using RayFire;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public LayerMask Rock; // 감지할 레이어 설정
    public ParticleSystem explosionEffectPrefab;
    public float explosionForce = 1000f;
    public float explosionRadius = 5f;

    private bool exploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!exploded && collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            exploded = true;
            Explode(collision.contacts[0].point);
        }
    }

    void Explode(Vector3 explosionPosition)
    {
        ParticleSystem explosionEffectInstance = Instantiate(explosionEffectPrefab, explosionPosition, Quaternion.identity);
        explosionEffectInstance.Play();

        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius, Rock);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, 0.35f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);

    }
}
