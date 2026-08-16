using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public Button skipButton;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;
    public bool playSoundPerLetter = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public List<AudioClip> letterSounds;

    [Header("Current State")]
    [TextArea(3, 10)]
    public string currentText;

    [Header("Queue Settings")]
    public float defaultDelayBetweenPhrases = 2f;   // задержка между фразами по умолчанию

    // Внутреннее состояние
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string fullText;

    // Очередь
    private Queue<DialoguePhrase> phraseQueue = new Queue<DialoguePhrase>();
    private bool isPlayingQueue = false;
    private Coroutine queueCoroutine;
    private bool skipDelayRequested = false;   // флаг для пропуска задержки

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipTyping);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            SkipTyping();
        }
    }

    // ==================== ОДИНОЧНЫЙ РЕЖИМ (совместимость) ====================

    /// <summary>
    /// Показать одиночный диалог (очищает очередь и останавливает воспроизведение очереди)
    /// </summary>
    public void ShowDialogue(string text, Sprite portrait = null)
    {
        if (isPlayingQueue)
            StopDialogueQueue();

        ClearQueue();
        DisplayPhrase(text, portrait);
    }

    /// <summary>
    /// Показать диалог с автоматическим закрытием
    /// </summary>
    public void ShowDialogueAuto(string text, float autoCloseDelay = 3f, Sprite portrait = null)
    {
        ShowDialogue(text, portrait);
        StartCoroutine(AutoClose(autoCloseDelay + typingSpeed * fullText.Length));
    }

    private IEnumerator AutoClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseDialogue();
    }

    // ==================== РЕЖИМ ОЧЕРЕДИ ====================

    /// <summary>
    /// Добавить фразу в очередь
    /// </summary>
    /// <param name="text">Текст фразы</param>
    /// <param name="portrait">Портрет (опционально)</param>
    /// <param name="delayAfter">Задержка после этой фразы (если -1, используется defaultDelayBetweenPhrases)</param>
    public void AddPhrase(string text, Sprite portrait = null, float delayAfter = -1f)
    {
        phraseQueue.Enqueue(new DialoguePhrase(text, portrait, delayAfter));
    }

    /// <summary>
    /// Добавить несколько фраз
    /// </summary>
    public void AddPhrases(IEnumerable<DialoguePhrase> phrases)
    {
        foreach (var p in phrases)
            phraseQueue.Enqueue(p);
    }

    /// <summary>
    /// Запустить воспроизведение очереди (если она не пуста и уже не играет)
    /// </summary>
    public void StartDialogueQueue()
    {
        if (isPlayingQueue || phraseQueue.Count == 0)
            return;

        if (queueCoroutine != null)
            StopCoroutine(queueCoroutine);

        queueCoroutine = StartCoroutine(PlayQueue());
    }

    /// <summary>
    /// Остановить воспроизведение очереди, закрыть диалог и очистить очередь
    /// </summary>
    public void StopDialogueQueue()
    {
        if (queueCoroutine != null)
        {
            StopCoroutine(queueCoroutine);
            queueCoroutine = null;
        }
        isPlayingQueue = false;
        skipDelayRequested = false;
        CloseDialogue();
        // По желанию можно очистить очередь:
        // ClearQueue();
    }

    /// <summary>
    /// Очистить очередь фраз (без остановки воспроизведения, если оно идёт – остановите сначала)
    /// </summary>
    public void ClearQueue()
    {
        phraseQueue.Clear();
    }

    /// <summary>
    /// Корутина последовательного воспроизведения очереди
    /// </summary>
    private IEnumerator PlayQueue()
    {
        isPlayingQueue = true;

        while (phraseQueue.Count > 0)
        {
            DialoguePhrase phrase = phraseQueue.Dequeue();

            // Показываем текущую фразу
            DisplayPhrase(phrase.text, phrase.portrait);

            // Ждём завершения печатания
            yield return new WaitUntil(() => !isTyping);

            // Если после этой фразы очередь пуста – не ждём задержку, просто завершаем
            if (phraseQueue.Count == 0)
                break;

            // Ожидание задержки перед следующей фразой (с возможностью пропуска)
            float delay = phrase.delayAfter >= 0 ? phrase.delayAfter : defaultDelayBetweenPhrases;
            if (delay > 0)
            {
                skipDelayRequested = false;
                float timer = 0f;
                while (timer < delay)
                {
                    timer += Time.deltaTime;
                    if (skipDelayRequested)
                    {
                        skipDelayRequested = false;
                        break;
                    }
                    yield return null;
                }
            }
        }

        // Очередь завершена
        CloseDialogue();
        isPlayingQueue = false;
        queueCoroutine = null;
    }

    // ==================== ОБЩИЕ МЕТОДЫ ====================

    /// <summary>
    /// Отобразить фразу (внутренний метод, не влияет на очередь)
    /// </summary>
    private void DisplayPhrase(string text, Sprite portrait = null)
    {
        fullText = text;
        currentText = text;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (portraitImage != null && portrait != null)
            portraitImage.sprite = portrait;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    /// <summary>
    /// Закрыть диалог (панель, сброс состояния)
    /// </summary>
    public void CloseDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isTyping = false;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Сбрасываем флаг пропуска задержки (на всякий случай)
        skipDelayRequested = false;
    }

    /// <summary>
    /// Пропустить печатание или задержку (зависит от режима)
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping)
        {
            // Идёт печатание – показываем весь текст сразу
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.text = fullText;
            isTyping = false;

            // В режиме очереди после этого корутина PlayQueue сама продолжит (дождётся !isTyping)
            // В одиночном режиме ничего не делаем – диалог остаётся открытым с полным текстом
        }
        else
        {
            // Текст уже полностью напечатан (или идёт задержка между фразами)
            if (isPlayingQueue)
            {
                // Если есть ещё фразы – пропускаем задержку
                if (phraseQueue.Count > 0)
                {
                    skipDelayRequested = true;
                }
                else
                {
                    // Фраз нет – закрываем диалог и останавливаем очередь
                    StopDialogueQueue();
                }
            }
            else
            {
                // Одиночный режим – закрываем диалог
                CloseDialogue();
            }
        }
    }

    // ==================== КОРУТИНА ПЕЧАТАНИЯ ====================

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;

            if (playSoundPerLetter && audioSource != null && letterSounds.Count > 0)
            {
                PlayRandomLetterSound();
            }

            float delay = typingSpeed;
            if (c == '.' || c == ',' || c == '!' || c == '?' || c == ';')
                delay = typingSpeed * 3f;

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    private void PlayRandomLetterSound()
    {
        if (letterSounds.Count == 0 || audioSource == null) return;

        AudioClip clip = letterSounds[0];
        audioSource.pitch = Random.Range(0.7f, 1f);
        audioSource.volume = Random.Range(0.8f, 1.0f);
        audioSource.PlayOneShot(clip);
    }

    // ==================== ВСПОМОГАТЕЛЬНЫЙ КЛАСС ====================

    [System.Serializable]
    public class DialoguePhrase
    {
        [TextArea(2, 5)]
        public string text;
        public Sprite portrait;
        public float delayAfter = -1f; // -1 значит использовать глобальное значение

        public DialoguePhrase(string text, Sprite portrait = null, float delayAfter = -1f)
        {
            this.text = text;
            this.portrait = portrait;
            this.delayAfter = delayAfter;
        }
    }
}