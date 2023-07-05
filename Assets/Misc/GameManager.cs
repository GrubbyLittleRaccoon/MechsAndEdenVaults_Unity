using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Inspector-defined references
    public PlayerInputManager playerInputManager;
    public Object Terrain; // Need this for sizing, managing enemy spawns etc.
    List<Camera> playerCameras = new List<Camera>();

    private void Awake()
    {
        // Subscribe to PlayerInputManager events
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.onPlayerJoined += PlayerJoined;
        playerInputManager.onPlayerLeft += PlayerLeft;
    }

    //Player Join event also triggers when first player is created
    private void PlayerJoined(PlayerInput playerInput)
    {
        // Add new player's camera to the list
        Camera playerCamera = playerInput.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCameras.Add(playerCamera);
        }

        // Arrange the cameras for the current player count
        CheckPlayerCount();
    }

    private void PlayerLeft(PlayerInput playerInput)
    {
        // Remove leaving player's camera from the list
        Camera playerCamera = playerInput.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCameras.Remove(playerCamera);
        }

        // Arrange the cameras for the current player count
        CheckPlayerCount();
    }

    //Checks the current player count and rearrange the cameras accordingly.
    void CheckPlayerCount()
    {
        int playerCount = PlayerInput.all.Count;

        if (playerCount > 1)
        {
            // Switch to split screen
            ArrangeCamerasForSplitScreen();
        }
        else if (playerCount == 1)
        {
            // Switch back to full screen
            ArrangeCamerasForFullScreen();
        }
    }

    void ArrangeCamerasForSplitScreen()
    {
        int playerCount = playerCameras.Count;

        for (int i = 0; i < playerCount; i++)
        {
            Camera playerCamera = playerCameras[i];

            if (playerCount == 1)
            {
                playerCamera.rect = new Rect(0, 0, 1, 1);
            }
            else if (playerCount == 2)
            {
                playerCamera.rect = new Rect(0, i * 0.5f, 1, 0.5f);
            }
            else if (playerCount == 3)
            {
                if (i == 0) // Player 1 gets more space
                {
                    playerCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
                }
                else // Players 2 and 3 get less space
                {
                    playerCamera.rect = new Rect((i - 1) * 0.5f, 0, 0.5f, 0.5f);
                }
            }
            else if (playerCount == 4)
            {
                playerCamera.rect = new Rect((i % 2) * 0.5f, (i / 2) * 0.5f, 0.5f, 0.5f);
            }
        }
    }

    void ArrangeCamerasForFullScreen()
    {
        foreach (Camera playerCamera in playerCameras)
        {
            playerCamera.rect = new Rect(0, 0, 1, 1);
        }
    }
}
