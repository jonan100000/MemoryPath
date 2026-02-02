using UnityEngine;

public class TeleportPortal : MonoBehaviour
{
    public Transform destino;

    [Header("Configuración de Prefabs")]
    public GameObject prefabAbierto;
    public GameObject prefabCerrado;

    public bool activo = true;
    public ButtonPortalSwitch[] botonesAsociados;

    private GameObject instanciaAbierta;
    private GameObject instanciaCerrada;
    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();

        // Creamos las instancias una sola vez al inicio y las hacemos hijas
        if (prefabAbierto != null)
        {
            instanciaAbierta = Instantiate(prefabAbierto, transform.position, transform.rotation, transform);
        }

        if (prefabCerrado != null)
        {
            instanciaCerrada = Instantiate(prefabCerrado, transform.position, transform.rotation, transform);
            // Aplicamos la escala del prefab que vimos en tu foto (0.2, 1, 1) o la que traiga el prefab
            instanciaCerrada.transform.localScale = prefabCerrado.transform.localScale;
        }
    }

    void Start()
    {
        ActualizarEstado();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!activo) return;

        if (other.TryGetComponent<MovimientoPorBloques25D>(out var player))
        {
            if (destino != null)
            {
                player.Teletransportar(destino.position);
                DesactivarPortal();

                if (destino.TryGetComponent<TeleportPortal>(out var otro))
                    otro.DesactivarPortal();
            }
        }
    }

    public void ActivarPortal()
    {
        activo = true;
        ActualizarEstado();
    }

    public void DesactivarPortal()
    {
        activo = false;
        ActualizarEstado();

        foreach (var boton in botonesAsociados)
            if (boton != null) boton.PonerDesactivado();
    }

    private void ActualizarEstado()
    {
        if (col != null) col.enabled = activo;

        // Simplemente encendemos/apagamos las instancias
        if (instanciaAbierta != null) instanciaAbierta.SetActive(activo);
        if (instanciaCerrada != null) instanciaCerrada.SetActive(!activo);
    }
}