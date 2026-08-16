using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponSwayAndBob weaponSwayAndBob;
    [SerializeField] public WeaponBase weapon;
    [SerializeField] private float fireRate = 0.2f;

    private PlayerController playerController;
    private bool canFire = true;
    private bool wasFiring = false;
    private float fireCooldown;

    private void Start()
    {
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Обработка кулдауна
        if (!canFire)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
                canFire = true;
        }

        bool isFiring = playerController.fire;

        // Стреляем ТОЛЬКО если:
        // 1. Клавиша зажата
        // 2. Можно стрелять (кулдаун прошел)
        // 3. В прошлом кадре клавиша была отпущена (новое нажатие)
        if (isFiring && canFire && !wasFiring)
        {
            Fire();
            canFire = false;
            fireCooldown = fireRate;
        }

        // Запоминаем состояние клавиши для следующего кадра
        wasFiring = isFiring;
    }

    private void Fire()
    {
        weaponSwayAndBob.Shoot();
        weapon.FireOnce();
    }
}