using UnityEngine;

public class Stair : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;

    [Range(0f, 1f)]
    public float transparentAlpha = 0.4f;

    private MeshRenderer mr;
    private Material mat;
    private MovimientoPorBloques25D playerScript;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;
    }

    void Update()
    {
        // Solo el PLAYER real usa el Input para subir/bajar
        if (playerScript == null) return;

        float distBottom = Mathf.Abs(playerScript.transform.position.y - bottomPoint.position.y);
        float distTop = Mathf.Abs(playerScript.transform.position.y - topPoint.position.y);

        if (distBottom < distTop)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                playerScript.Teletransportar(topPoint.position);
                // Notificamos que esto ha sido un movimiento de turno
                playerScript.RegistrarPasoDeEscalera();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                playerScript.Teletransportar(bottomPoint.position);
                // Notificamos que esto ha sido un movimiento de turno
                playerScript.RegistrarPasoDeEscalera();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Si entra el Jugador
        if (other.CompareTag("Player"))
        {
            playerScript = other.GetComponent<MovimientoPorBloques25D>();
            if (playerScript != null) playerScript.enEscalera = true;
            SetTransparency(transparentAlpha);
        }
        
        // Si entra la Sombra, también avisamos que está en escalera 
        // para que sus constraints de Rigidbody cambien (LiberarY)
        if (other.CompareTag("Sombra"))
        {
            var sombraMov = other.GetComponent<MovimientoPorBloques25D>(); // Si hereda del mismo script
            if (sombraMov != null) sombraMov.enEscalera = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerScript != null) playerScript.enEscalera = false;
            playerScript = null;
            SetTransparency(1f);
        }

        if (other.CompareTag("Sombra"))
        {
            var sombraMov = other.GetComponent<MovimientoPorBloques25D>();
            if (sombraMov != null) sombraMov.enEscalera = false;
        }
    }

    void SetTransparency(float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;
    }
}