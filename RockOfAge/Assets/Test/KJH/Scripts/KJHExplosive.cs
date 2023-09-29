using RayFire;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : HoldObstacleBase
{
    public LayerMask Rock; // 감지할 레이어 설정
    public ParticleSystem explosionEffectPrefab;
    public float explosionForce = 100f;
    public float explosionRadius = 1f;
    public AudioClip explosion;

    private bool exploded = false;

    void PlayExplosionSound()
    {
        if (explosion != null)
        {
            GameObject soundObject = new GameObject("explosion2");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = explosion;
            audioSource.Play();
            Destroy(soundObject, explosion.length);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!exploded && collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            PlayExplosionSound();
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
        Dead();
    }
    protected override void Dead()
    {
        // ! Photon
        base.Dead();
    }
}

