using UnityEngine;
using UnityEngine.Events;

public class MenuPaper : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light pointLight;
    [SerializeField] private float maxIntensity = 5f;
    [SerializeField] private float transitionSpeed = 10f;

    [Header("Click")]
    [SerializeField] private UnityEvent onClick;

    private float targetIntensity;
    private bool enabledInteraction = true;

    private void Awake()
    {
        if (pointLight != null)
        {
            pointLight.intensity = 0f;
        }
    }

    private void Update()
    {
        if (pointLight == null)
            return;

        pointLight.intensity = Mathf.MoveTowards(
            pointLight.intensity,
            targetIntensity,
            transitionSpeed * Time.deltaTime
        );
    }

    private void OnMouseEnter()
    {
        if (!enabledInteraction)
            return;

        targetIntensity = maxIntensity;
    }

    private void OnMouseExit()
    {
        targetIntensity = 0f;
    }

    private void OnMouseDown()
    {

        if(gameObject.name == "Play")
        {
            GlobalGameManager.Instance.StartNewGame();
        }

        else
        {
            if (!enabledInteraction)
                return;

            onClick?.Invoke();
        }



    }

    public void SetEnabled(bool enabled)
    {
        enabledInteraction = enabled;

        if (!enabled)
        {
            targetIntensity = 0f;
        }
    }
}