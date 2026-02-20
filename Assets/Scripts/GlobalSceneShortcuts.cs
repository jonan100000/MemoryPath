using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GlobalSceneShortcuts : MonoBehaviour
{
    private const string MenuPrincipal = "MenuPrincipal";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarEscenaActual();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IrAlMenuPrincipal();
        }
    }

    public void ReiniciarEscenaActual()
    {
        Scene escena = SceneManager.GetActiveScene();
        if (escena.name == MenuPrincipal) return;

        Time.timeScale = 1f;
        SceneLoadService.LoadSceneByIndex(escena.buildIndex);
    }

    public void IrAlMenuPrincipal()
    {
        Scene escena = SceneManager.GetActiveScene();
        if (escena.name == MenuPrincipal) return;

        Time.timeScale = 1f;
        SceneLoadService.LoadSceneByName(MenuPrincipal);
    }
}
