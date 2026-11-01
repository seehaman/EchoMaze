using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// One GameManager per level scene. It keeps the lives, creates Echoes,
// shows the on-screen text and decides when the level is won or lost.
public class GameManager : MonoBehaviour
{
    [Header("Level info")]
    public string levelTitle = "Level 1: First Echo";
    public string levelTip = "Dying is allowed. Every failed run comes back as an Echo.";
    public string nextSceneName = "Level2";   // leave empty on the last level

    [Header("Rules")]
    public int lives = 5;
    public int maxEchoes = 3;             // the oldest Echo disappears when a 4th is made
    public float respawnDelay = 1f;       // pause after dying so the player sees what hit them

    [Header("Scene references (drag in the Inspector)")]
    public PlayerController player;
    public GameObject echoPrefab;
    public Text livesText;
    public Text echoesText;
    public Text messageText;

    // true only while the player is allowed to move.
    public bool isPlaying = false;

    private bool levelOver = false;       // true after winning or losing
    private bool playerWon = false;
    private bool isRespawning = false;
    private Vector3 startPosition;        // where the player ball starts
    private List<EchoGhost> echoes = new List<EchoGhost>();

    void Start()
    {
        // Remember where the ball was placed in the editor.
        startPosition = player.transform.position;

        UpdateHUD();
        messageText.text = levelTitle + "\n" + levelTip + "\n\nPress SPACE to start";
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // R restarts the current level at any time.
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (playerWon)
            {
                GoToNextLevel();
            }
            else if (!isPlaying && !levelOver && !isRespawning)
            {
                // Start the level from the title message.
                isPlaying = true;
                messageText.text = "";
            }
        }
    }

    // Called by the player when it touches spikes, a spinner arm or an Echo.
    public void PlayerDied(string hazardName)
    {
        if (!isPlaying) return;

        isPlaying = false;
        lives = lives - 1;
        player.gameObject.SetActive(false);   // hide the ball

        // The run that just failed becomes a new Echo.
        CreateEcho(player.recordedPath);
        UpdateHUD();

        if (lives <= 0)
        {
            levelOver = true;
            messageText.text = "GAME OVER\nYou ran out of lives.\n\nPress R to try again";
        }
        else
        {
            messageText.text = DeathMessage(hazardName) + "\nThat run is now an Echo.";
            isRespawning = true;
            Invoke("Respawn", respawnDelay);
        }
    }

    void Respawn()
    {
        isRespawning = false;
        player.ResetToStart(startPosition);
        player.gameObject.SetActive(true);

        // Every Echo goes back to the start of its run and waits again.
        foreach (EchoGhost echo in echoes)
        {
            echo.RestartRun();
        }

        messageText.text = "";
        isPlaying = true;
    }

    void CreateEcho(List<Vector3> path)
    {
        // Ignore runs that are too short to replay (died instantly).
        if (path.Count < 10) return;

        // Only keep the newest Echoes: remove the oldest one first.
        if (echoes.Count >= maxEchoes)
        {
            Destroy(echoes[0].gameObject);
            echoes.RemoveAt(0);
        }

        GameObject newEcho = Instantiate(echoPrefab, path[0], Quaternion.identity);
        EchoGhost ghost = newEcho.GetComponent<EchoGhost>();
        ghost.Setup(path, this);
        echoes.Add(ghost);
    }

    // Called by the player when it reaches the green goal.
    public void LevelComplete()
    {
        if (!isPlaying) return;

        isPlaying = false;
        levelOver = true;
        playerWon = true;
        player.gameObject.SetActive(false);

        if (nextSceneName == "")
        {
            messageText.text = "YOU ESCAPED THE ECHO MAZE!\n\nPress SPACE to play again";
        }
        else
        {
            messageText.text = "LEVEL COMPLETE!\n\nPress SPACE for the next level";
        }
    }

    void GoToNextLevel()
    {
        if (nextSceneName == "")
        {
            SceneManager.LoadScene("Level1");   // finished the game: back to the first level
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // Picks a message that tells the player what killed them.
    string DeathMessage(string hazardName)
    {
        if (hazardName.Contains("Echo")) return "Your own Echo caught you!";
        if (hazardName.Contains("Arm")) return "The spinner hit you!";
        return "You rolled onto the spikes!";
    }

    void UpdateHUD()
    {
        livesText.text = "Lives: " + lives;
        echoesText.text = "Echoes: " + echoes.Count + " / " + maxEchoes;
    }
}
