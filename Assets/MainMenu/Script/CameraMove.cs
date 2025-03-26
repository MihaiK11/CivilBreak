using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class CameraTransition : MonoBehaviour
{
    public List<Transform> positions; // List of positions for the camera to follow
    public List<float> transitionDurations; // List of transition durations for each step

    public CinemachineVirtualCamera virtualCamera; // Reference to Cinemachine Virtual Camera

    private int currentTargetIndex = 0; // Index of the target position in the list
    private bool isTransitioning = false; // Flag to check if transition is in progress
    private float transitionTime = 0f; // Time elapsed for transition
    private float currentDuration = 1.0f; // Current transition duration

    void Start()
    {
        if (positions.Count < 2 || transitionDurations.Count < positions.Count - 1)
        {
            Debug.LogError("Make sure you have at least 2 positions and corresponding durations!");
            return;
        }

        // Set initial position & rotation
        virtualCamera.transform.position = positions[0].position;
        virtualCamera.transform.rotation = positions[0].rotation;

        StartTransition();
    }

    void Update()
    {
        if (isTransitioning)
        {
            transitioning();
        }
    }

    void StartTransition()
    {
        if (currentTargetIndex < positions.Count - 1)
        {
            isTransitioning = true;
            transitionTime = 0f; // Reset time
            currentDuration = transitionDurations[currentTargetIndex]; // Set duration for current transition
        }
        else
        {
            Debug.Log("All transitions completed!");
        }
    }

    void transitioning()
    {
        if (isTransitioning)
        {
            transitionTime += Time.deltaTime;
            float t = transitionTime / currentDuration;

            // Move smoothly between positions
            virtualCamera.transform.position = Vector3.Lerp(positions[currentTargetIndex].position, positions[currentTargetIndex + 1].position, t);

            // Rotate smoothly between rotations
            virtualCamera.transform.rotation = Quaternion.Slerp(positions[currentTargetIndex].rotation, positions[currentTargetIndex + 1].rotation, t);

            // When transition completes, move to the next position
            if (t >= 1f)
            {
                isTransitioning = false;
                currentTargetIndex++;

                // Start next transition if there are more positions
                if (currentTargetIndex < positions.Count - 1)
                {
                    StartTransition();
                }
            }
        }
    }
}
