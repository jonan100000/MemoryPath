using UnityEngine;

public class MovimientoPorBloques25D : MonoBehaviour
{
    public float tamañoBloque = 1f;
    public float velocidad = 10f;

    public float distanciaSuelo = 0.2f;
    public LayerMask capaSuelo;

    private Rigidbody rb;
    private float xObjetivo;
    private bool moviendo = false;
    private bool estaEnSuelo;

    [HideInInspector] public bool enEscalera = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        xObjetivo = rb.position.x;

        // Congelamos Y al empezar
        CongelarY();
    }

    void Update()
    {
        ComprobarSuelo();

        // 🔁 Cambiamos constraints según estado
        if (estaEnSuelo && !enEscalera)
            CongelarY();
        else
            LiberarY();


        // ❌ No moverse si está en el aire
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

    // 🔒 Congela Y
    void CongelarY()
    {
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    // 🔓 Libera Y (para caer)
    void LiberarY()
    {
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    // 🔁 Teletransporte
    public void Teletransportar(Vector3 destino)
    {
        // 1️⃣ Liberar Y antes de teletransportar
        LiberarY();

        // 2️⃣ Teletransportar
        moviendo = false;
        rb.velocity = Vector3.zero;
        rb.position = destino;
        xObjetivo = destino.x;

        // 3️⃣ Comprobar si está sobre suelo y volver a congelar
        if (estaEnSuelo && !enEscalera)
            CongelarY();
    }

}
