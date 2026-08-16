using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;
    private CharacterJoint[] joints;

    void Start()
    {
        // Находим все нужные компоненты на дочерних объектах
        animator = GetComponent<Animator>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        joints = GetComponentsInChildren<CharacterJoint>();

        // В начале игры включаем анимацию, выключаем физику
        EnableAnimator();


        EnableRagdoll();
    }

    // Вызови этот метод, когда персонаж должен "упасть"
    public void EnableRagdoll()
    {
        // 1. Отключаем аниматор, чтобы он не управлял костями
        if (animator != null)
            animator.enabled = false;

        // 2. Включаем физику для всех частей тела
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false; // Включаем влияние физики [citation:12]
            rb.useGravity = true;
            rb.detectCollisions = true;
        }
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
        foreach (CharacterJoint jnt in joints)
        {
            jnt.enableCollision = true; // Включаем столкновения между частями тела
        }
    }

    // Вызови этот метод, чтобы "оживить" персонажа
    public void EnableAnimator()
    {
        // 1. Включаем аниматор обратно
        if (animator != null)
            animator.enabled = true;

        // 2. Выключаем физику, чтобы анимация снова взяла контроль
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true; // Физика больше не управляет объектом [citation:12]
            rb.useGravity = false;
            // detectCollisions можно оставить включенным, но isKinematic = true сделает коллайдеры "нефизическими"
        }
        // Если вы выключали коллайдеры, включите их обратно здесь
    }
}