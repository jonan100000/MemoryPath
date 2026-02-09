using UnityEngine;

public class ButtonBridgePulse : MonoBehaviour
{
    // Referencias
    public BridgeController puente; // Referencia al puente que este botón controla

    // Referencias visuales
    [Header("Referencias Visuales (Hijos)")]
    public GameObject visualActivado;   // Visual cuando el botón está presionado
    public GameObject visualDesactivado; // Visual cuando el botón no está presionado

    void Start() => SetVisual(false); // Inicializamos el botón en estado “no presionado”

    // Triggers
    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si el collider pertenece al jugador o a la sombra
        if (EsEntidadValida(other))
        {
            SetVisual(true); // Activamos la visual de botón presionado
            
            if (puente != null)
            {
                puente.AlternarPuente(); // Cambiamos el estado del puente (sube/baja)
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Cuando la entidad sale, desactivamos la visual
        if (EsEntidadValida(other))
        {
            SetVisual(false);
        }
    }

    // Método de ayuda para detectar si la entidad es válida (Jugador o Sombra)
    // Helpers de validacion
    private bool EsEntidadValida(Collider col)
    {
        return col.GetComponent<MovimientoPorBloques25D>() != null || 
               col.GetComponent<SombraAcosadora>() != null;
    }

    // Cambia la visual del botón según esté presionado o no
    // Helpers visuales
    private void SetVisual(bool presionado)
    {
        if (visualActivado != null) visualActivado.SetActive(presionado);
        if (visualDesactivado != null) visualDesactivado.SetActive(!presionado);
    }
}
