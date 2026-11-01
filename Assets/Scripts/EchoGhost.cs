using System.Collections.Generic;
using UnityEngine;

// An Echo replays one failed run of the player, over and over.
// It is put on the Echo prefab. The GameManager creates Echoes with Instantiate.
public class EchoGhost : MonoBehaviour
{
    // Seconds the Echo waits after the player respawns before it starts moving.
    public float wakeDelay = 2f;

    // Seconds the Echo waits at the end of its run before replaying again.
    public float loopPause = 0.75f;

    // During the last part of any wait, the Echo flickers as a warning.
    public float warningTime = 0.5f;

    private List<Vector3> path;     // the positions recorded from the failed run
    private int step;               // which position of the path we are on
    private float waitTimer;        // counts down while the Echo is waiting
    private GameManager gameManager;
    private Renderer echoRenderer;
    private Collider echoCollider;

    // Called by the GameManager right after this Echo is created.
    public void Setup(List<Vector3> recordedPath, GameManager manager)
    {
        path = recordedPath;
        gameManager = manager;
        echoRenderer = GetComponent<Renderer>();
        echoCollider = GetComponent<Collider>();
        RestartRun();
    }

    // Go back to the beginning of the run and wait (called on every respawn).
    public void RestartRun()
    {
        step = 0;
        transform.position = path[0];
        StartWaiting(wakeDelay);
    }

    void StartWaiting(float seconds)
    {
        waitTimer = seconds;
        echoRenderer.enabled = false;   // invisible while waiting
        echoCollider.enabled = false;   // harmless while waiting
    }

    void Update()
    {
        // Flicker near the end of the wait so the player knows it is about to move.
        if (waitTimer > 0 && waitTimer < warningTime)
        {
            echoRenderer.enabled = Mathf.Repeat(Time.time * 10, 1) > 0.5f;
        }
    }

    void FixedUpdate()
    {
        // Freeze while the game is paused (start screen, death, win).
        if (!gameManager.isPlaying) return;

        // Still waiting: count down, then wake up.
        if (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0)
            {
                echoRenderer.enabled = true;
                echoCollider.enabled = true;   // now touching it costs a life
            }
            return;
        }

        // Move to the next recorded position. The player recorded one position
        // per FixedUpdate, so playing one per FixedUpdate gives the same speed.
        transform.position = path[step];
        step++;

        // End of the run: start again from the beginning after a short pause.
        if (step >= path.Count)
        {
            step = 0;
            transform.position = path[0];
            StartWaiting(loopPause);
        }
    }
}
