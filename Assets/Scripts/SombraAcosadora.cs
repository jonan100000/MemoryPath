using UnityEngine;

public partial class SombraAcosadora : MonoBehaviour
{
    // Referencias y configuracion
    public PlayerMovement scriptJugador; // Referencia al script del jugador para leer movimientos
    public int movimientosParaActivar = 5;       // Número de pasos que el jugador debe realizar para despertar a la sombra
    public float velocidadSombra = 5f;           // Velocidad a la que se mueve la sombra

    // Estado interno
    private Rigidbody rb;                        // Rigidbody de la sombra
    private bool activa = false;                 // Indica si la sombra está despierta
    private bool muerta = false;                 // Indica si la sombra ha sido eliminada
    private SpriteRenderer spriteRenderer;       // SpriteRenderer de la sombra
    private Stair stairActual;                   // Escalera actual en la que esta la sombra

    private float xObjetivoPropio;              // Posición X objetivo para moverse por bloques
    private bool ejecutandoAccion = false;       // Controla si la sombra está en medio de un movimiento

    private int indicePasosSombra = 0;          // Índice actual en el historial de movimientos del jugador
    private int pasosPermitidos = 0;            // Tickets de movimiento disponibles para sincronizar con el jugador
    private int pasosMaximos = 0;               // Límite de pasos que puede realizar la sombra
    private int teleportsSinTurno = 0;          // Conteo de teletransportes sin terminar turno
    private const int maxTeleportsSinTurno = 10;
    private bool HaTerminadoReproduccion() => indicePasosSombra >= pasosMaximos;

    // Ciclo de vida Unity
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Estado inicial: invisible e intangible
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        GetComponent<Collider>().enabled = false;
        rb.isKinematic = true;
    }

    // API publica: estado general

    public bool EstaActiva() => activa && !muerta; // Método útil para otros scripts (similar a playerOcupado en MovimientoPorBloques)

    public void SetStair(Stair stair)
    {
        stairActual = stair;
    }

    // Se llama desde el Player para darle "tickets" de movimiento
    // API publica: recibe un paso del jugador
    public void SincronizarPaso()
    {
        if (activa && !muerta && !HaTerminadoReproduccion())
        {
            pasosPermitidos++;
        }
    }

    // Logica por frame: despertar
    void Update()
    {
        // Despertar la sombra cuando el jugador alcance los movimientos iniciales
        if (!activa && scriptJugador.movimientosRealizados >= movimientosParaActivar)
        {
            DespertarSombra();
        }
    }

    // Fisica por turnos: reproduce historial
    void FixedUpdate()
    {
        // Condiciones para no ejecutar movimientos
        if (!activa || muerta || TurnCoordinator.TeleportBloqueaMovimiento())
            return;

        if (HaTerminadoReproduccion())
        {
            // Ya no quedan pasos por reproducir, limpiamos cualquier ticket pendiente.
            pasosPermitidos = 0;
            ejecutandoAccion = false;
            return;
        }

        if (pasosPermitidos <= 0)
            return;

        // Leemos el comando grabado del historial del jugador
        PlayerMovement.TipoMovimiento comando = scriptJugador.historialComandos[indicePasosSombra];

        if (comando == PlayerMovement.TipoMovimiento.Escalera)
        {
            if (!TurnCoordinator.PuedeTeletransportar()) return;

            // Buscamos triggers de escalera cercanos para ejecutar teletransporte
            Stair escalera = stairActual;
            if (escalera == null)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
                foreach (var hit in hitColliders)
                {
                    escalera = hit.GetComponentInParent<Stair>();
                    if (escalera == null) escalera = hit.GetComponentInChildren<Stair>();
                    if (escalera != null) break;
                }
            }

            if (escalera == null)
            {
                FinalizarTurno();
                return;
            }

            escalera.EjecutarTeletransporteSombra();
            FinalizarTurno(); // Similar a liberar pasos del jugador
        }
        else
        {
            // Movimiento horizontal por bloques
            if (!ejecutandoAccion)
            {
                float direccion = (comando == PlayerMovement.TipoMovimiento.Derecha) ? 1f : -1f;
                xObjetivoPropio = rb.position.x + (direccion * scriptJugador.tamañoBloque);
                ejecutandoAccion = true;
            }

            float nuevaX = Mathf.MoveTowards(rb.position.x, xObjetivoPropio, velocidadSombra * Time.fixedDeltaTime);

            // Mantener Y y Z constantes (2.5D)
            rb.MovePosition(new Vector3(nuevaX, rb.position.y, rb.position.z));

            if (Mathf.Abs(nuevaX - xObjetivoPropio) < 0.01f)
            {
                rb.MovePosition(new Vector3(xObjetivoPropio, rb.position.y, rb.position.z));
                ejecutandoAccion = false;
                FinalizarTurno(); // Similar a desbloquear input del jugador
            }
        }
    }

    // Turno: consume un paso y limpia flags
    void FinalizarTurno()
    {
        indicePasosSombra++;
        pasosPermitidos--;
        if (pasosPermitidos < 0) pasosPermitidos = 0;
        teleportsSinTurno = 0; // Termina el turno de la sombra
        ejecutandoAccion = false; // Clave para recalcular en el próximo FixedUpdate
    }

    // Activacion inicial
    void DespertarSombra()
    {
        activa = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        GetComponent<Collider>().enabled = true;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ; // Congelar Z y rotación para 2.5D

        indicePasosSombra = 0;
        pasosMaximos = scriptJugador.movimientosRealizados; // Sombra reproduce todos los pasos que hizo el jugador hasta despertar
        pasosPermitidos = 0;
        teleportsSinTurno = 0;
    }
}



