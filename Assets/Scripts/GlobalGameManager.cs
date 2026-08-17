using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance { get; private set; }

    [Header("Scene Management")]
    public string[] levelScenes;
    public string mainMenuScene = "MainMenu";

    private int currentLevelIndex = -1;
    private bool isLoadingNextLevel = false; // флаг для отслеживания

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Подписываемся на событие загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Отписываемся
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        UpdateCurrentLevelIndex();
    }

    /// <summary>
    /// Вызывается когда сцена полностью загружена
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Обновляем индекс
        UpdateCurrentLevelIndex();

        // Сбрасываем флаг (если он был установлен)
        isLoadingNextLevel = false;

        Debug.Log($"Сцена загружена: {scene.name}, индекс: {currentLevelIndex}");
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        // Не вызываем UpdateCurrentLevelIndex() здесь — он вызовется в OnSceneLoaded
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelScenes.Length)
        {
            Debug.LogError($"Индекс {index} вне диапазона! Всего уровней: {levelScenes.Length}");
            return;
        }

        Debug.Log($"Загружаем новый уровень под индексом {index}: {levelScenes[index]}");

        currentLevelIndex = index;
        LoadScene(levelScenes[index]);
    }

    public void NextLevel()
    {
        int nextIndex = currentLevelIndex + 1;

        Debug.Log($"Попытка загрузить следующий уровень. Текущий: {currentLevelIndex}, следующий: {nextIndex}");

        if (nextIndex >= levelScenes.Length)
        {
            Debug.Log("Все уровни пройдены! Загружаем меню.");
            LoadMainMenu();
            return;
        }

        LoadLevel(nextIndex);
    }

    public void LoadMainMenu()
    {
        currentLevelIndex = -1;
        LoadScene(mainMenuScene);
    }

    public void StartNewGame()
    {
        if (GameManager.Instance != null)
        {
            // GameManager.Instance.ResetStats();
        }
        currentLevelIndex = 0;

        LoadLevel(0);
    }

    public void RestartCurrentLevel()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelScenes.Length)
        {
            LoadLevel(currentLevelIndex);
        }
        else
        {
            LoadMainMenu();
        }
    }

    public string GetCurrentLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public int GetCurrentLevelNumber()
    {
        return currentLevelIndex + 1;
    }

    private void UpdateCurrentLevelIndex()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int newIndex = System.Array.IndexOf(levelScenes, currentScene);

        if (newIndex >= 0)
        {
            currentLevelIndex = newIndex;
        }
        // Если сцена не из списка уровней (например, меню) — оставляем -1
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}