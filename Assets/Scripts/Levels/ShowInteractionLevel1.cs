using TMPro;
using UnityEngine;

public class ShowInteractionLevel1 : MonoBehaviour
{
    private bool inZone = false;
    [SerializeField] TextMeshProUGUI interactionText;
    [SerializeField] GameObject pistolGameObject;
    [SerializeField] GameObject UI;
    [SerializeField] OnAndOffArmsLevel1 onAndOffArmsLevel1;
    [SerializeField] PlayerController playerController;
    [SerializeField] DialogueSystem dialogueSystem;
    [SerializeField] AudioSource audioSiren;

    public void ShowInteractionText()
    {
        interactionText.gameObject.SetActive(true);
        inZone = true;
    }

    public void HideInteractionText()
    {
        interactionText.gameObject.SetActive(false);
        inZone = false;
    }

    private void Update()
    {
        if (inZone)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactionText.gameObject.SetActive(false);
                onAndOffArmsLevel1.OnArmsLevel1();
                UI.SetActive(true);
                playerController.firePermission = true;
                pistolGameObject.SetActive(false);
                dialogueSystem.gameObject.SetActive(true);
                dialogueSystem.ShowDialogueAuto("Немедленно прекратите сопротивление и вернитесь к рабочему месту.");
                audioSiren.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
                
            }
        }
    }
}
