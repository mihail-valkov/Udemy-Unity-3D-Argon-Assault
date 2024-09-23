using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.WSA;

public class DisablePlayerControls : MonoBehaviour
{
    private bool activated;
    PlayerController playerController;
    private Health health;

    void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController)
        {
            health = playerController.GetComponent<Health>();
            if (!health)
            {
                Debug.LogError("Health component not found in the player controller");
            }
        }
        else
        {
            Debug.LogError("PlayerController not found in the scene");
        }
    }

    void OnEnable()
    {
        activated = true;

        // Disable the player controller

        if (playerController)
        {
            playerController.MovePlayerToSInitialPosition();
            playerController.ControlsEnabled = false;
        }

        // Disable the health tracking

        if (health)
        {
            health.IsHealthTrackingActive = false;
        }
    }

    void OnDisable()
    {
        if (!activated)
        {
            return;
        }

        // Enable the player controller
        if (playerController)
        {
            playerController.ControlsEnabled = true;
        }

        // Enable the health tracking
        if (health)
        {
            health.IsHealthTrackingActive = true;
        }
    }
}
