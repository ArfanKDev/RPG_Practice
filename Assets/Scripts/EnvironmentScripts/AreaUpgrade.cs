using UnityEngine;

public class AreaUpgrade : MonoBehaviour
{

    [Header("Old Area")]
    public GameObject oldArea;

    [Header("Expanded Area")]
    public GameObject expansionRoom;

    private bool upgraded = false;

    void Start()
    {
        expansionRoom.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (upgraded)
            return;

        if (other.CompareTag("Player"))
        {
            UpgradeArea();
        }
    }

    private void UpgradeArea()
    {
        upgraded = true;

        // Old area OFF
        oldArea.SetActive(false);

        // Expanded area ON
        expansionRoom.SetActive(true);
    }
}

