using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SpecialTimerLevel1SecretEnding : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image blackImage;            // просто Image (не Panel!)
    [SerializeField] private float fadeDuration = 1.5f;

    private float idleTimer = 0f;
    private bool isIdle = false;
    private bool isFading = false;

    private void Update()
    {
        if (isFading) return;

        if (playerController.move == Vector2.zero)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= 15f && !isIdle)
            {
                isIdle = true;
                StartCoroutine(FadeAndLoadScene());
            }
        }
        else
        {
            idleTimer = 0f;
            isIdle = false;
        }
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
        SceneManager.LoadScene("SecretEndingScene");
    }
}