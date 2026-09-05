using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject entryPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Game UI")]
    [SerializeField] private GameObject pauseButton;

    private bool gameStarted;
    private bool isPaused;

    private void Start()
    {
        Time.timeScale = 1f;

        gameStarted = false;
        isPaused = false;

        ShowEntryScreen();
    }

    public void StartGame()
    {
        gameStarted = true;
        isPaused = false;

        Time.timeScale = 1f;

        if (entryPanel != null)
            entryPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    public void PauseGame()
    {
        if (!gameStarted)
            return;

        if (isPaused)
            return;

        isPaused = true;

        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (pauseButton != null)
            pauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(
            currentScene.buildIndex
        );
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowEntryScreen()
    {
        Time.timeScale = 0f;

        if (entryPanel != null)
            entryPanel.SetActive(true);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(false);
    }
}