using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    private bool isGameOver = false;
    private bool isWin = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver || isWin) return;

        isGameOver = true;
        Time.timeScale = 0f; // Pause the game

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void WinGame()
    {
        if (isGameOver || isWin) return;

        isWin = true;
        Time.timeScale = 0f; // Pause the game

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
