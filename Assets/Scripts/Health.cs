using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Health : MonoBehaviour
{
    [SerializeField][Tooltip("Would be overriden by PlayerSettings")] float health = 100f;
    [SerializeField] float fuel = 100;
    [SerializeField] ParticleSystem hitEffectPrefab;
    [SerializeField] ParticleSystem explodeEffectPrefab;
    [SerializeField] PlayableDirector playableDirector;

    Dictionary<Enemy, bool> collidedWith = new Dictionary<Enemy, bool>();

    VibrationController vibrationController;

    public float HealthValue
    {
        get { return health; }
    }

    public float FuelValue 
    { 
        get { return fuel; } 
    }

    public bool IsHealthTrackingActive { get; set; } = true;

    private void Awake() 
    {
        vibrationController = GetComponent<VibrationController>();
    }

    void Start()
    {
        if (GameManager.Instance && GameManager.Instance.PlayerSettings)
        {
            health = GameManager.Instance.PlayerSettings.MaxHealthValue;
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (!IsHealthTrackingActive)
        {
            return;
        }

        if (other.gameObject.tag == "Obstacle" || 
            other.gameObject.tag == "Terrain")
        {
            this.TakeDamage(10, gameObject.transform.position);
            return;
        }

        var enemy = other.GetComponent<Enemy>();

        if (!enemy)
        {
            //find the enemy in the parent
            enemy = other.GetComponentInParent<Enemy>();
            if (!enemy)
            {
                Debug.Log("Untracked trigger with: " + other.gameObject.name);
                return;
            }
        }
    
        if (collidedWith.ContainsKey(enemy))
        {
            return;
        }

        collidedWith.Add(enemy, true);
        //when the player collides with the enemy, the level should restart in 2 seconds
        Debug.Log("Trigger with: " + enemy.gameObject.name);

        this.TakeDamage(100, gameObject.transform.position);

        enemy.TakeDamage(100);
    }

    private void PlayHitEffect(Vector3 point)
    {
        if (hitEffectPrefab)
        {
            ParticleSystem hitEffect = Instantiate(hitEffectPrefab, new Vector3(point.x, point.y, point.z), Quaternion.identity);
            if (!hitEffect.main.playOnAwake)
            {
                hitEffect.Play();
            }
            Destroy(hitEffect.gameObject, hitEffect.main.duration + hitEffect.main.startLifetime.constantMax);
        }
    }

    private void TriggerDeathFX()
    {
        ParticleSystem hitEffect = Instantiate(explodeEffectPrefab, transform.position, Quaternion.identity);
        if (!hitEffect.main.playOnAwake)
        {
            hitEffect.Play();
        }
        Destroy(hitEffect.gameObject, hitEffect.main.duration + hitEffect.main.startLifetime.constantMax);
    }

    public void TakeDamage(float value, Vector3 point)
    {
        if (!IsHealthTrackingActive)
        {
            return;
        }

        if (vibrationController)
        {
            vibrationController.TriggerVibration();
        }

        health -= (int)value;
        if (health <= 0)
        {
            IsHealthTrackingActive = false;

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

            TriggerDeathFX();
            GameManager.Instance.LevelFailed();
        }
        else
        {
            PlayHitEffect(point);
            //cameraShake.Shake();
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

    public void TakeFuel(float value)
    {
        this.fuel -= (float)value;
        if (this.fuel <= 0)
        {
            GameManager.Instance.LevelFailed();
        }
    }
}
