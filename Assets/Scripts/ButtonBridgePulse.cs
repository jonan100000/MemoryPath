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
        // Comprobamos si es el Jugador O la Sombra
        if (EsEntidadValida(other))
        {
            SetVisual(true);
            
            if (puente != null)
            {
                puente.AlternarPuente();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Comprobamos si es el Jugador O la Sombra
        if (EsEntidadValida(other))
        {
            SetVisual(false);
        }
    }

    // Método de ayuda para detectar ambas entidades
    private bool EsEntidadValida(Collider col)
    {
        return col.GetComponent<MovimientoPorBloques25D>() != null || 
               col.GetComponent<SombraAcosadora>() != null;
    }

    private void SetVisual(bool presionado)
    {
        if (visualActivado != null) visualActivado.SetActive(presionado);
        if (visualDesactivado != null) visualDesactivado.SetActive(!presionado);
    }
}