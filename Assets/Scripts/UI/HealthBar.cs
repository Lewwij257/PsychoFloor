using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthImage; // ссылка на Image

    void Update()
    {
        float fillAmount = (float)GameManager.Instance.Player.GetComponent<PlayerController>().currentHealth / GameManager.Instance.Player.GetComponent<PlayerController>().maxHealth;

        healthImage.fillAmount = fillAmount;



    }
}