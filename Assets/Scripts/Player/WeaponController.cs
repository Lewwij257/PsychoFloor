using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponSwayAndBob weaponSwayAndBob;
    [SerializeField] public WeaponBase weapon;
    [SerializeField] private float fireRate = 0.2f;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 12;
    [SerializeField] private int totalAmmo = 48;
    private int currentAmmo;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 1.5f;

    private PlayerController playerController;
    private bool canFire = true;
    private bool wasFiring = false;
    private float fireCooldown;
    private bool isReloading = false;

    private void Start()
    {
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        // Перезарядка по R
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo && totalAmmo > 0)
            StartReload();

        // Кулдаун
        if (!canFire)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f) canFire = true;
        }

        bool isFiring = playerController.fire;

        // Стрельба (только если не перезарядка, есть патроны)
        if (!isReloading && isFiring && canFire && !wasFiring && currentAmmo > 0)
        {
            Fire();
            canFire = false;
            fireCooldown = fireRate;
        }

        wasFiring = isFiring;
    }

    private void Fire()
    {
        currentAmmo--;
        weaponSwayAndBob.Shoot();
        weapon.FireOnce();

        // Автоперезарядка если патроны кончились
        if (currentAmmo == 0 && totalAmmo > 0)
            StartReload();
    }

    private void StartReload()
    {
        if (isReloading) return;
        isReloading = true;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(reloadTime);

        // Пополняем патроны
        int needed = maxAmmo - currentAmmo;
        int toAdd = Mathf.Min(needed, totalAmmo);
        currentAmmo += toAdd;
        totalAmmo -= toAdd;

        isReloading = false;
    }
}