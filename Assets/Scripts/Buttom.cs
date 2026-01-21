using UnityEngine;

public class SimpleButtonSwitch : MonoBehaviour
{
    [Header("Prefab del botón activado")]
    public GameObject botonActivadoPrefab;

    [Header("Altura del botón activado (offset Y)")]
    public float alturaOffset = 0f; // por defecto 0, puedes ajustar en inspector

    private bool activado = false;

    void OnTriggerEnter(Collider other)
    {
        if (activado) return; // ya activado, no hacer nada

        if (other.GetComponent<MovimientoPorBloques25D>() != null)
        {
            ActivarBoton();
        }
    }

    void ActivarBoton()
    {
        if (botonActivadoPrefab != null)
        {
            // Instanciamos el prefab activado
            Vector3 posicion = transform.position;
            posicion.y += alturaOffset; // ajustamos la altura según el offset

            Instantiate(botonActivadoPrefab, posicion, transform.rotation, transform.parent);
        }

        activado = true;

        // Destruir el botón original
        Destroy(gameObject);
    }
}
