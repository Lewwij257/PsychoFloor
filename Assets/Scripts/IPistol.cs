using UnityEngine;

public class Pistol : WeaponBase
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask shootableLayers;
    [SerializeField] private int damage = 34;

    public override void FireOnce()
    {
        Debug.Log("=== FIRE ONCE CALLED ===");


        GameManager.Instance.shotsOnCurrentFloor += 1;

        Ray ray = new Ray(muzzle.position, muzzle.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, shootableLayers))
        {
            GameObject hitObject = hit.collider.gameObject;

            // === Определяем тип поверхности ===
            SurfaceType surface = SurfaceType.Default;
            var surfaceIdentifier = hit.collider.GetComponent<SurfaceIdentifier>();
            if (surfaceIdentifier != null)
                surface = surfaceIdentifier.SurfaceType;

            // === Спавним импакт (как в твоём примере) ===
            Quaternion impactRot = Quaternion.LookRotation(-ray.direction, hit.normal);
            ImpactManager.Instance?.SpawnImpact(hit.point, impactRot, surface);

            // === Проверяем слой Enemy ===
            if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var finalDamage = damage;

                // Определяем часть тела по тегу
                string bodyPart = hitObject.tag;

                switch (bodyPart)
                {
                    case "Head":
                        finalDamage = finalDamage * 3;
                        GameManager.Instance.hitsOnCurrentFloor += 1;
                        GameManager.Instance.headshotsOnCurrentFloor += 1;
                        GameManager.Instance.damageDealedOnCurrentFloor += finalDamage;
                        break;
                    case "Torso":
                        GameManager.Instance.hitsOnCurrentFloor += 1;
                        GameManager.Instance.damageDealedOnCurrentFloor += finalDamage;

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