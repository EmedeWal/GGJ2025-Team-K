using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    /// <summary>
    /// Code for a platform that moves between a series of points in a loop, going back to the first point when it reaches the last.
    /// IF you have the time you could add code to make it accelerate and deccelerate smoothly when it changes directions
    /// </summary>

    private Vector2 startPosition;

    [SerializeField] private List<Vector2> moveTargets;
    [SerializeField] private Vector2 currentTarget;
    private int currentTargetIndex = 0;

    void Start()
    {
        startPosition = transform.position;
        currentTarget = moveTargets[0];

        LevelManager.resetGameState += LevelManager_resetGameState;
    }

    private void LevelManager_resetGameState()
    {
        transform.position = startPosition;
        currentTarget = moveTargets[0];
    }

    void FixedUpdate()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, currentTarget, 0.05f);

        //If close to the target, switch to the next target on the list
        if (Vector2.Distance(transform.localPosition, currentTarget) <= 0.1f)
        {
            //Wraps around back to the first target
            if (currentTargetIndex == moveTargets.Count - 1) currentTargetIndex = 0;
            else currentTargetIndex += 1;

            currentTarget = moveTargets[currentTargetIndex];
        }
    }
}
