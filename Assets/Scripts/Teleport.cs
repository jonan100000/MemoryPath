using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    // Referencias y estado
    public Transform destino;    // Punto al que se teletransportara el jugador o la sombra
    public bool activo = true;   // Estado del portal: abierto o cerrado

    // Referencias visuales
    [Header("Visuales 2D")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteAbierto;
    public Sprite spriteCerrado;

    public ButtonPortalSwitch[] botonesAsociados; // Botones que pueden activar/desactivar este portal

    // Estado interno
    private Collider col;                 // Collider del portal
    private bool ultimoEstadoActivo;      // Guardamos ultimo estado para detectar cambios

    // Ciclo de vida Unity
    void Awake()
    {
        col = GetComponent<Collider>();
        // Aseguramos que el collider sea trigger para teletransportar
        if (col != null) col.isTrigger = true;

        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Guardamos el estado inicial
        ultimoEstadoActivo = activo;
    }

    void Start() => ActualizarVisual(); // Inicializamos visual segun el estado

    // Logica por frame: sincroniza cambios de estado
    void Update()
    {
        // Detectamos cambios en el estado del portal
        if (activo != ultimoEstadoActivo)
        {
            SincronizarConPareja(activo); // Actualizamos estado del portal y su "pareja" en destino
        }
    }

    // Triggers
    void OnTriggerEnter(Collider other)
    {
        TryTeleport(other);
    }

    void OnTriggerStay(Collider other)
    {
        // Si el portal se activa mientras alguien esta dentro, lo teletransportamos igualmente.
        TryTeleport(other);
    }

    // Accion principal: teletransporta jugador o sombra
    private void TryTeleport(Collider other)
    {
        if (!activo || destino == null) return; // Si el portal esta cerrado o no tiene destino, no hacemos nada
        if (!TurnCoordinator.PuedeTeletransportar()) return;

        // 1. Intentamos obtener el script del Jugador
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.Teletransportar(destino.position); // Teletransportamos al jugador usando su metodo
            SincronizarConPareja(false);             // Cerramos el portal despues de usarlo
            return; // Salimos para no ejecutar el resto
        }

        // 2. Si no es el jugador, comprobamos si es la Sombra
        var sombra = other.GetComponent<SombraAcosadora>();
        if (sombra != null)
        {
            // Movimiento fisico inmediato de la sombra
            sombra.Teletransportar(destino.position, true);

            SincronizarConPareja(false); // Cerramos portal despues de usarlo
        }
    }

    // Sincroniza este portal con su "pareja" en el destino
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

    // Cambia la visual del portal segun este abierto o cerrado
    // Visuales
    public void ActualizarVisual()
    {
        // Nota: No desactivamos el collider por seguridad de fisicas
        if (spriteRenderer != null && (spriteAbierto != null || spriteCerrado != null))
        {
            spriteRenderer.sprite = activo ? spriteAbierto : spriteCerrado;
            spriteRenderer.enabled = true;
        }
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
