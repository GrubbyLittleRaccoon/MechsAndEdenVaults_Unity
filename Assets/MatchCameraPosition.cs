using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MatchCameraPosition : MonoBehaviour
{
    public Camera gameCamera;

    MatchCameraPosition()
    {
    }

    void Start()
    {
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when the script is disabled
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        // Get the current position and rotation of the Scene view camera
        Vector3 sceneCameraPosition = SceneView.lastActiveSceneView.camera.transform.position;
        Quaternion sceneCameraRotation = SceneView.lastActiveSceneView.camera.transform.rotation;

        // Set the position and rotation of the in-game camera to match the Scene view camera
        this.transform.position = sceneCameraPosition;
        this.transform.rotation = sceneCameraRotation;
    }
}