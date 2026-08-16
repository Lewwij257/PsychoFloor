using UnityEngine;

public class OnAndOffArmsLevel1 : MonoBehaviour
{

    private void Awake()
    {
        OffArmsLevel1();
    }

    [SerializeField] public GameObject arms;
    [SerializeField] public GameObject pistol;

    public void OffArmsLevel1()
    {
        arms.SetActive(false);
        pistol.SetActive(false);
    }

    public void OnArmsLevel1()
    {
        arms.SetActive(true);
        pistol.SetActive(true);
    }

}
