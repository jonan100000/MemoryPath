using UnityEngine;

public class ButtonVisualSwitch : MonoBehaviour
{
    [Header("Prefab del botón activado")]
    public GameObject botonActivadoPrefab;

    [Header("Altura del botón activado (offset Y)")]
    public float alturaOffset = 0f;

    private bool activado = false;

    public void ActivarVisual()
    {
        if (activado) return;

        if (botonActivadoPrefab != null)
        {
            Vector3 posicion = transform.position;
            posicion.y += alturaOffset;

            Instantiate(
                botonActivadoPrefab,
                posicion,
                transform.rotation,
                transform.parent
            );
        }

        activado = true;
        Destroy(gameObject);
    }
}
