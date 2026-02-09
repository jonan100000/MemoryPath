using UnityEngine;

public class ButtonPortalSwitch : MonoBehaviour
{
    // Referencias
    public TeleportPortal[] portales; // Lista de portales que este botón puede activar

    // Referencias visuales
    [Header("Referencias Visuales")]
    public GameObject visualActivado;   // Prefab o objeto que se muestra cuando el botón está activado
    public GameObject visualDesactivado; // Prefab u objeto cuando el botón está desactivado

    // Estado
    private int presionados = 0;

    void Start() => ActualizarVisual(); // Inicializa la apariencia del botón según el estado de los portales

    void Update()
    {
        if (EstaPresionado())
        {
            MantenerPortalesActivos();
        }
    }

    // Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (!EsEntidadValida(other)) return;

        presionados++;
        MantenerPortalesActivos();
        ActualizarVisual();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!EsEntidadValida(other)) return;

        presionados--;
        if (presionados < 0) presionados = 0;
        ActualizarVisual();
    }

    // Helpers de estado
    private bool EstaPresionado() => presionados > 0;

    private bool EsEntidadValida(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("Sombra");
    }

    // Logica principal: mantiene portales abiertos
    private void MantenerPortalesActivos()
    {
        if (portales == null) return;
        foreach (var p in portales)
        {
            if (p != null && !p.activo)
            {
                p.SincronizarConPareja(true); // Cambiamos estado y sincronizamos con su “pareja” en el destino
            }
        }
    }

    // Actualiza la visual del botón según el estado de todos los portales asociados
    // Visuales: refleja el estado de portales y presionados
    public void ActualizarVisual()
    {
        if (EstaPresionado())
        {
            if (visualActivado != null) visualActivado.SetActive(true);
            if (visualDesactivado != null) visualDesactivado.SetActive(false);
            return;
        }

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
