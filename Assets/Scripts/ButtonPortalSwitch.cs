using UnityEngine;

public class ButtonPortalSwitch : MonoBehaviour
{
    public TeleportPortal[] portales; // Para saber a quién mirar

    [Header("Referencias Visuales")]
    public GameObject visualActivado;
    public GameObject visualDesactivado;

    void Start() => ActualizarVisual();

    // --- SOLO AÑADIMOS ESTO ---
    private void OnTriggerEnter(Collider other)
    {
        // Si el que pisa el botón es el jugador
        if (other.TryGetComponent<MovimientoPorBloques25D>(out _))
        {
            foreach (var p in portales)
            {
                if (p != null && !p.activo) 
                {
                    // Solo activamos si están apagados
                    p.SincronizarConPareja(true);
                }
            }
        }
    }
    // --------------------------

    public void ActualizarVisual()
    {
        bool todosActivos = true;

        if (portales == null || portales.Length == 0) todosActivos = false;
        else
        {
            foreach (var p in portales)
            {
                if (p == null || !p.activo) { todosActivos = false; break; }
            }
        }

        // Intercambio directo de visibilidad
        if (visualActivado != null) visualActivado.SetActive(todosActivos);
        if (visualDesactivado != null) visualDesactivado.SetActive(!todosActivos);
    }
}