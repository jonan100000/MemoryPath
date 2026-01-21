using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    public Transform destino;      // el otro portal
    public float transparentAlpha = 0.4f;
    public GameObject objetoTransformacion; // Nuevo: objeto en el que se transformará

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
            
            // Transformar este portal
            TransformarPortal(this.gameObject);
            
            // Transformar el portal destino
            TransformarPortal(destino.gameObject);
        }
    }

    void TransformarPortal(GameObject portal)
    {
        // Evitar transformar múltiples veces si ya está transformado
        TeleportPortal scriptPortal = portal.GetComponent<TeleportPortal>();
        if (scriptPortal == null || portal.activeSelf == false) return;

        // Si hay un objeto de transformación asignado
        if (objetoTransformacion != null)
        {
            // Crear el nuevo objeto en la misma posición y rotación
            GameObject nuevoObjeto = Instantiate(
                objetoTransformacion, 
                portal.transform.position, 
                portal.transform.rotation
            );
        }
        
        // Desactivar el portal original
        portal.SetActive(false);
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