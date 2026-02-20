using UnityEngine;
using System.Collections.Generic;

public class ButtonBridgePulse : MonoBehaviour
{
    private sealed class EstadoGlobalPuente
    {
        public bool estadoOriginal;
        public int botonesActivos;
    }

    private static readonly Dictionary<BridgeController, EstadoGlobalPuente> estadosGlobales =
        new Dictionary<BridgeController, EstadoGlobalPuente>();

    // Referencias
    public BridgeController puente; // Referencia unica (compatibilidad)
    public BridgeController[] puentes; // Puentes que este boton controla

    // Referencias visuales
    [Header("Referencias Visuales (Hijos)")]
    public GameObject visualActivado;   // Visual cuando el boton esta presionado
    public GameObject visualDesactivado; // Visual cuando el boton no esta presionado

    private int presiones = 0;

    void Start() => SetVisual(false); // Inicializamos el boton en estado "no presionado"

    // Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (!EsEntidadValida(other)) return;

        presiones++;
        if (presiones == 1)
        {
            ActivarPulsoGlobal();
            SetVisual(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!EsEntidadValida(other)) return;

        presiones = Mathf.Max(0, presiones - 1);
        if (presiones == 0)
        {
            LiberarPulsoGlobal();
            SetVisual(false);
        }
    }

    // Metodo de ayuda para detectar si la entidad es valida (Jugador o Sombra)
    // Helpers de validacion
    private bool EsEntidadValida(Collider col)
    {
        return col.GetComponent<PlayerMovement>() != null ||
               col.GetComponent<SombraAcosadora>() != null;
    }

    // Logica global: varios botones sobre el mismo puente no deben alternarlo dos veces.
    private void ActivarPulsoGlobal()
    {
        foreach (var bridge in EnumerarPuentes())
        {
            if (bridge == null) continue;

            if (!estadosGlobales.TryGetValue(bridge, out var estado))
            {
                estado = new EstadoGlobalPuente
                {
                    estadoOriginal = bridge.activo,
                    botonesActivos = 0
                };
                estadosGlobales[bridge] = estado;
            }

            estado.botonesActivos++;
            bridge.SetActivo(!estado.estadoOriginal);
        }
    }

    private void LiberarPulsoGlobal()
    {
        foreach (var bridge in EnumerarPuentes())
        {
            if (bridge == null) continue;
            if (!estadosGlobales.TryGetValue(bridge, out var estado)) continue;

            estado.botonesActivos = Mathf.Max(0, estado.botonesActivos - 1);
            if (estado.botonesActivos == 0)
            {
                bridge.SetActivo(estado.estadoOriginal);
                estadosGlobales.Remove(bridge);
            }
            else
            {
                bridge.SetActivo(!estado.estadoOriginal);
            }
        }
    }

    private IEnumerable<BridgeController> EnumerarPuentes()
    {
        if (puentes != null && puentes.Length > 0)
        {
            foreach (var bridge in puentes) yield return bridge;
            yield break;
        }

        if (puente != null) yield return puente;
    }

    // Cambia la visual del boton segun este presionado o no
    // Helpers visuales
    private void SetVisual(bool presionado)
    {
        if (visualActivado != null) visualActivado.SetActive(presionado);
        if (visualDesactivado != null) visualDesactivado.SetActive(!presionado);
    }
}
