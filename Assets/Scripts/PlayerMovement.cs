using UnityEngine;
using System.Collections.Generic;

public class MovimientoPorBloques25D : MonoBehaviour
{
    public float tamañoBloque = 1f;
    public float velocidad = 10f;

    public float distanciaSuelo = 0.2f;
    public LayerMask capaSuelo;

    public SombraAcosadora scriptSombra;

    public int movimientosRealizados = 0;
    public List<Vector3> historialPosiciones = new List<Vector3>();

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

        historialPosiciones.Add(transform.position);
        CongelarY();
    }

    void Update()
    {
        ComprobarSuelo();

        if (estaEnSuelo && !enEscalera && !enTeletransporte)
            CongelarY();
        else
            LiberarY();

        if (!estaEnSuelo || moviendo)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            xObjetivo -= tamañoBloque;
            moviendo = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            xObjetivo += tamañoBloque;
            moviendo = true;
        }
    }

    void FixedUpdate()
    {
        if (!moviendo) return;

        float nuevaX = Mathf.MoveTowards(
            rb.position.x,
            xObjetivo,
            velocidad * Time.fixedDeltaTime
        );

        rb.MovePosition(new Vector3(nuevaX, rb.position.y, rb.position.z));

        if (Mathf.Abs(nuevaX - xObjetivo) < 0.01f)
        {
            rb.MovePosition(new Vector3(xObjetivo, rb.position.y, rb.position.z));
            moviendo = false;

            movimientosRealizados++;
            historialPosiciones.Add(new Vector3(xObjetivo, rb.position.y, rb.position.z));

            // 🔔 AVISO A LA SOMBRA CUANDO EL PASO TERMINA
            if (scriptSombra != null && scriptSombra.EstaActiva())
            {
                scriptSombra.SincronizarPaso();
            }
        }
    }

    void ComprobarSuelo()
    {
        estaEnSuelo = Physics.Raycast(
            transform.position,
            Vector3.down,
            distanciaSuelo,
            capaSuelo
        );
    }

    void CongelarY()
    {
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    void LiberarY()
    {
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
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
        // Solo si la sombra aún no ha nacido, aumentamos su "memoria" futura
        if (!scriptSombra.EstaActiva()) {
            // No hacemos nada, porque DespertarSombra se encargará de leer movimientosRealizados
        }
        
        movimientosRealizados++;
        historialPosiciones.Add(transform.position);

        if (scriptSombra != null && scriptSombra.EstaActiva())
        {
            scriptSombra.SincronizarPaso();
        }
    }
}
