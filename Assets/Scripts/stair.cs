using UnityEngine;

public class Stair : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;

    [Range(0f, 1f)]
    public float transparentAlpha = 0.4f;

    private MeshRenderer mr;
    private Material mat;
    private MovimientoPorBloques25D player;

    private float epsilon = 0.05f; // margen pequeño para comparar posiciones


    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.W) && player.transform.position.y < topPoint.position.y - epsilon)
        {
            // Solo subir si no está ya en TopPoint
            player.Teletransportar(topPoint.position);
        }

        if (Input.GetKeyDown(KeyCode.S) && player.transform.position.y > bottomPoint.position.y + epsilon)
        {
            // Solo bajar si no está ya en BottomPoint
            player.Teletransportar(bottomPoint.position);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<MovimientoPorBloques25D>();
        if (player == null) return;

        player.enEscalera = true; // ✅ marcar que está en escalera

        Color c = mat.color;
        c.a = transparentAlpha;
        mat.color = c;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovimientoPorBloques25D>() == null) return;

        player.enEscalera = false; // ✅ sale de escalera
        player = null;

        Color c = mat.color;
        c.a = 1f;
        mat.color = c;
    }

}

