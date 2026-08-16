using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;          // панель с диалогом
    public TextMeshProUGUI dialogueText;      // текст
    public Image portraitImage;               // портрет (опционально)
    public Button skipButton;                 // кнопка пропуска (опционально)

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;         // задержка между буквами
    public bool playSoundPerLetter = true;    // звук на каждую букву

    [Header("Audio")]
    public AudioSource audioSource;           // источник звука
    public List<AudioClip> letterSounds;      // список звуков для букв

    [Header("Current State")]
    [TextArea(3, 10)]
    public string currentText;                // текст для отображения

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string fullText;

    void Start()
    {
        // Скрываем диалог при старте
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Назначаем кнопку пропуска
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipTyping);
    }

    void Update()
    {
        // Нажатие пробела или E для пропуска печатания
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            SkipTyping();
        }
    }

    /// <summary>
    /// Показать диалог с текстом
    /// </summary>
    public void ShowDialogue(string text, Sprite portrait = null)
    {
        fullText = text;
        currentText = text;

        // Показываем панель
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Устанавливаем портрет
        if (portraitImage != null && portrait != null)
            portraitImage.sprite = portrait;

        // Запускаем печатание
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    /// <summary>
    /// Пропустить печатание (показать весь текст сразу)
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping)
        {
            // Если идёт печатание — показываем весь текст сразу
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.text = fullText;
            isTyping = false;
        }
        else
        {
            // Если текст уже напечатан — закрываем диалог
            CloseDialogue();
        }
    }

    /// <summary>
    /// Закрыть диалог
    /// </summary>
    public void CloseDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isTyping = false;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
    }

    /// <summary>
    /// Корутина печатания текста по буквам
    /// </summary>
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            // Добавляем букву
            dialogueText.text += c;

            // Звук на каждую букву
            if (playSoundPerLetter && audioSource != null && letterSounds.Count > 0)
            {
                PlayRandomLetterSound();
            }

            // Задержка (разная для знаков препинания)
            float delay = typingSpeed;
            if (c == '.' || c == ',' || c == '!' || c == '?' || c == ';')
                delay = typingSpeed * 3f; // пауза после знаков препинания

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    /// <summary>
    /// Воспроизвести случайный звук из списка
    /// </summary>
    private void PlayRandomLetterSound()
    {
        if (letterSounds.Count == 0 || audioSource == null) return;

        // Берём первый (или случайный) звук из списка
        AudioClip clip = letterSounds[0]; // если у вас один звук

        // Если несколько — можно брать случайный
        // AudioClip clip = letterSounds[Random.Range(0, letterSounds.Count)];

        // === ГЛАВНОЕ: случайный Pitch ===
        audioSource.pitch = Random.Range(0.7f, 1f); // диапазон высоты тона

        // Небольшой разброс громкости (опционально)
        audioSource.volume = Random.Range(0.8f, 1.0f);

        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Показать диалог с автоматическим закрытием через N секунд
    /// </summary>
    public void ShowDialogueAuto(string text, float autoCloseDelay = 3f, Sprite portrait = null)
    {
        ShowDialogue(text, portrait);
        StartCoroutine(AutoClose(autoCloseDelay));
    }

    private IEnumerator AutoClose(float delay)
    {
        yield return new WaitForSeconds(delay + typingSpeed * fullText.Length);
        CloseDialogue();
    }
}