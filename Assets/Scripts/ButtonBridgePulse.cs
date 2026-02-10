using UnityEngine;
using System.Collections.Generic;

public class ButtonBridgePulse : MonoBehaviour
{
    // Referencias
    public BridgeController puente; // Referencia unica (compatibilidad)
    public BridgeController[] puentes; // Puentes que este boton controla

    // Referencias visuales
    [Header("Referencias Visuales (Hijos)")]
    public GameObject visualActivado;   // Visual cuando el boton esta presionado
    public GameObject visualDesactivado; // Visual cuando el boton no esta presionado

    private int presiones = 0;
    private readonly Dictionary<BridgeController, bool> estadosOriginales =
        new Dictionary<BridgeController, bool>();

    void Start() => SetVisual(false); // Inicializamos el boton en estado "no presionado"

    // Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (!EsEntidadValida(other)) return;

        presiones++;
        if (presiones == 1)
        {
            CapturarEstados();
            AlternarPuentes();
            SetVisual(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!EsEntidadValida(other)) return;

        presiones = Mathf.Max(0, presiones - 1);
        if (presiones == 0)
        {
            RestaurarEstados();
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

    private void CapturarEstados()
    {
        estadosOriginales.Clear();
        foreach (var bridge in EnumerarPuentes())
        {
            if (bridge == null || estadosOriginales.ContainsKey(bridge)) continue;
            estadosOriginales[bridge] = bridge.activo;
        }
    }

    private void AlternarPuentes()
    {
        foreach (var kvp in estadosOriginales)
        {
            if (kvp.Key != null)
            {
                kvp.Key.SetActivo(!kvp.Value);
            }
        }
    }

    private void RestaurarEstados()
    {
        foreach (var kvp in estadosOriginales)
        {
            if (kvp.Key != null)
            {
                kvp.Key.SetActivo(kvp.Value);
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
