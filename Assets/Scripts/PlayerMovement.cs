using System.Collections.Generic;
using UnityEngine;

public class MovimientoPorBloques25D : MonoBehaviour
{
    public enum TipoMovimiento { Izquierda, Derecha, Escalera }

    public float tamañoBloque = 1f;
    public float velocidad = 10f;
    public float distanciaSuelo = 0.2f;
    public LayerMask capaSuelo;
    public SombraAcosadora scriptSombra;
    public int movimientosRealizados = 0;
    public List<TipoMovimiento> historialComandos = new List<TipoMovimiento>();

    private Rigidbody rb;
    private float xObjetivo;
    private bool moviendo;
    private bool estaEnSuelo;

    [HideInInspector] public bool enEscalera = false;
    [HideInInspector] public bool enTeletransporte = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        xObjetivo = rb.position.x;
        CongelarY();
    }

    void Update()
    {
        ComprobarSuelo();
        ActualizarRestriccionesY();

        if (BloquearInput())
        {
            return;
        }

        LeerInputMovimiento();
    }

    void FixedUpdate()
    {
        if (!moviendo)
        {
            return;
        }

        float nuevaX = Mathf.MoveTowards(rb.position.x, xObjetivo, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(new Vector3(nuevaX, rb.position.y, rb.position.z));

        if (Mathf.Abs(nuevaX - xObjetivo) < 0.01f)
        {
            rb.MovePosition(new Vector3(xObjetivo, rb.position.y, rb.position.z));
            moviendo = false;
            movimientosRealizados++;

            if (scriptSombra != null)
            {
                scriptSombra.SincronizarPaso();
            }
        }
    }

    void ComprobarSuelo()
    {
        estaEnSuelo = Physics.Raycast(transform.position, Vector3.down, distanciaSuelo, capaSuelo);
    }

    void ActualizarRestriccionesY()
    {
        if (estaEnSuelo && !enEscalera && !enTeletransporte)
        {
            CongelarY();
        }
        else
        {
            LiberarY();
        }
    }

    bool BloquearInput()
    {
        bool sombraOcupada = scriptSombra != null && scriptSombra.EstaActiva() && scriptSombra.EstaOcupada();
        return !estaEnSuelo || moviendo || sombraOcupada;
    }

    void LeerInputMovimiento()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RegistrarMovimientoLateral(-tamañoBloque, TipoMovimiento.Izquierda);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            RegistrarMovimientoLateral(tamañoBloque, TipoMovimiento.Derecha);
        }
    }

    void RegistrarMovimientoLateral(float deltaX, TipoMovimiento movimiento)
    {
        xObjetivo += deltaX;
        moviendo = true;
        historialComandos.Add(movimiento);
    }

    void CongelarY()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void LiberarY()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Teletransportar(Vector3 destino)
    {
        enTeletransporte = true;
        LiberarY();
        moviendo = false;
        rb.velocity = Vector3.zero;
        rb.position = destino;
        xObjetivo = destino.x;
    }

    public void RegistrarPasoDeEscalera()
    {
        movimientosRealizados++;
        historialComandos.Add(TipoMovimiento.Escalera);
        if (scriptSombra != null)
        {
            scriptSombra.SincronizarPaso();
        }
    }
}
