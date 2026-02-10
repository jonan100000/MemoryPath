using UnityEngine;
using UnityEngine.UI;

public sealed class InicioMensaje : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelMensaje;
    public Button botonCerrar;

    [Header("Opcional")]
    public bool mostrarCursor = true;
    public bool bloquearTiempo = true;

    private bool activo = false;

    void Start()
    {
        if (panelMensaje != null) panelMensaje.SetActive(true);
        if (botonCerrar != null) botonCerrar.onClick.AddListener(Cerrar);

        if (mostrarCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (bloquearTiempo)
        {
            Time.timeScale = 0f;
        }

        activo = true;
    }

    public void Cerrar()
    {
        if (!activo) return;

        if (panelMensaje != null) panelMensaje.SetActive(false);

        if (bloquearTiempo)
        {
            Time.timeScale = 1f;
        }

        if (mostrarCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        activo = false;
    }
}
