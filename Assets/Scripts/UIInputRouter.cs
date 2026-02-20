using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class UIInputRouter : MonoBehaviour
{
    [Header("Escenas")]
    public string menuPrincipalSceneName = "MenuPrincipal";

    private PlayerMovement jugador;

    // Direccionales tactiles
    public void MoverIzquierda()
    {
        PlayerMovement p = GetJugador();
        if (p != null) p.BotonIzquierda();
    }

    public void MoverDerecha()
    {
        PlayerMovement p = GetJugador();
        if (p != null) p.BotonDerecha();
    }

    public void MoverArriba()
    {
        PlayerMovement p = GetJugador();
        if (p != null) p.BotonArriba();
    }

    public void MoverAbajo()
    {
        PlayerMovement p = GetJugador();
        if (p != null) p.BotonAbajo();
    }

    // Navegacion UI
    public void ReiniciarNivel()
    {
        Scene escena = SceneManager.GetActiveScene();
        if (escena.name == menuPrincipalSceneName) return;

        Time.timeScale = 1f;
        SceneLoadService.LoadSceneByIndex(escena.buildIndex);
    }

    public void IrMenuPrincipal()
    {
        Scene escena = SceneManager.GetActiveScene();
        if (escena.name == menuPrincipalSceneName) return;

        Time.timeScale = 1f;
        SceneLoadService.LoadSceneByName(menuPrincipalSceneName);
    }

    public void SiguienteNivel()
    {
        Time.timeScale = 1f;
        int actual = SceneManager.GetActiveScene().buildIndex;
        int siguiente = actual + 1;
        if (siguiente < SceneManager.sceneCountInBuildSettings)
        {
            SceneLoadService.LoadSceneByIndex(siguiente);
        }
        else
        {
            SceneLoadService.LoadSceneByName(menuPrincipalSceneName);
        }
    }

    public void CerrarJuego()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private PlayerMovement GetJugador()
    {
        if (jugador != null && jugador.isActiveAndEnabled) return jugador;
        jugador = FindObjectOfType<PlayerMovement>();
        return jugador;
    }
}
