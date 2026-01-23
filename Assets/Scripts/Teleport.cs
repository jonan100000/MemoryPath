using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    public Transform destino;

    [Header("Prefabs visuales")]
    public GameObject portalAbiertoPrefab;
    public GameObject portalCerradoPrefab;

    [HideInInspector] public bool activo = true;

    private GameObject visualActual;

    public ButtonPortalSwitch[] botonesAsociados; // Para notificar cambios

    private MeshRenderer mr;
    private Collider col;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        InstanciarVisual(portalAbiertoPrefab);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!activo) return;

        MovimientoPorBloques25D player = other.GetComponent<MovimientoPorBloques25D>();
        if (player == null) return;

        if (destino != null)
            player.Teletransportar(destino.position);

        DesactivarPortal();
        if (destino.TryGetComponent<TeleportPortal>(out TeleportPortal otro))
            otro.DesactivarPortal();
    }

    public void DesactivarPortal()
    {
        activo = false;
        CambiarVisual(portalCerradoPrefab);

        // Notificar a todos los botones que los portales están inactivos
        foreach (var boton in botonesAsociados)
        {
            if (boton != null)
                boton.PonerDesactivado();
        }
    }

    public void ActivarPortal()
    {
        activo = true;
        CambiarVisual(portalAbiertoPrefab);
    }

    private void CambiarVisual(GameObject prefab)
    {
        // Hacer invisible el objeto original
        if (mr != null) mr.enabled = false;
        if (col != null) col.enabled = false;

        // Destruye visual anterior
        if (visualActual != null) Destroy(visualActual);

        // Instancia nuevo prefab visual
        if (prefab != null)
            InstanciarVisual(prefab);
    }

    private void InstanciarVisual(GameObject prefab)
    {
        visualActual = Instantiate(prefab, transform.position, transform.rotation, transform);
    }
}
