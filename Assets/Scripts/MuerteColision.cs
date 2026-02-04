using UnityEngine;
using UnityEngine.SceneManagement;

public class MuerteColisíon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si lo que entró en el cubo es el Jugador
        if (other.CompareTag("Player"))
        {
            // Reinicia la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            // Importante por si morimos después de haber ganado
            Time.timeScale = 1f;
        }
    }
}
