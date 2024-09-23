using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionEffectPrefab;
    [SerializeField] ParticleSystem hitEffectPrefab;
    [SerializeField] int killScore = 10;
    //[SerializeField] int hitScore = 1;
    [SerializeField] float health = 6f;

    bool isDead = false;

    void Awake()
    {
        //add a rigid body to the enemy
        var existingRb = gameObject.GetComponent<Rigidbody>();
        if (!existingRb)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        TakeDamage(1f);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        // Decrease health by 1 for each particle collision
        health -= damage;

        // Play hit effect if available
        if (hitEffectPrefab)
        {
            var position = transform.position;
            ParticleSystem hitEffect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            hitEffect.Play();
            // Destroy the hit effect after finished playing
            Destroy(hitEffect.gameObject, hitEffect.main.startLifetime.constant + hitEffect.main.duration);
        }

        // Check if health is zero or below
        if (health <= 0)
        {
            isDead = true;
            // Play explosion effect if available
            if (explosionEffectPrefab)
            {
                var position = transform.position;
                ParticleSystem explosionEffect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
                explosionEffect.Play();
                // Destroy the explosion effect after finished playing
                Destroy(explosionEffect.gameObject, explosionEffect.main.startLifetime.constant + explosionEffect.main.duration);
            }

            // Destroy the enemy
            StartCoroutine(DestroyEnemy());

            // Add score to the score keeper
            GameManager.Instance.ScoreKeeper.AddScore(killScore);
        }
        else
        {
            // Add score to the score keeper
            //GameManager.Instance.ScoreKeeper.AddScore(hitScore);
        }
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}
