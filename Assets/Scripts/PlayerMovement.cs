using UnityEngine;
using System.Collections.Generic;

public partial class PlayerMovement : MonoBehaviour
{
    // Tipos y configuracion
    // Tipos de movimientos que el jugador puede realizar
    public enum TipoMovimiento { Izquierda, Derecha, Escalera }

    public float tamanoBloque = 1f; // Distancia que cubre un paso
    // Configuracion de movimiento
    public float velocidad = 10f;   // Velocidad de movimiento horizontal
    public float distanciaSuelo = 0.2f; // Distancia para detectar si el jugador estÃ¡ sobre el suelo
    public LayerMask capaSuelo;     // Capa para raycast de suelo

    // Referencias
    public SombraAcosadora scriptSombra; // Referencia al script de la sombra acosadora
    public AudioClip sonidoMovimiento;
    public AudioSource audioSourceMovimiento;
    public AudioClip sonidoTeleport;
    public AudioSource audioSourceTeleport;

    // Estado e historial
    public int movimientosRealizados = 0; // Contador de pasos realizados por el jugador
    public List<TipoMovimiento> historialComandos = new List<TipoMovimiento>(); 
    // Lista donde guardamos todos los movimientos para que la sombra los pueda reproducir

    // Estado interno
    private Rigidbody rb;          // Rigidbody del jugador
    private float xObjetivo;       // PosiciÃ³n X a la que el jugador se dirige
    private bool moviendo = false; // Indica si el jugador estÃ¡ en medio de un movimiento
    private bool pasoPendiente = false; // Paso pendiente de completar por input
    private bool estaEnSuelo;      // Para comprobar si puede moverse
    private int teleportsSinTurno = 0; // Conteo de teletransportes sin terminar turno
    private const int maxTeleportsSinTurno = 10;
    private const float FacingRightY = 0f;
    private const float FacingLeftY = 180f;
    private const float TouchInputBufferSeconds = 0.2f;
    private float inputTactilIzquierdaHasta = -1f;
    private float inputTactilDerechaHasta = -1f;
    private float inputTactilArribaHasta = -1f;
    private float inputTactilAbajoHasta = -1f;
    private int ultimoFrameTeleportEscalera = -1;
    private float bloqueoEscaleraHasta = -1f;

    [HideInInspector] public bool enEscalera = false;       // Flag para indicar si el jugador estÃ¡ en escalera
    [HideInInspector] public bool enTeletransporte = false; // Flag para teletransporte

    // Ciclo de vida Unity
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtenemos el Rigidbody
        if (audioSourceMovimiento == null) audioSourceMovimiento = GetComponent<AudioSource>();
        if (audioSourceTeleport == null) audioSourceTeleport = audioSourceMovimiento;
        xObjetivo = rb.position.x;      // Inicializamos el objetivo X con la posiciÃ³n actual
        CongelarY();                    // Congelamos la posiciÃ³n Y y rotaciones innecesarias
    }

    // Input y logica por frame
    void Update()
    {
        ComprobarSuelo(); // Chequeamos si el jugador estÃ¡ sobre el suelo

        // Control fÃ­sico vertical
        if (estaEnSuelo && !enEscalera && !enTeletransporte)
            CongelarY(); // Congelamos Y si estÃ¡ en suelo y no estÃ¡ en escalera ni teletransporte
        else
            LiberarY();  // Permitimos movimiento vertical en el aire, escalera o teletransporte

        // BLOQUEO DE INPUT UNIFICADO:
        // Bloqueamos input si el jugador no estÃ¡ en suelo, si ya estÃ¡ moviÃ©ndose, o si la sombra estÃ¡ ocupada
        if (!PuedeRecibirInput())
            return; // Salimos del Update si alguna condiciÃ³n de bloqueo se cumple

        // LECTURA DE INPUTS
        bool moverIzquierda = Input.GetKeyDown(KeyCode.A)
                           || Input.GetKeyDown(KeyCode.LeftArrow)
                           || ConsumirInputTactil(ref inputTactilIzquierdaHasta);
        bool moverDerecha = Input.GetKeyDown(KeyCode.D)
                         || Input.GetKeyDown(KeyCode.RightArrow)
                         || ConsumirInputTactil(ref inputTactilDerechaHasta);

        if (moverIzquierda) // Mover a la izquierda
        {
            xObjetivo -= tamanoBloque;           // Calculamos la nueva posiciÃ³n X
            moviendo = true;                      // Indicamos que estamos en movimiento
            historialComandos.Add(TipoMovimiento.Izquierda); // Registramos el movimiento para la sombra
            pasoPendiente = true;
            CompletarPaso();
            ReproducirSonidoMovimiento();
            SetFacing(FacingLeftY);
        }
        else if (moverDerecha) // Mover a la derecha
        {
            xObjetivo += tamanoBloque;
            moviendo = true;
            historialComandos.Add(TipoMovimiento.Derecha);
            pasoPendiente = true;
            CompletarPaso();
            ReproducirSonidoMovimiento();
            SetFacing(FacingRightY);
        }
        // ObservaciÃ³n: No se gestionan diagonales ni inputs simultÃ¡neos, simplificando movimiento por bloques
    }

    // Fisica de movimiento por bloques
    void FixedUpdate()
    {
        if (!moviendo) return; // Solo calculamos fÃ­sica si estamos moviÃ©ndonos

        // Movemos suavemente hacia la posiciÃ³n objetivo usando MoveTowards
        float nuevaX = Mathf.MoveTowards(rb.position.x, xObjetivo, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(new Vector3(nuevaX, rb.position.y, rb.position.z));

        // Comprobamos si hemos llegado al objetivo
        if (Mathf.Abs(nuevaX - xObjetivo) < 0.01f)
        {
            rb.MovePosition(new Vector3(xObjetivo, rb.position.y, rb.position.z));
            moviendo = false;          // Terminamos el movimiento
            CompletarPaso();
        }
    }
    private void SetFacing(float yRotation)
    {
        Vector3 euler = transform.localEulerAngles;
        if (Mathf.Abs(Mathf.DeltaAngle(euler.y, yRotation)) < 0.1f)
            return;

        transform.localEulerAngles = new Vector3(euler.x, yRotation, euler.z);
    }

    // API publica: input tactil horizontal
    public void BotonIzquierda() => RegistrarInputTactil(ref inputTactilIzquierdaHasta);
    public void BotonDerecha() => RegistrarInputTactil(ref inputTactilDerechaHasta);

    // API publica: input tactil vertical para escaleras
    public void BotonArriba() => RegistrarInputTactil(ref inputTactilArribaHasta);
    public void BotonAbajo() => RegistrarInputTactil(ref inputTactilAbajoHasta);

    // API publica: consumos de input vertical
    public bool ConsumirSubir()
    {
        return Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.UpArrow)
            || ConsumirInputTactil(ref inputTactilArribaHasta);
    }

    public bool ConsumirBajar()
    {
        return Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.DownArrow)
            || ConsumirInputTactil(ref inputTactilAbajoHasta);
    }

    private void RegistrarInputTactil(ref float inputHasta)
    {
        float nuevoVencimiento = Time.unscaledTime + TouchInputBufferSeconds;
        if (nuevoVencimiento > inputHasta) inputHasta = nuevoVencimiento;
    }

    private bool ConsumirInputTactil(ref float inputHasta)
    {
        if (Time.unscaledTime > inputHasta) return false;
        inputHasta = -1f;
        return true;
    }

    private void ReproducirSonidoMovimiento()
    {
        if (sonidoMovimiento == null) return;
        if (audioSourceMovimiento != null) audioSourceMovimiento.PlayOneShot(sonidoMovimiento);
        else AudioSource.PlayClipAtPoint(sonidoMovimiento, transform.position);
    }

    private void ReproducirSonidoTeleport()
    {
        if (sonidoTeleport == null) return;
        if (audioSourceTeleport != null) audioSourceTeleport.PlayOneShot(sonidoTeleport);
        else AudioSource.PlayClipAtPoint(sonidoTeleport, transform.position);
    }
}


