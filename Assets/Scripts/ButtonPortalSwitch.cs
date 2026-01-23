using UnityEngine;

public class ButtonPortalSwitch : MonoBehaviour
{
    public TeleportPortal[] portales;
    public GameObject prefabActivado;
    public GameObject prefabDesactivado;
    public float alturaOffset = 0f;

    private GameObject visualActual;

    void Start()
    {
        ActualizarVisual();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovimientoPorBloques25D>() == null) return;

        foreach (var portal in portales)
        {
            if (!portal.activo)
            {
                portal.ActivarPortal();
            }
        }

        ActualizarVisual();
    }

    public void PonerDesactivado()
    {
        ActualizarVisual();
    }

    private void ActualizarVisual()
    {
        GameObject prefab = TodosPortalesActivos() ? prefabActivado : prefabDesactivado;

        if (visualActual != null)
            Destroy(visualActual);

        if (prefab != null)
        {
            Vector3 pos = transform.position;
            pos.y += alturaOffset;
            visualActual = Instantiate(prefab, pos, transform.rotation, transform);
        }
    }

    private bool TodosPortalesActivos()
    {
        foreach (var portal in portales)
        {
            if (!portal.activo) return false;
        }
        return true;
    }
}
