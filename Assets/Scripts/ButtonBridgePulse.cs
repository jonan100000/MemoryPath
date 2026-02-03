using UnityEngine;

public class ButtonBridgePulse : MonoBehaviour
{
    public BridgeController puente;

    [Header("Referencias Visuales (Hijos)")]
    public GameObject visualActivado;
    public GameObject visualDesactivado;

    void Start() => SetVisual(false);

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovimientoPorBloques25D>(out _))
        {
            SetVisual(true); // El botón se ve presionado
            
            if (puente != null)
            {
                puente.AlternarPuente(); // Cambia el estado del puente (On/Off)
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MovimientoPorBloques25D>(out _))
        {
            SetVisual(false); // El botón vuelve a su estado normal al quitar el pie
        }
    }

    private void SetVisual(bool presionado)
    {
        if (visualActivado != null) visualActivado.SetActive(presionado);
        if (visualDesactivado != null) visualDesactivado.SetActive(!presionado);
    }
}