using UnityEngine;

public class Stair : MonoBehaviour
{
    // Referencias
    public Transform topPoint;    // Punto superior de la escalera
    public Transform bottomPoint; // Punto inferior de la escalera

    // Configuracion visual
    [Range(0f, 1f)]
    public float transparentAlpha = 0.4f; // Transparencia que se aplica al jugador cuando está en la escalera
    public bool usarTransparencia = false;

    // Estado interno
    private SpriteRenderer spriteRenderer; // SpriteRenderer del objeto escalera
    private PlayerMovement playerScript; // Referencia al script del jugador cuando entra en la escalera
    private SombraAcosadora sombraScript;         // Referencia al script de la sombra cuando entra en la escalera

    // Ciclo de vida Unity
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Input de escalera (solo jugador)
    void Update()
    {
        // Solo el PLAYER usa input directo para subir/bajar
        if (playerScript == null) return;
        if (!playerScript.PuedeRecibirInput()) return;

        // Calculamos distancia desde la base y desde la cima
        float distBottom = Mathf.Abs(playerScript.transform.position.y - bottomPoint.position.y);
        float distTop = Mathf.Abs(playerScript.transform.position.y - topPoint.position.y);

        // Si el jugador está más cerca de la base, puede subir
        if (distBottom < distTop)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                // Teletransportamos al jugador a la cima
                playerScript.Teletransportar(topPoint.position);
                // Registramos este movimiento como un "paso de escalera"
                playerScript.RegistrarPasoDeEscalera(); 
            }
        }
        else // Si está más cerca de la cima, puede bajar
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                TurnCoordinator.BloquearPorTeleport();
                playerScript.Teletransportar(bottomPoint.position);
                playerScript.RegistrarPasoDeEscalera();
            }
        }
    }

    // Método llamado por la sombra para teletransportarse por la escalera
    // Accion de sombra: teletransporta segun posicion
    public void EjecutarTeletransporteSombra()
    {
        if (sombraScript == null) return; // Si no hay sombra, salimos
        if (!TurnCoordinator.PuedeTeletransportar()) return;

        // Calculamos distancia desde la base y desde la cima
        float distBottom = Vector3.Distance(sombraScript.transform.position, bottomPoint.position);
        float distTop = Vector3.Distance(sombraScript.transform.position, topPoint.position);

        // Elegimos destino: si está abajo, sube; si está arriba, baja
        Vector3 destino = (distBottom < distTop) ? topPoint.position : bottomPoint.position;

        // Teletransportamos físicamente a la sombra
        sombraScript.Teletransportar(destino, false);
    }

    // Detecta cuando algo entra en el trigger de la escalera
    // Triggers
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerScript = other.GetComponent<PlayerMovement>();
            if (playerScript != null) playerScript.enEscalera = true; // Flag que indica que está en escalera
            SetTransparency(transparentAlpha); // Hacemos la escalera transparente para el jugador
        }

        // Guardamos referencia a la sombra si entra
        if (other.CompareTag("Sombra"))
        {
            sombraScript = other.GetComponent<SombraAcosadora>();
            if (sombraScript != null) sombraScript.SetStair(this);
        }
    }

    // Detecta cuando algo sale del trigger de la escalera
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerScript != null) playerScript.enEscalera = false; // Reset flag escalera
            playerScript = null;
            SetTransparency(1f); // Restauramos transparencia completa
        }

        if (other.CompareTag("Sombra"))
        {
            if (sombraScript != null) sombraScript.SetStair(null);
            sombraScript = null; // Quitamos referencia a la sombra
        }
    }

    // Cambia la transparencia del material de la escalera
    // Helpers visuales
    void SetTransparency(float alpha)
    {
        if (!usarTransparencia || spriteRenderer == null) return;
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}



