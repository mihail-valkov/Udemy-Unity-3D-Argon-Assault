using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionEffectPrefab;
    [SerializeField] ParticleSystem hitEffectPrefab;
    [SerializeField] int killScore = 10;
    [SerializeField] int hitScore = 1;
    [SerializeField] float health = 6f;

    void OnParticleCollision(GameObject other)
    {
        // Decrease health by 1 for each particle collision
        health -= 1;

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
            GameManager.Instance.ScoreKeeper.AddScore(hitScore);
        }
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}
