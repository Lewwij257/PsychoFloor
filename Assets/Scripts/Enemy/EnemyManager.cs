using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Movement")]
    public float currentCharacterSpeed;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Other")]
    [SerializeField] public GameManager gameManager;


    private GameObject player;

    private void Awake()
    {
        player = gameManager.Player;
    }

    private void Update()
    {
        
    }


}
