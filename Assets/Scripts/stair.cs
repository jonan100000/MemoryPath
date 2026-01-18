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

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            player.Teletransportar(topPoint.position);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            player.Teletransportar(bottomPoint.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<MovimientoPorBloques25D>();
        if (player == null) return;

        Color c = mat.color;
        c.a = transparentAlpha;
        mat.color = c;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MovimientoPorBloques25D>() == null) return;

        player = null;

        Color c = mat.color;
        c.a = 1f;
        mat.color = c;
    }
}

