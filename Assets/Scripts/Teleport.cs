using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    public Transform destino;
    public bool activo = true;

    [Header("Visuales")]
    public GameObject prefabAbierto;
    public GameObject prefabCerrado;

    public ButtonPortalSwitch[] botonesAsociados;

    private GameObject instanciaAbierta;
    private GameObject instanciaCerrada;
    private Collider col;
    private bool ultimoEstadoActivo;

    void Awake()
    {
        col = GetComponent<Collider>();
        // Asegúrate de que el collider sea siempre Trigger para teletransportar
        if (col != null) col.isTrigger = true;

        if (prefabAbierto != null) 
            instanciaAbierta = Instantiate(prefabAbierto, transform.position, transform.rotation, transform);
        
        if (prefabCerrado != null)
        {
            instanciaCerrada = Instantiate(prefabCerrado, transform.position, transform.rotation, transform);
            instanciaCerrada.transform.localScale = prefabCerrado.transform.localScale;
        }
        
        ultimoEstadoActivo = activo;
    }

    void Start() => ActualizarVisual();

    void Update()
    {
        if (activo != ultimoEstadoActivo)
        {
            SincronizarConPareja(activo);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Solo teletransporta si está activo
        if (!activo || destino == null) return;

        if (other.TryGetComponent<MovimientoPorBloques25D>(out var player))
        {
            player.Teletransportar(destino.position);
            SincronizarConPareja(false); // Se apagan ambos tras el uso
        }
    }

    public void SincronizarConPareja(bool nuevoEstado)
    {
        activo = nuevoEstado;
        ultimoEstadoActivo = nuevoEstado;
        ActualizarVisual();

        if (destino != null)
        {
            TeleportPortal otroPortal = destino.GetComponent<TeleportPortal>();
            if (otroPortal != null && otroPortal.activo != nuevoEstado)
            {
                otroPortal.activo = nuevoEstado;
                otroPortal.ActualizarVisual();
                // Importante: que el otro también actualice sus botones
                otroPortal.NotificarBotones();
            }
        }
        NotificarBotones();
    }

    public void ActualizarVisual()
    {
        // OPCIONAL: Si el botón es el único que lo activa, 
        // NO desactives el collider para evitar errores de físicas.
        // if (col != null) col.enabled = activo; 

        if (instanciaAbierta != null) instanciaAbierta.SetActive(activo);
        if (instanciaCerrada != null) instanciaCerrada.SetActive(!activo);
    }

    public void NotificarBotones()
    {
        if (botonesAsociados == null) return;
        foreach (var b in botonesAsociados)
        {
            if (b != null) b.ActualizarVisual();
        }
    }
}