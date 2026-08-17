using UnityEngine;

public class TestHutest : MonoBehaviour
{
    [SerializeField] DialogueSystem dialogueSystem;


    private void Start()
    {
        dialogueSystem.ShowDialogue("ХУЙХУЙХУХЙХУЙХУЙХУЙХЙУХ");
    }
}
