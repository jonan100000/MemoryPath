using UnityEngine;

public partial class SombraAcosadora : MonoBehaviour
{
    // Referencias y configuracion
    public PlayerMovement scriptJugador; // Referencia al script del jugador para leer movimientos
    public int movimientosParaActivar = 5; // Numero de pasos que el jugador debe realizar para despertar a la sombra
    public float velocidadSombra = 5f; // Velocidad a la que se mueve la sombra
    public AudioClip sonidoTeleport;
    public AudioSource audioSourceTeleport;

    // Estado interno
    private Rigidbody rb; // Rigidbody de la sombra
    private bool activa = false; // Indica si la sombra esta despierta
    private bool muerta = false; // Indica si la sombra ha sido eliminada
    private SpriteRenderer spriteRenderer; // SpriteRenderer de la sombra
    private Stair stairActual; // Escalera actual en la que esta la sombra

    private float xObjetivoPropio; // Posicion X objetivo para moverse por bloques
    private bool ejecutandoAccion = false; // Controla si la sombra esta en medio de un movimiento

    private int indicePasosSombra = 0; // Indice actual en el historial de movimientos del jugador
    private int pasosPermitidos = 0; // Tickets de movimiento disponibles para sincronizar con el jugador
    private int pasosMaximos = 0; // Limite de pasos que puede realizar la sombra
    private int teleportsSinTurno = 0; // Conteo de teletransportes sin terminar turno
    private const int maxTeleportsSinTurno = 10;
    private const float FacingRightY = 0f;
    private const float FacingLeftY = 180f;
    private bool HaTerminadoReproduccion() => indicePasosSombra >= pasosMaximos;

    // Ciclo de vida Unity
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (audioSourceTeleport == null) audioSourceTeleport = GetComponent<AudioSource>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Estado inicial: invisible e intangible
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        GetComponent<Collider>().enabled = false;
        rb.isKinematic = true;
    }

    // API publica: estado general
    public bool EstaActiva() => activa && !muerta;

    public void SetStair(Stair stair)
    {
        stairActual = stair;
    }

    // Se llama desde el Player para darle "tickets" de movimiento
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
        if (!activa && scriptJugador.movimientosRealizados >= movimientosParaActivar)
        {
            DespertarSombra();
        }
    }

    // Fisica por turnos: reproduce historial
    void FixedUpdate()
    {
        if (!activa || muerta)
            return;

        if (HaTerminadoReproduccion())
        {
            pasosPermitidos = 0;
            ejecutandoAccion = false;
            return;
        }

        if (pasosPermitidos <= 0)
            return;

        PlayerMovement.TipoMovimiento comando = scriptJugador.historialComandos[indicePasosSombra];

        if (comando == PlayerMovement.TipoMovimiento.Escalera)
        {
            if (!TurnCoordinator.PuedeTeletransportar()) return;

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
            FinalizarTurno();
        }
        else
        {
            if (!ejecutandoAccion)
            {
                float direccion = (comando == PlayerMovement.TipoMovimiento.Derecha) ? 1f : -1f;
                SetFacing(comando == PlayerMovement.TipoMovimiento.Derecha ? FacingRightY : FacingLeftY);
                xObjetivoPropio = rb.position.x + (direccion * scriptJugador.tamanoBloque);
                ejecutandoAccion = true;
            }

            float nuevaX = Mathf.MoveTowards(rb.position.x, xObjetivoPropio, velocidadSombra * Time.fixedDeltaTime);
            rb.MovePosition(new Vector3(nuevaX, rb.position.y, rb.position.z));

            if (Mathf.Abs(nuevaX - xObjetivoPropio) < 0.01f)
            {
                rb.MovePosition(new Vector3(xObjetivoPropio, rb.position.y, rb.position.z));
                ejecutandoAccion = false;
                FinalizarTurno();
            }
        }
    }

    // Turno: consume un paso y limpia flags
    void FinalizarTurno()
    {
        indicePasosSombra++;
        pasosPermitidos--;
        if (pasosPermitidos < 0) pasosPermitidos = 0;
        teleportsSinTurno = 0;
        ejecutandoAccion = false;
    }

    // Activacion inicial
    void DespertarSombra()
    {
        activa = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        GetComponent<Collider>().enabled = true;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        indicePasosSombra = 0;
        pasosMaximos = scriptJugador.movimientosRealizados;
        pasosPermitidos = 0;
        teleportsSinTurno = 0;
    }

    private void SetFacing(float yRotation)
    {
        Vector3 euler = transform.localEulerAngles;
        if (Mathf.Abs(Mathf.DeltaAngle(euler.y, yRotation)) < 0.1f)
            return;

        transform.localEulerAngles = new Vector3(euler.x, yRotation, euler.z);
    }
}
