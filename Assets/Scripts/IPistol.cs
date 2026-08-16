using UnityEngine;

public class Pistol : WeaponBase
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask shootableLayers;
    [SerializeField] private int damage = 34;

    public override void FireOnce()
    {
        Ray ray = new Ray(muzzle.position, muzzle.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, shootableLayers))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Проверяем слой Enemy
            if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var finalDamage = damage;
                // Определяем часть тела по тегу
                string bodyPart = hitObject.tag;

                switch (bodyPart)
                {
                    case "Head":
                        finalDamage = finalDamage * 3;
                        break;

                    case "Torso":

                        break;

                    default:
                        break;
                }

                GameObject enemyObject = hitObject.transform.root.gameObject;
                enemyObject.GetComponent<EnemyManager>().TakeDamage(finalDamage);

            }
        }

        Debug.DrawRay(muzzle.position, muzzle.forward * range, Color.red, 0.5f);
    }

    public override void Reload()
    {
        Debug.Log("Pistol reloaded!");
    }
}