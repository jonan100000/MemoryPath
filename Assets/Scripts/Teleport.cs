using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    public Transform destino;      // el otro portal
    public float transparentAlpha = 0.4f;

    private MeshRenderer mr;
    private Material mat;
    private MovimientoPorBloques25D player;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;
    }

    void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<MovimientoPorBloques25D>();
        if (player == null) return;

        // Opcional: hacer transparente mientras está en el portal
        Color c = mat.color;
        c.a = transparentAlpha;
        mat.color = c;

        // Teletransportar inmediatamente
        if (destino != null)
        {
            player.Teletransportar(destino.position);
        }
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
