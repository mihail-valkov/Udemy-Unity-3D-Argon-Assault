using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionEffectPrefab;
    [SerializeField] PlayableDirector playableDirector;

    private void OnTriggerEnter(Collider other)
    {
        //when the player collides with the enemy, the level should restart in 2 seconds
        Debug.Log("Trigger with: " + other.gameObject.name);

        Health playerHealth = GetComponent<Health>();
        playerHealth.TakeDamage(100, gameObject.transform.position);

        // if (explosionEffectPrefab)
        // {
        //     var position = transform.position;
        //     ParticleSystem explosionEffect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
        //     explosionEffect.Play();
        // }

        //disable the player's movement
        GetComponent<PlayerController>().ControlsEnabled = false;

        //hide player
        Invoke("HidePlayer", 0.2f);

        //slow down the motion animation to see the explosion effect
        if (playableDirector)
        {
            //playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0.2f);
            //gradually slow down the motion animation
            SlowDownMotion();
        }
    }

    private void HidePlayer()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    
    public void SlowDownMotion(float endSpeed = 0.1f)
    {
        StartCoroutine(SlowDownMotionCo(endSpeed));
    }

    public void ResumeMotion()
    {
        StartCoroutine(SlowDownMotionCo(1f));
    }

    private IEnumerator SlowDownMotionCo(float endSpeed)
    {
        float slowDownTime = 1;
        float currentTime = 0;
        float startSpeed = (float)playableDirector.playableGraph.GetRootPlayable(0).GetSpeed();
        while (currentTime < slowDownTime)
        {
            currentTime += Time.deltaTime;
            float newSpeed = Mathf.Lerp(startSpeed, endSpeed, EasingFunction(currentTime / slowDownTime));
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(newSpeed);
            yield return null;
        }
    }

    float EasingFunction(float x)
    {
        return x * x;
    }
}
