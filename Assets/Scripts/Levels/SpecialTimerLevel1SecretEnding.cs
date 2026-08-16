using UnityEngine;

public class SpecialTimerLevel1SecretEnding : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private float idleTimer = 0f;
    private bool isIdle = false;

    private void Update()
    {
        // Проверяем, стоит ли игрок на месте
        if (playerController.move == Vector2.zero)
        {
            // Если стоит — увеличиваем таймер
            idleTimer += Time.deltaTime;

            // Если простоял 15 секунд — выполняем код
            if (idleTimer >= 15f && !isIdle)
            {
                isIdle = true;
                OnPlayerIdleFor15Seconds();
            }
        }
        else
        {
            // Если игрок двигается — сбрасываем таймер
            idleTimer = 0f;
            isIdle = false;
        }
    }

    /// <summary>
    /// Вызывается когда игрок стоит неподвижно 15 секунд
    /// </summary>
    private void OnPlayerIdleFor15Seconds()
    {
        Debug.Log("Игрок стоит 15 секунд! Секретный контент активирован!");

        // === ТВОЙ КОД ЗДЕСЬ ===
        // Например:
        // - Показать диалог
        // - Спавнить секретного врага
        // - Активировать скрытую дверь
        // - Воспроизвести звук
        // - Запустить анимацию
    }
}