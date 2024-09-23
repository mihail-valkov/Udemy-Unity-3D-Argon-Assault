using UnityEngine;
using UnityEngine.InputSystem;

public class VibrationController : MonoBehaviour
{
    // Duration and intensity of the vibration
    public float vibrationDuration = 0.5f;
    public float leftMotorIntensity = 0.75f; // Strength for the left motor (heavy rumble)
    public float rightMotorIntensity = 0.75f; // Strength for the right motor (fine rumble)

    private float vibrationTimer = 0f;
    private Gamepad gamepad;

    void Start()
    {
        // Check if a gamepad is connected
        gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.LogWarning("No gamepad connected");
        }
    }

    void Update()
    {
        if (gamepad != null && vibrationTimer > 0)
        {
            // Reduce the timer
            vibrationTimer -= Time.deltaTime;
            
            // Stop vibration when timer reaches zero
            if (vibrationTimer <= 0)
            {
                gamepad.SetMotorSpeeds(0f, 0f);
            }
        }
    }

    public void TriggerVibration()
    {
        if (gamepad != null)
        {
            // Start the vibration
            gamepad.SetMotorSpeeds(leftMotorIntensity, rightMotorIntensity);
            vibrationTimer = vibrationDuration;
        }
    }

    private void OnDisable()
    {
        // Stop vibration when the script or game is disabled
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
}
