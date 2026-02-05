using UnityEngine;

public class SombraAcosadora : MonoBehaviour
{
    public MovimientoPorBloques25D scriptJugador;
    public int movimientosParaActivar = 5;
    public float velocidadSombra = 5f;

    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private Collider shadowCollider;
    private bool activa = false;
    private bool muerta = false;

    private float xObjetivoPropio;
    private bool ejecutandoAccion = false;

    private int indicePasosSombra = 0;
    private int pasosPermitidos = 0;
    private int pasosMaximos = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        shadowCollider = GetComponent<Collider>();

        // Estado inicial: invisible e intangible
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (shadowCollider != null) shadowCollider.enabled = false;
        rb.isKinematic = true;
    }

    public bool EstaActiva() => activa && !muerta;

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
        // Despertar cuando el jugador alcance el límite de movimientos inicial
        if (!activa && scriptJugador.movimientosRealizados >= movimientosParaActivar)
        {
            DespertarSombra();
        }
    }

    void FixedUpdate()
    {
        if (!PuedeProcesarPaso())
            return;

        // Leemos el comando grabado (Izquierda, Derecha o Escalera)
        // Usamos el enum definido en el script del Player
        MovimientoPorBloques25D.TipoMovimiento comando = scriptJugador.historialComandos[indicePasosSombra];

        if (comando == MovimientoPorBloques25D.TipoMovimiento.Escalera)
        {
            // Buscamos si estamos dentro de un trigger de escalera
            // Podemos usar un OverlapSphere pequeño para encontrar el script de la escalera
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

            FinalizarTurno();
        }
        else
        {
            // Lógica de movimiento por bloques independiente
            if (!ejecutandoAccion)
            {
                float direccion = (comando == MovimientoPorBloques25D.TipoMovimiento.Derecha) ? 1f : -1f;
                xObjetivoPropio = rb.position.x + (direccion * scriptJugador.tamañoBloque);
                ejecutandoAccion = true;
            }

            float nuevaX = Mathf.MoveTowards(rb.position.x, xObjetivoPropio, velocidadSombra * Time.fixedDeltaTime);

            // Mantenemos su propia Y (gravedad) mientras se mueve en X
            rb.MovePosition(new Vector3(nuevaX, rb.position.y, rb.position.z));

            if (Mathf.Abs(nuevaX - xObjetivoPropio) < 0.01f)
            {
                rb.MovePosition(new Vector3(xObjetivoPropio, rb.position.y, rb.position.z));
                ejecutandoAccion = false;
                FinalizarTurno();
            }
        }
    }

    bool PuedeProcesarPaso()
    {
        return activa && !muerta && pasosPermitidos > 0 && indicePasosSombra < pasosMaximos;
    }

    void FinalizarTurno()
    {
        indicePasosSombra++;
        pasosPermitidos--;
        ejecutandoAccion = false;
    }

    void DespertarSombra()
    {
        activa = true;
        if (meshRenderer != null) meshRenderer.enabled = true;
        if (shadowCollider != null) shadowCollider.enabled = true;

        rb.isKinematic = false;
        // Congelamos rotaciones y eje Z para el 2.5D
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        indicePasosSombra = 0;
        pasosMaximos = scriptJugador.movimientosRealizados;
        pasosPermitidos = 0;
    }

    // Método restaurado para colisiones con trampas, etc.
    public void Morir()
    {
        muerta = true;
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void ResetearMovimiento()
    {
        // Al poner esto en false, obligamos a la sombra a que en el próximo 
        // FixedUpdate recalcule su 'xObjetivoPropio' desde su nueva posición X.
        ejecutandoAccion = false;
    }

    public void CompletarPasoPorTeleport()
    {
        ejecutandoAccion = false;
        // Consumimos el "ticket" del movimiento que nos metió al portal
        // para que no intente repetirlo al salir.
        indicePasosSombra++;
        pasosPermitidos--;
    }

    // Método de utilidad para el Player si necesita bloquear inputs
    public bool TienePasosPendientes() => pasosPermitidos > 0;

    // Dentro de SombraAcosadora.cs
    public bool EstaOcupada()
    {
        // Está ocupada si: está ejecutando un paso lateral O si su velocidad vertical es significativa (cayendo)
        bool cayendo = Mathf.Abs(rb.velocity.y) > 0.1f;
        return ejecutandoAccion || cayendo || TienePasosPendientes();
    }
}
