using UnityEngine;

public class DetectPortal : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player passed through a portal at ");
        }
    }
}
