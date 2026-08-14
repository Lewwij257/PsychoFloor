using System.Collections;
using UnityEngine;

public class TraceEffect : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float duration = 0.25f;         // общее время жизни
    [SerializeField] private float appearTime = 0.05f;      // время появления
    [SerializeField] private float holdTime = 0.05f;        // время удержания

    private Material material;
    private Coroutine currentCoroutine;

    // Событие завершения (для возврата в пул)
    public System.Action<TraceEffect> OnComplete;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Создаём экземпляр материала, чтобы не менять shared
        material = lineRenderer.material;
        material.SetFloat("_Dissolve", 1f);   // полностью невидим
        gameObject.SetActive(false);
    }

    public void Play(Vector3 start, Vector3 end)
    {
        // Устанавливаем позиции
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        gameObject.SetActive(true);

        // Запускаем анимацию (если уже была, останавливаем)
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimateDissolve());
    }

    private IEnumerator AnimateDissolve()
    {
        // 1. Быстрое появление (dissolve 1 → 0)
        float elapsed = 0f;
        while (elapsed < appearTime)
        {
            float t = elapsed / appearTime;
            material.SetFloat("_Dissolve", Mathf.Lerp(1f, 0f, t));
            material.SetFloat("_DissolveSofftness", 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        material.SetFloat("_Dissolve", 0f);

        // 2. Удержание
        yield return new WaitForSeconds(holdTime);

        // 3. Исчезновение (dissolve 0 → 1)
        float disappearTime = duration - appearTime - holdTime;
        if (disappearTime < 0.01f) disappearTime = 0.1f;
        elapsed = 0f;
        while (elapsed < disappearTime)
        {
            float t = elapsed / disappearTime;
            material.SetFloat("_Dissolve", Mathf.Lerp(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return null;
        }
        material.SetFloat("_Dissolve", 1f);

        // 4. Отключаем объект и уведомляем пул
        gameObject.SetActive(false);
        OnComplete?.Invoke(this);
        currentCoroutine = null;
    }

    // Опционально: если нужно обновить параметры материала в рантайме
    public void SetEmissionColor(Color color)
    {
        material.SetColor("_Emission", color);
    }
}