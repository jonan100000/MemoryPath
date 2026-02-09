using UnityEngine;
using System.Collections.Generic;

public partial class MovimientoPorBloques25D : MonoBehaviour
{
    // Tipos y configuracion
    // Tipos de movimientos que el jugador puede realizar
    public enum TipoMovimiento { Izquierda, Derecha, Escalera }

    public float tamañoBloque = 1f; // Distancia que cubre un paso
    // Configuracion de movimiento
    public float velocidad = 10f;   // Velocidad de movimiento horizontal
    public float distanciaSuelo = 0.2f; // Distancia para detectar si el jugador está sobre el suelo
    public LayerMask capaSuelo;     // Capa para raycast de suelo

    // Referencias
    public SombraAcosadora scriptSombra; // Referencia al script de la sombra acosadora

    // Estado e historial
    public int movimientosRealizados = 0; // Contador de pasos realizados por el jugador
    public List<TipoMovimiento> historialComandos = new List<TipoMovimiento>(); 
    // Lista donde guardamos todos los movimientos para que la sombra los pueda reproducir

    // Estado interno
    private Rigidbody rb;          // Rigidbody del jugador
    private float xObjetivo;       // Posición X a la que el jugador se dirige
    private bool moviendo = false; // Indica si el jugador está en medio de un movimiento
    private bool estaEnSuelo;      // Para comprobar si puede moverse
    private int teleportsSinTurno = 0; // Conteo de teletransportes sin terminar turno
    private const int maxTeleportsSinTurno = 10;

    [HideInInspector] public bool enEscalera = false;       // Flag para indicar si el jugador está en escalera
    [HideInInspector] public bool enTeletransporte = false; // Flag para teletransporte

    // Ciclo de vida Unity
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtenemos el Rigidbody
        xObjetivo = rb.position.x;      // Inicializamos el objetivo X con la posición actual
        CongelarY();                    // Congelamos la posición Y y rotaciones innecesarias
    }

    // Input y logica por frame
    void Update()
    {
        ComprobarSuelo(); // Chequeamos si el jugador está sobre el suelo

        // Control físico vertical
        if (estaEnSuelo && !enEscalera && !enTeletransporte)
            CongelarY(); // Congelamos Y si está en suelo y no está en escalera ni teletransporte
        else
            LiberarY();  // Permitimos movimiento vertical en el aire, escalera o teletransporte

        // BLOQUEO DE INPUT UNIFICADO:
        // Bloqueamos input si el jugador no está en suelo, si ya está moviéndose, o si la sombra está ocupada
        if (!PuedeRecibirInput())
            return; // Salimos del Update si alguna condición de bloqueo se cumple

        // LECTURA DE INPUTS
        if (Input.GetKeyDown(KeyCode.A)) // Mover a la izquierda
        {
            xObjetivo -= tamañoBloque;           // Calculamos la nueva posición X
            moviendo = true;                      // Indicamos que estamos en movimiento
            historialComandos.Add(TipoMovimiento.Izquierda); // Registramos el movimiento para la sombra
        }
        else if (Input.GetKeyDown(KeyCode.D)) // Mover a la derecha
        {
            xObjetivo += tamañoBloque;
            moviendo = true;
            historialComandos.Add(TipoMovimiento.Derecha);
        }
        // Observación: No se gestionan diagonales ni inputs simultáneos, simplificando movimiento por bloques
    }

    // Fisica de movimiento por bloques
    void FixedUpdate()
    {
        if (!moviendo) return; // Solo calculamos física si estamos moviéndonos

        // Movemos suavemente hacia la posición objetivo usando MoveTowards
        float nuevaX = Mathf.MoveTowards(rb.position.x, xObjetivo, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(new Vector3(nuevaX, rb.position.y, rb.position.z));

        // Comprobamos si hemos llegado al objetivo
        if (Mathf.Abs(nuevaX - xObjetivo) < 0.01f)
        {
            rb.MovePosition(new Vector3(xObjetivo, rb.position.y, rb.position.z));
            moviendo = false;          // Terminamos el movimiento
            movimientosRealizados++;   // Incrementamos contador de pasos
            teleportsSinTurno = 0; // Termina el turno del jugador

            // Sincronizamos paso con la sombra (similar a SombraAcosadora.SincronizarPaso)
            TurnCoordinator.RegistrarPasoJugador(scriptSombra);
        }
    }
}
