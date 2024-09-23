using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{

    [Header("Input")]
    [SerializeField] InputAction playerInput;
    [SerializeField] InputAction shootInput;

    [Header("Movement Settings")]
    [SerializeField] float contorlSpeed = 15;
    [SerializeField] float xContorlRange = 17;
    [SerializeField] float yContorlRange = 15;
    [SerializeField] float yawPositionFactor = 2;
    [SerializeField] float positionPitchFactor = 2;
    [SerializeField] float controlPitchFactor = 10;
    [SerializeField] float controlRollFactor = 15;
    [SerializeField] float cameraYOffset = 3;
    [SerializeField] AudioClip[] laserSounds;

    [Header("Shooting Settings")]
    //[SerializeField] float shootingInterval = 0.1f;
    [SerializeField] ParticleSystem[] lasers;

    //[SerializeField] GameObject bulletPrefab;
    //private float lastShootingTime;

    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 previousInput = Vector2.zero;
    private float accelerationSmoothTime = 0.05f;
    private float decelerationSmoothTime = 0.02f;
    float lastInputTime;

    Vector2 input;
    Health playerHealth;
    
    bool controlsEnabled = true;
    private Vector3 playerInitialPosition;
    private Vector3 playerInitialRotation;


    // Start is called before the first frame update
    void Start()
    {
        playerInitialPosition = transform.localPosition;
        playerInitialRotation = transform.localRotation.eulerAngles;
    } 

    void Awake()
    {
        playerHealth = GetComponent<Health>();

        //find all child colliders and set them as triggers
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        ControlsEnabled = true;
    }

    private void OnDisable()
    {
        ControlsEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        input = playerInput.ReadValue<Vector2>();
        Shoot();
        Move();

        //if P is pressed, slow down the motion animation
        if (playerHealth)
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                playerHealth.SlowDownMotion();
            }
            else if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                playerHealth.ResumeMotion();
            }
        }
    }

    private bool IsKeyboardInput()
    {
        return playerInput.IsPressed();
    }

    private void Move()
    {
        bool isKeyboardInput = IsKeyboardInput() && input != Vector2.zero;

        if (isKeyboardInput)
        {
            lastInputTime = Time.time;
        }

        float timeSinceLastInput = Time.time - lastInputTime;
        bool isDecelerating = timeSinceLastInput > 0.03f; // Small threshold to differentiate between no input and very small input

        float smoothTime = isDecelerating ? decelerationSmoothTime : accelerationSmoothTime;

        // Apply gradual change to the velocity
        currentVelocity = Vector2.SmoothDamp(currentVelocity, input, ref currentVelocity, smoothTime);

        // If we're decelerating and velocity is very small, set it to zero to prevent floating
        if (isDecelerating && currentVelocity.sqrMagnitude < 0.02f)
        {
            currentVelocity = Vector2.zero;
        }

        // Store the current input for next frame's comparison
        previousInput = input;
        // Use the smoothed velocity for movement
        Vector3 movement = new Vector3(currentVelocity.x, currentVelocity.y, 0f);
        
        float horizontal = movement.x;
        float vertical = movement.y;

        transform.localPosition += movement * Time.deltaTime * contorlSpeed;

        //clamp localPosition within screen bounds
        var x = Mathf.Clamp(transform.localPosition.x, -xContorlRange / 2, xContorlRange / 2);
        var y = Mathf.Clamp(transform.localPosition.y, -yContorlRange / 2 + cameraYOffset, yContorlRange / 2 + cameraYOffset);

        transform.localPosition = new Vector3(x, y, 0f);

        //rotate on y while moving left and right to increase shooting range, give z axes is forward

        var xRotation = (y - cameraYOffset) * -positionPitchFactor - vertical * controlPitchFactor;
        var yRotation = x * yawPositionFactor;
        var zRotation = -horizontal * controlRollFactor;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
    }

    public void MovePlayerToSInitialPosition()
    {
        //move player gradually to the initial position
        StartCoroutine(MovePlayerToInitialPosition());
    }

    private IEnumerator MovePlayerToInitialPosition()
    {
        float duration = 1.5f;
        float elapsedTime = 0;
        Vector3 initialPosition = playerInitialPosition;

        //Vector3 initialRotation = playerInitialRotation;

        Vector3 initialLocalPosition = transform.localPosition;
        //Vector3 initialLocalRotation = transform.localRotation.eulerAngles;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(initialLocalPosition, initialPosition, elapsedTime / duration);
            //transform.localRotation = Quaternion.Euler(Vector3.Lerp(initialLocalRotation, initialRotation, elapsedTime / duration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialPosition;
        //transform.localRotation = Quaternion.Euler(initialRotation);
    }

    private void Shoot()
    {

        SetLasersEnabled(shootInput.IsPressed());
        
        //once every 100 milliseconds
        // if (shootInput.IsPressed() && Time.time - lastShootingTime > shootingInterval) 
        // {
        //     lastShootingTime = Time.time; 
        //     Instantiate(bulletPrefab, transform.position, transform.rotation);
        // }
    }

    private void SetLasersEnabled(bool active)
    {
        if (laserSounds.Length > 0 && active)
        {
            //play random laser sound
            PlayRandomSound(laserSounds);   
        }

        foreach (var laser in lasers)
        {
            //enalbe emission component
            var emission = laser.emission;
            emission.enabled = active;
        }
    }

    float previousSoundTime = 0;
    private void PlayRandomSound(AudioClip[] clips)
    {
        //play random sound and wait for 200 milliseconds before playing another sound
        if (Time.time - previousSoundTime < 0.08f)
        {
            return;
        }

        previousSoundTime = Time.time;
        var randomIndex = UnityEngine.Random.Range(0, clips.Length);
        //var audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(clips[randomIndex]);
        AudioSource.PlayClipAtPoint(clips[randomIndex], transform.position);
    }

    public bool ControlsEnabled
    {
        get
        {
            return controlsEnabled;
        }
        set
        {
            controlsEnabled = value;
            if (controlsEnabled)
            {
                playerInput.Enable();
                shootInput.Enable();
            }
            else
            {
                playerInput.Disable();
                shootInput.Disable();
            }
        }
    }
}
