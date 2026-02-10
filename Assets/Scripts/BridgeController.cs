using UnityEngine;

public class BridgeController : MonoBehaviour
{
    // Estado
    public bool activo = false; // Estado del puente: activado (colisión habilitada) o desactivado

    // Referencias visuales
    [Header("Visuales")]
    public GameObject visualActivado;   // Visual cuando el puente está activo (pasable)
    public GameObject visualDesactivado; // Visual cuando el puente está desactivado (no pasable)

    // Componentes
    private Collider col; // Collider del puente

    // Ciclo de vida Unity
    void Awake()
    {
        col = GetComponent<Collider>(); // Obtenemos el collider del puente
    }

    void Start() => ActualizarVisual(); // Inicializamos la apariencia del puente según el estado

    // Función que alterna el estado del puente; llamada por botones como ButtonBridgePulse
    // API publica: alterna el estado
    public void AlternarPuente()
    {
        activo = !activo;   // Cambiamos el estado
        ActualizarVisual(); // Actualizamos visual y collider
    }

    // API publica: fuerza el estado
    public void SetActivo(bool nuevoEstado)
    {
        activo = nuevoEstado;
        ActualizarVisual();
    }

    // Actualiza la visual y el collider del puente según su estado
    // Helpers de estado/visual
    public void ActualizarVisual()
    {
        if (col != null) col.enabled = activo; // Activamos o desactivamos el collider para bloquear el paso

        // Activamos o desactivamos los visuales según el estado
        if (visualActivado != null) visualActivado.SetActive(activo);
        if (visualDesactivado != null) visualDesactivado.SetActive(!activo);
    }
}
