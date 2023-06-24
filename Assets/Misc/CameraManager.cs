using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera player1Camera;
    public Camera player2Camera;
    public Camera player3Camera;
    public Camera player4Camera;

    // Whether or not split-screen is currently active.
    private bool isSplitScreenActive = false;

    void Start()
    {
        SetSplitScreen(true); // TODO Change when we figure out multi-scene
    }

    public void SetSplitScreen(bool isActive)
    {
        if (isActive)
        {
            // Enable split-screen
            Debug.Log("split-screen enabled");
            player1Camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);       // Top-left
            player2Camera.enabled = true;
            player2Camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);    // Top-right
            player3Camera.enabled = true;
            player3Camera.rect = new Rect(0, 0, 0.5f, 0.5f);          // Bottom-left
            player4Camera.enabled = true;
            player4Camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);       // Bottom-right
        }
        else
        {
            Debug.Log("split-screen disabled");
            // Disable split-screen (full screen for player 1, other cameras are off)
            player1Camera.rect = new Rect(0, 0, 1, 1);
            player2Camera.enabled = false;
            player2Camera.rect = new Rect(0, 0, 1, 1);
            player3Camera.enabled = false;
            player3Camera.rect = new Rect(0, 0, 1, 1);
            player4Camera.enabled = false;
            player4Camera.rect = new Rect(0, 0, 1, 1);
        }

        isSplitScreenActive = isActive;
    }

    public bool GetSplitScreenStatus()
    {
        return isSplitScreenActive;
    }
}