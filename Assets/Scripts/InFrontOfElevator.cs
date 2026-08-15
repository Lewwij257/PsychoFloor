using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{



    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER ENTER!");
        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }
}
