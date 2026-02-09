using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    // Referencias y estado
    public Transform destino;    // Punto al que se teletransportará el jugador o la sombra
    public bool activo = true;   // Estado del portal: abierto o cerrado

    // Referencias visuales
    [Header("Visuales")]
    public GameObject prefabAbierto; // Prefab para mostrar portal abierto
    public GameObject prefabCerrado; // Prefab para mostrar portal cerrado

    public ButtonPortalSwitch[] botonesAsociados; // Botones que pueden activar/desactivar este portal

    // Estado interno
    private GameObject instanciaAbierta;  // Instancia en escena del portal abierto
    private GameObject instanciaCerrada;  // Instancia en escena del portal cerrado
    private Collider col;                 // Collider del portal
    private bool ultimoEstadoActivo;      // Guardamos último estado para detectar cambios

    // Ciclo de vida Unity
    void Awake()
    {
        col = GetComponent<Collider>();
        // Aseguramos que el collider sea trigger para teletransportar
        if (col != null) col.isTrigger = true;

        // Instanciamos prefabs visuales
        if (prefabAbierto != null) 
            instanciaAbierta = Instantiate(prefabAbierto, transform.position, transform.rotation, transform);
        
        if (prefabCerrado != null)
        {
            instanciaCerrada = Instantiate(prefabCerrado, transform.position, transform.rotation, transform);
            instanciaCerrada.transform.localScale = prefabCerrado.transform.localScale;
        }

        // Guardamos el estado inicial
        ultimoEstadoActivo = activo;
    }

    void Start() => ActualizarVisual(); // Inicializamos visual según el estado

    // Logica por frame: sincroniza cambios de estado
    void Update()
    {
        // Detectamos cambios en el estado del portal
        if (activo != ultimoEstadoActivo)
        {
            SincronizarConPareja(activo); // Actualizamos estado del portal y su “pareja” en destino
        }
    }

    // Triggers
    void OnTriggerEnter(Collider other)
    {
        TryTeleport(other);
    }

    void OnTriggerStay(Collider other)
    {
        // Si el portal se activa mientras alguien está dentro, lo teletransportamos igualmente.
        TryTeleport(other);
    }

    // Accion principal: teletransporta jugador o sombra
    private void TryTeleport(Collider other)
    {
        if (!activo || destino == null) return; // Si el portal está cerrado o no tiene destino, no hacemos nada
        if (!TurnCoordinator.PuedeTeletransportar()) return;

        // 1. Intentamos obtener el script del Jugador
        var player = other.GetComponent<MovimientoPorBloques25D>();
        if (player != null)
        {
            TurnCoordinator.BloquearPorTeleport();
            player.Teletransportar(destino.position); // Teletransportamos al jugador usando su método
            SincronizarConPareja(false);             // Cerramos el portal después de usarlo
            return; // Salimos para no ejecutar el resto
        }

        // 2. Si no es el jugador, comprobamos si es la Sombra
        var sombra = other.GetComponent<SombraAcosadora>();
        if (sombra != null)
        {
            TurnCoordinator.BloquearPorTeleport();
            // Movimiento físico inmediato de la sombra
            sombra.Teletransportar(destino.position, true);

            SincronizarConPareja(false); // Cerramos portal después de usarlo
        }
    }

    // Sincroniza este portal con su “pareja” en el destino
    // Sincroniza el estado con el portal destino
    public void SincronizarConPareja(bool nuevoEstado)
    {
        activo = nuevoEstado;
        ultimoEstadoActivo = nuevoEstado;
        ActualizarVisual();

        if (destino != null)
        {
            // Intentamos obtener el portal en el destino para sincronizarlo
            TeleportPortal otroPortal = destino.GetComponent<TeleportPortal>();
            if (otroPortal != null && otroPortal.activo != nuevoEstado)
            {
                otroPortal.activo = nuevoEstado;
                otroPortal.ultimoEstadoActivo = nuevoEstado;
                otroPortal.ActualizarVisual();
                // Actualizamos los botones asociados del otro portal
                otroPortal.NotificarBotones();
            }
        }
        // Actualizamos botones locales
        NotificarBotones();
    }

    // Cambia la visual del portal según esté abierto o cerrado
    // Visuales
    public void ActualizarVisual()
    {
        // Nota: No desactivamos el collider por seguridad de físicas
        if (instanciaAbierta != null) instanciaAbierta.SetActive(activo);
        if (instanciaCerrada != null) instanciaCerrada.SetActive(!activo);
    }

    // Notifica a todos los botones asociados para que actualicen su apariencia
    // Notifica a botones asociados
    public void NotificarBotones()
    {
        if (botonesAsociados == null) return;
        foreach (var b in botonesAsociados)
        {
            if (b != null) b.ActualizarVisual();
        }
    }
}

