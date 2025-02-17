using UnityEngine;

public class FixedCanvasRotation : MonoBehaviour
{
    public Transform player; // Référence au player (ou à l'objet qui tourne)

    //LS
    void Update()
    {
        // Fixer la rotation du Canvas (par exemple, toujours face à la caméra)
        transform.rotation = Quaternion.identity; // Réinitialiser la rotation (sans rotation)
    }
}
