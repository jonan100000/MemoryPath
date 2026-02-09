using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MenuNavigation : MonoBehaviour
{
    // UI: cargar una escena desde el menu
    public void CargarNivel(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
