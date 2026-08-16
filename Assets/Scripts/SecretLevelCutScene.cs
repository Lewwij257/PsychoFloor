using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SecretLevelCutScene : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private Image blackImage;            // просто Image (не Panel!)
    [SerializeField] private float fadeDuration = 1.5f;

    private float idleTimer = 0f;
    private bool isIdle = false;
    private bool isFading = false;

    private void Start()
    {
        // Активируем объект ДО вызова диалога
        if (dialogueSystem != null)
        {
            dialogueSystem.gameObject.SetActive(true);
            Debug.Log("✅ DialogPanel активирован через код");
        }

        StartCoroutine(PlayCutScene());
    }

    public IEnumerator PlayCutScene()
    {
        yield return new WaitForSeconds(3f);
        dialogueSystem.AddPhrase("Статус?", null, 5f);
        dialogueSystem.AddPhrase("Психоэмоциональные показатели стабилизированы, блокатор обновлён", null, 5f);
        dialogueSystem.AddPhrase("Распорядитесь о его повторном вступлении в должность", null, 5f);
        dialogueSystem.StartDialogueQueue();

        yield return new WaitForSeconds(18f);

        StartCoroutine(FadeAndLoadScene());

        Debug.Log("✅ CutScene запущена!");
    }

    private IEnumerator FadeAndLoadScene()
    {
        isFading = true;

        // Делаем Image видимым
        blackImage.gameObject.SetActive(true);

        // Получаем цвет
        Color color = blackImage.color;

        // Затухание
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            blackImage.color = color;
            yield return null;
        }

        // Убеждаемся, что полностью чёрный
        color.a = 1f;
        blackImage.color = color;

        // Пауза
        yield return new WaitForSeconds(0.3f);

        // Загрузка сцены
        SceneManager.LoadScene("Menu");
    }
}