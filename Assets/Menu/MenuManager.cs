using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera menuCamera;
    [SerializeField] private CinemachineCamera settingsCamera;
    [SerializeField] private CinemachineCamera creditsCamera;

    [Header("Settings UI")]
    [SerializeField] private GameObject settingsContent;
    [SerializeField] private Toggle settingsExitToggle;

    [Header("Credits UI")]
    [SerializeField] private GameObject creditsContent;
    [SerializeField] private Button creditsExitButton;

    [Header("Paper Buttons")]
    [SerializeField] private MenuPaper[] papers;

    [Header("Camera Priority")]
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority = 0;

    [Header("Settings Exit Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip exitSound;

    private enum MenuState
    {
        Main,
        Settings,
        Credits
    }

    private MenuState currentState = MenuState.Main;

    private void Awake()
    {
        ShowMain();
    }

    public void ShowMain()
    {
        currentState = MenuState.Main;
        UpdateMenu();
    }

    public void OpenSettings()
    {
        if (currentState == MenuState.Settings) return;
        currentState = MenuState.Settings;
        UpdateMenu();
    }

    public void ExitSettings(bool value)
    {
        if (!value) return;
        if (currentState != MenuState.Settings) return;
        PlayExitSound();
        ShowMain();
    }

    public void OpenCredits()
    {
        if (currentState == MenuState.Credits) return;
        currentState = MenuState.Credits;
        UpdateMenu();
    }

    public void ExitCredits()
    {
        if (currentState != MenuState.Credits) return;
        ShowMain();
    }

    private void UpdateMenu()
    {
        bool isMain = currentState == MenuState.Main;
        bool isSettings = currentState == MenuState.Settings;
        bool isCredits = currentState == MenuState.Credits;

        menuCamera.Priority = isMain
            ? activePriority
            : inactivePriority;

        settingsCamera.Priority = isSettings
            ? activePriority
            : inactivePriority;

        creditsCamera.Priority = isCredits
            ? activePriority
            : inactivePriority;

        if (settingsContent != null) settingsContent.SetActive(isSettings);

        if (settingsExitToggle != null)
        {
            settingsExitToggle.gameObject.SetActive(true);
            settingsExitToggle.interactable = isSettings;

            if (isSettings)settingsExitToggle.SetIsOnWithoutNotify(false);
        }

        if (creditsContent != null) creditsContent.SetActive(isCredits);

        if (creditsExitButton != null)
        {
            creditsExitButton.gameObject.SetActive(true);
            creditsExitButton.interactable = isCredits;
        }

        bool papersEnabled = isMain;

        foreach (MenuPaper paper in papers)
        {
            if (paper != null)paper.SetEnabled(papersEnabled);
        }
    }

    private void PlayExitSound()
    {
        if (audioSource != null && exitSound != null) audioSource.PlayOneShot(exitSound);
    }
}