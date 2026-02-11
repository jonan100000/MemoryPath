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

    [HideInInspector] public bool enEscalera = false;       // Flag para indicar si el jugador estÃ¡ en escalera
    [HideInInspector] public bool enTeletransporte = false; // Flag para teletransporte

    // Ciclo de vida Unity
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtenemos el Rigidbody
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
        if (Input.GetKeyDown(KeyCode.A)) // Mover a la izquierda
        {
            xObjetivo -= tamanoBloque;           // Calculamos la nueva posiciÃ³n X
            moviendo = true;                      // Indicamos que estamos en movimiento
            historialComandos.Add(TipoMovimiento.Izquierda); // Registramos el movimiento para la sombra
            pasoPendiente = true;
            CompletarPaso();
            SetFacing(FacingLeftY);
        }
        else if (Input.GetKeyDown(KeyCode.D)) // Mover a la derecha
        {
            xObjetivo += tamanoBloque;
            moviendo = true;
            historialComandos.Add(TipoMovimiento.Derecha);
            pasoPendiente = true;
            CompletarPaso();
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
}


