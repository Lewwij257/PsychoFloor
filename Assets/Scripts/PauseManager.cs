using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;          // панель паузы (вся)
    public Button continueButton;
    public Button menuButton;

    [Header("References")]
    public PlayerController playerController; // ссылка на игрока (можно найти в Start)

    private bool isPaused = false;

    void Start()
    {
        // Находим игрока, если не назначен
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        // Скрываем панель при старте
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Назначаем кнопки
        if (continueButton != null)
            continueButton.onClick.AddListener(ResumeGame);

        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMenu);
    }

    void Update()
    {
        // Нажатие Esc
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.CapsLock))
        {
            Debug.Log("PAUSE!");
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;                 // останавливаем время

        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Отключаем управление игроком (если нужно)
        if (playerController != null)
            playerController.enabled = false;

        // Отключаем оружие/ввод (если есть отдельные скрипты)
        // Можно также отключить скрипт оружия, если он есть
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;
    }

    public void GoToMenu()
    {
        // Возвращаем время (на всякий случай)
        Time.timeScale = 1f;

        // Загружаем главное меню через GameManager
        if (GameManager.Instance != null)
        {
            SceneManager.LoadScene("MainMenu"); // название вашей сцены меню
        }
        else
        {
            // Fallback
            SceneManager.LoadScene("MainMenu");
        }
    }
}