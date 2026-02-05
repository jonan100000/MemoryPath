using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MenuNavigation : MonoBehaviour
{
    public void CargarNivel(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
