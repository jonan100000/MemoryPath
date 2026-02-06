using UnityEngine;
using UnityEngine.SceneManagement;

public class MuerteColision : MonoBehaviour
{
    // Para objetos SÓLIDOS (donde el jugador rebota o choca)
    private void OnCollisionEnter(Collision collision)
    {
        // Llamamos a la función que maneja la muerte del jugador o de la sombra
        ManejarMuerte(collision.gameObject);
    }

    // Para objetos FANTASMA (sensores, zonas de vacío con "Is Trigger")
    private void OnTriggerEnter(Collider other)
    {
        // Llamamos a la misma función para no duplicar lógica
        ManejarMuerte(other.gameObject);
    }

    // Función centralizada para no repetir código
    private void ManejarMuerte(GameObject objetoQueToca)
    {
        if (objetoQueToca.CompareTag("Player"))
        {
            // Reiniciamos la escena actual si choca el jugador
            Time.timeScale = 1f; // Asegura que el tiempo esté normal antes de reiniciar
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (objetoQueToca.CompareTag("Sombra"))
        {
            // Buscamos el script en la sombra y llamamos a su método de morir
            SombraAcosadora scriptSombra = objetoQueToca.GetComponent<SombraAcosadora>();
            if (scriptSombra != null)
            {
                scriptSombra.Morir();
            }
            // Comentario: si no tiene el script, no hace nada. Podría agregarse un log de advertencia.
        }
    }
}
