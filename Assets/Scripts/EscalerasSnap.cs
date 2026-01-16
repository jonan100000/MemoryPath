using UnityEngine;

public class EscalerasSnap : MonoBehaviour
{
    private Rigidbody rb;
    private Transform puntoArribaEscalera;
    private bool enEscalera = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (enEscalera && Input.GetKeyDown(KeyCode.W) || 
            enEscalera && Input.GetKeyDown(KeyCode.UpArrow))
        {
            SubirEscalera();
        }
    }

    void SubirEscalera()
    {
        // Cancelar velocidad antes de mover
        rb.velocity = Vector3.zero;

        Vector3 nuevaPosicion = new Vector3(
            puntoArribaEscalera.position.x,
            puntoArribaEscalera.position.y,
            rb.position.z
        );

        rb.MovePosition(nuevaPosicion);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Escalera"))
        {
            enEscalera = true;
            puntoArribaEscalera = other.transform.Find("PuntoArriba");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Escalera"))
        {
            enEscalera = false;
            puntoArribaEscalera = null;
        }
    }
}
