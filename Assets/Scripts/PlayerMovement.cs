using UnityEngine;

public class MovimientoPorBloques25D : MonoBehaviour
{
    public float tamañoBloque = 1f;
    public float velocidad = 10f;

    private Rigidbody rb;
    private float xObjetivo;
    private bool moviendo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        xObjetivo = rb.position.x;
    }

    void Update()
    {
        if (!moviendo)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                xObjetivo -= tamañoBloque;
                moviendo = true;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                xObjetivo += tamañoBloque;
                moviendo = true;
            }
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

    

}
