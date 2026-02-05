using UnityEngine;

public class ButtonPortalSwitch : MonoBehaviour
{
    public TeleportPortal[] portales;

    [Header("Referencias Visuales")]
    public GameObject visualActivado;
    public GameObject visualDesactivado;

    void Start() => ActualizarVisual();

    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si es el jugador O la sombra
        if (other.CompareTag("Player") || other.CompareTag("Sombra"))
        {
            foreach (var p in portales)
            {
                if (p != null && !p.activo)
                {
                    p.SincronizarConPareja(true);
                }
            }
        }
    }

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

        if (visualActivado != null) visualActivado.SetActive(todosActivos);
        if (visualDesactivado != null) visualDesactivado.SetActive(!todosActivos);
    }
}