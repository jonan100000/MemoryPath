using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
    // UI: cargar una escena desde el menu
    public void CargarNivel(string nombreEscena)
    {
        SceneLoadService.LoadSceneByName(nombreEscena);
    }

    // UI: cerrar el juego
    public void CerrarJuego()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
