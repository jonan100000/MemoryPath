using UnityEngine;

public class SombraAcosadora : MonoBehaviour
{
    public MovimientoPorBloques25D scriptJugador;
    public int movimientosParaActivar = 5;
    public float velocidadSombra = 5f;

    private Rigidbody rb;
    private bool activa = false;
    private bool muerta = false;

    private int indicePasosSombra = 0;
    private int pasosPermitidos = 0;
    private int pasosMaximos = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        rb.isKinematic = true;
    }

    public bool EstaActiva() => activa && !muerta;

    public void SincronizarPaso()
    {
        if (activa && !muerta)
        {
            pasosPermitidos++;
        }
    }

    void Update()
    {
        if (!activa && scriptJugador.movimientosRealizados >= movimientosParaActivar)
        {
            DespertarSombra();
        }
    }

    void FixedUpdate()
    {
        if (!activa || muerta || pasosPermitidos <= 0 || indicePasosSombra >= pasosMaximos) 
            return;

        Vector3 objetivo = scriptJugador.historialPosiciones[indicePasosSombra];

        float distanciaX = Mathf.Abs(transform.position.x - objetivo.x);
        float distanciaY = Mathf.Abs(transform.position.y - objetivo.y);

        // CASO A: Escalera (Teletransporte vertical)
        // Si hay mucha diferencia de altura y poca en X, saltamos directamente
        if (distanciaY > 0.5f && distanciaX < 0.1f)
        {
            rb.position = objetivo;
            FinalizarMovimiento(objetivo);
        }
        // CASO B: Movimiento horizontal normal
        else
        {
            float nuevaX = Mathf.MoveTowards(transform.position.x, objetivo.x, velocidadSombra * Time.fixedDeltaTime);
            
            // Movemos en X pero mantenemos la Y del objetivo por si hay pequeños desniveles
            rb.MovePosition(new Vector3(nuevaX, objetivo.y, transform.position.z));

            if (Mathf.Abs(nuevaX - objetivo.x) < 0.01f)
            {
                FinalizarMovimiento(objetivo);
            }
        }
    }

    private void FinalizarMovimiento(Vector3 posicionFinal)
    {
        rb.MovePosition(posicionFinal);
        indicePasosSombra++;
        pasosPermitidos--;
    }

    void DespertarSombra()
    {
        activa = true;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        indicePasosSombra = 0;
        pasosMaximos = scriptJugador.movimientosRealizados;
        pasosPermitidos = 0;
    }

    public void Morir()
    {
        muerta = true;
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}