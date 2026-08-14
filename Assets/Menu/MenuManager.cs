using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera menuCamera;
    [SerializeField] private CinemachineCamera settingsCamera;

    [Header("Settings UI")]
    [SerializeField] private GameObject settingsContent;
    [SerializeField] private Toggle exitToggle;

    [Header("Paper Buttons")]
    [SerializeField] private MenuPaper[] papers;

    [Header("Camera Priority")]
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority = 0;

    [Header("Settings Exit Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip exitSound;

    private bool inSettings = false;

    private void Awake()
    {
        ShowMenu(false);
    }

    public void OpenSettings()
    {
        if (inSettings)
            return;

        ShowMenu(true);
    }

    public void ExitSettings(bool value)
    {
        if (!value)
            return;

        if (!inSettings)
            return;

        if (audioSource != null && exitSound != null)
        {
            audioSource.PlayOneShot(exitSound);
        }

        ShowMenu(false);
    }

    private void ShowMenu(bool settings)
    {
        inSettings = settings;

        // Камеры
        menuCamera.Priority = settings
            ? inactivePriority
            : activePriority;

        settingsCamera.Priority = settings
            ? activePriority
            : inactivePriority;

        // Содержимое настроек
        if (settingsContent != null)
        {
            settingsContent.SetActive(settings);
        }

        // Toggle выхода
        if (exitToggle != null)
        {
            // Toggle всегда виден
            exitToggle.gameObject.SetActive(true);

            // Но реагирует только внутри настроек
            exitToggle.interactable = settings;

            // При входе всегда сбрасываем его
            if (settings)
            {
                exitToggle.SetIsOnWithoutNotify(false);
            }
        }

        // Листы
        foreach (MenuPaper paper in papers)
        {
            if (paper != null)
            {
                paper.SetEnabled(!settings);
            }
        }
    }
}