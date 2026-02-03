using UnityEngine;

public class BridgeController : MonoBehaviour
{
    public bool activo = false;

    [Header("Visuales")]
    public GameObject visualActivado;
    public GameObject visualDesactivado;

    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
    }

    void Start() => ActualizarVisual();

    // El botón llamará a esta función para conmutar el estado
    public void AlternarPuente()
    {
        activo = !activo;
        ActualizarVisual();
    }

    public void ActualizarVisual()
    {
        if (col != null) col.enabled = activo;
        
        if (visualActivado != null) visualActivado.SetActive(activo);
        if (visualDesactivado != null) visualDesactivado.SetActive(!activo);
    }
}
