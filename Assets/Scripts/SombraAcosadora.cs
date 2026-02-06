using UnityEngine;

public class SombraAcosadora : MonoBehaviour
{
    public MovimientoPorBloques25D scriptJugador; // Referencia al script del jugador para leer movimientos
    public int movimientosParaActivar = 5;       // Número de pasos que el jugador debe realizar para despertar a la sombra
    public float velocidadSombra = 5f;           // Velocidad a la que se mueve la sombra

    private Rigidbody rb;                        // Rigidbody de la sombra
    private bool activa = false;                 // Indica si la sombra está despierta
    private bool muerta = false;                 // Indica si la sombra ha sido eliminada

    private float xObjetivoPropio;              // Posición X objetivo para moverse por bloques
    private bool ejecutandoAccion = false;       // Controla si la sombra está en medio de un movimiento

    private int indicePasosSombra = 0;          // Índice actual en el historial de movimientos del jugador
    private int pasosPermitidos = 0;            // Tickets de movimiento disponibles para sincronizar con el jugador
    private int pasosMaximos = 0;               // Límite de pasos que puede realizar la sombra

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Estado inicial: invisible e intangible
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        rb.isKinematic = true;
    }

    public bool EstaActiva() => activa && !muerta; // Método útil para otros scripts (similar a playerOcupado en MovimientoPorBloques)

    // Se llama desde el Player para darle "tickets" de movimiento
    public void SincronizarPaso()
    {
        if (activa && !muerta)
        {
            pasosPermitidos++;
        }
    }

    void Update()
    {
        // Despertar la sombra cuando el jugador alcance los movimientos iniciales
        if (!activa && scriptJugador.movimientosRealizados >= movimientosParaActivar)
        {
            DespertarSombra();
        }
    }

    void FixedUpdate()
    {
        // Condiciones para no ejecutar movimientos
        if (!activa || muerta || pasosPermitidos <= 0 || indicePasosSombra >= pasosMaximos)
            return;

        // Leemos el comando grabado del historial del jugador
        MovimientoPorBloques25D.TipoMovimiento comando = scriptJugador.historialComandos[indicePasosSombra];

        if (comando == MovimientoPorBloques25D.TipoMovimiento.Escalera)
        {
            // Buscamos triggers de escalera cercanos para ejecutar teletransporte
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (var hit in hitColliders)
            {
                Stair escalera = hit.GetComponent<Stair>();
                if (escalera != null)
                {
                    escalera.EjecutarTeletransporteSombra();
                    break;
                }
            }

            FinalizarTurno(); // Similar a liberar pasos del jugador
        }
        else
        {
            // Movimiento horizontal por bloques
            if (!ejecutandoAccion)
            {
                float direccion = (comando == MovimientoPorBloques25D.TipoMovimiento.Derecha) ? 1f : -1f;
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

    void FinalizarTurno()
    {
        indicePasosSombra++;
        pasosPermitidos--;
        ejecutandoAccion = false; // Clave para recalcular en el próximo FixedUpdate
    }

    void DespertarSombra()
    {
        activa = true;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ; // Congelar Z y rotación para 2.5D

        indicePasosSombra = 0;
        pasosMaximos = scriptJugador.movimientosRealizados; // Sombra reproduce todos los pasos que hizo el jugador hasta despertar
        pasosPermitidos = 0;
    }

    // Método para colisiones con trampas o muerte directa
    public void Morir()
    {
        muerta = true;
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void ResetearMovimiento()
    {
        ejecutandoAccion = false; // Fuerza a recalcular objetivo X en FixedUpdate
    }

    public void CompletarPasoPorTeleport()
    {
        ejecutandoAccion = false;
        indicePasosSombra++; // Consumimos el paso
        pasosPermitidos--;
    }

    public bool TienePasosPendientes() => pasosPermitidos > 0; // Similar a comprobar si el jugador puede moverse

    public bool EstaOcupada()
    {
        // Ocupada si está moviéndose o cayendo
        bool cayendo = Mathf.Abs(rb.velocity.y) > 0.1f;
        return ejecutandoAccion || cayendo || TienePasosPendientes();
    }
}
