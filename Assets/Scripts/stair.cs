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
    private SombraAcosadora sombraScript;

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

    public void EjecutarTeletransporteSombra()
    {
        if (sombraScript == null) return;

        // Detectamos si está más cerca de la base o de la cima
        float distBottom = Vector3.Distance(sombraScript.transform.position, bottomPoint.position);
        float distTop = Vector3.Distance(sombraScript.transform.position, topPoint.position);

        // Si está abajo, la mandamos arriba. Si está arriba, abajo.
        Vector3 destino = (distBottom < distTop) ? topPoint.position : bottomPoint.position;

        // Teletransporte físico inmediato
        sombraScript.transform.position = destino;
        Rigidbody rbS = sombraScript.GetComponent<Rigidbody>();
        if (rbS != null) rbS.position = destino;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerScript = other.GetComponent<MovimientoPorBloques25D>();
            if (playerScript != null) playerScript.enEscalera = true;
            SetTransparency(transparentAlpha);
        }

        // Guardamos la referencia de la sombra cuando entra
        if (other.CompareTag("Sombra"))
        {
            sombraScript = other.GetComponent<SombraAcosadora>();
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
            sombraScript = null;
        }
    }

    void SetTransparency(float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;
    }
}