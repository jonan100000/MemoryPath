using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ControladorMeta : MonoBehaviour
{
    public GameObject panelVictoria; // Panel que se activa cuando el jugador alcanza la meta

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mostrar el panel de victoria
            panelVictoria.SetActive(true);

            // CONGELAR EL JUEGO
            Time.timeScale = 0f;

            // Liberar el cursor para poder interactuar con la UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void IrAlMenu()
    {
        // IMPORTANTE: Resetear el tiempo antes de cambiar de escena
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void SiguienteNivel()
    {
        // Despausar el juego
        Time.timeScale = 1f;

        int escenaActual = SceneManager.GetActiveScene().buildIndex;

        // Si la siguiente escena existe en la lista de Build Settings...
        if (escenaActual + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(escenaActual + 1);
        }
        else
        {
            // Si es el último nivel, vuelve al menú principal
            SceneManager.LoadScene("MenuPrincipal");
        }
    }
}
