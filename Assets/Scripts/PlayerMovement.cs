using UnityEngine;
using System.Collections.Generic;

public class MovimientoPorBloques25D : MonoBehaviour
{
    public enum TipoMovimiento { Izquierda, Derecha, Escalera }

    public float tamañoBloque = 1f;
    public float velocidad = 10f;
    public float distanciaSuelo = 0.2f;
    public LayerMask capaSuelo;

    public SombraAcosadora scriptSombra;

    public int movimientosRealizados = 0;
    // Esta es la ÚNICA lista que necesitamos ahora
    public List<TipoMovimiento> historialComandos = new List<TipoMovimiento>();

    private Rigidbody rb;
    private float xObjetivo;
    private bool moviendo = false;
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

        // Manejo de físicas (Y)
        if (estaEnSuelo && !enEscalera && !enTeletransporte)
            CongelarY();
        else
            LiberarY();

        // LA ADUANA UNIFICADA
        // Definimos si la sombra está haciendo algo
        bool sombraOcupada = (scriptSombra != null && scriptSombra.EstaActiva() && scriptSombra.EstaOcupada());

        // Si el player se mueve, o está en el aire, o la sombra está trabajando: bloqueamos input.
        if (!estaEnSuelo || moviendo || sombraOcupada)
            return;

        // LECTURA DE INPUTS
        if (Input.GetKeyDown(KeyCode.A))
        {
            xObjetivo -= tamañoBloque;
            moviendo = true;
            historialComandos.Add(TipoMovimiento.Izquierda);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            xObjetivo += tamañoBloque;
            moviendo = true;
            historialComandos.Add(TipoMovimiento.Derecha);
        }
    }

    void FixedUpdate()
    {
        if (!moviendo) return;

        float nuevaX = Mathf.MoveTowards(rb.position.x, xObjetivo, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(new Vector3(nuevaX, rb.position.y, rb.position.z));

        if (Mathf.Abs(nuevaX - xObjetivo) < 0.01f)
        {
            rb.MovePosition(new Vector3(xObjetivo, rb.position.y, rb.position.z));
            moviendo = false;
            movimientosRealizados++;

            if (scriptSombra != null)
                scriptSombra.SincronizarPaso();
        }
    }

    void ComprobarSuelo()
    {
        estaEnSuelo = Physics.Raycast(transform.position, Vector3.down, distanciaSuelo, capaSuelo);
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
        if (scriptSombra != null) scriptSombra.SincronizarPaso();
    }
}