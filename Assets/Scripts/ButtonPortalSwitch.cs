using UnityEngine;

public class ButtonPortalSwitch : MonoBehaviour
{
    public TeleportPortal[] portales; // Lista de portales que este botón puede activar

    [Header("Referencias Visuales")]
    public GameObject visualActivado;   // Prefab o objeto que se muestra cuando el botón está activado
    public GameObject visualDesactivado; // Prefab u objeto cuando el botón está desactivado

    void Start() => ActualizarVisual(); // Inicializa la apariencia del botón según el estado de los portales

    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si quien entra es el jugador o la sombra
        if (other.CompareTag("Player") || other.CompareTag("Sombra"))
        {
            // Activamos todos los portales que están asociados y actualmente inactivos
            foreach (var p in portales)
            {
                if (p != null && !p.activo)
                {
                    p.SincronizarConPareja(true); // Cambiamos estado y sincronizamos con su “pareja” en el destino
                }
            }
        }
    }

    // Actualiza la visual del botón según el estado de todos los portales asociados
    public void ActualizarVisual()
    {
        bool todosActivos = true;

        // Si no hay portales, el botón aparece desactivado
        if (portales == null || portales.Length == 0) todosActivos = false;
        else
        {
            foreach (var p in portales)
            {
                if (p == null || !p.activo) { todosActivos = false; break; }
            }
        }

        // Mostramos u ocultamos los visuales según todos los portales estén activos o no
        if (visualActivado != null) visualActivado.SetActive(todosActivos);
        if (visualDesactivado != null) visualDesactivado.SetActive(!todosActivos);
    }
}
