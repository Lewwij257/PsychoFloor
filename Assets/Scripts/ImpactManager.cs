using System.Collections.Generic;
using UnityEngine;

public class ImpactManager : MonoBehaviour
{
    public static ImpactManager Instance { get; private set; }

    [System.Serializable]
    public struct ImpactPrefabEntry
    {
        public SurfaceType type;
        public GameObject prefab;
        public int poolSize; // размер пула для этого типа
    }

    [SerializeField] private ImpactPrefabEntry[] impactPrefabs;
    [SerializeField] private float destroyDelay = 1f; // время жизни частиц

    private Dictionary<SurfaceType, Queue<GameObject>> pools = new Dictionary<SurfaceType, Queue<GameObject>>();
    private Dictionary<SurfaceType, GameObject> prefabMap = new Dictionary<SurfaceType, GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Заполняем словари и создаём пулы
        foreach (var entry in impactPrefabs)
        {
            if (entry.prefab == null) continue;
            prefabMap[entry.type] = entry.prefab;
            var queue = new Queue<GameObject>();
            for (int i = 0; i < entry.poolSize; i++)
            {
                GameObject obj = Instantiate(entry.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            pools[entry.type] = queue;
        }
    }

    public void SpawnImpact(Vector3 position, Quaternion rotation, SurfaceType type = SurfaceType.Default)
    {
        // Если нет префаба для данного типа, используем дефолтный
        if (!prefabMap.ContainsKey(type))
            type = SurfaceType.Default;

        if (!pools.ContainsKey(type))
        {
            Debug.LogWarning($"Нет пула для типа {type}, пропускаем импакт");
            return;
        }

        var pool = pools[type];
        GameObject impact = null;

        if (pool.Count > 0)
        {
            impact = pool.Dequeue();
        }
        else
        {
            // Если пул пуст — создаём новый (или можно просто пропустить)
            impact = Instantiate(prefabMap[type]);
            Debug.LogWarning($"Пул для {type} пуст, создан новый объект");
        }

        impact.transform.position = position;
        impact.transform.rotation = rotation;
        impact.SetActive(true);

        // Автоматически возвращаем в пул через время
        StartCoroutine(ReturnToPoolAfterDelay(impact, type, destroyDelay));
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject obj, SurfaceType type, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        if (pools.ContainsKey(type))
            pools[type].Enqueue(obj);
    }
}