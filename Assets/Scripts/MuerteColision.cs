using UnityEngine;
using UnityEngine.SceneManagement;

public class MuerteColision : MonoBehaviour
{
    // Para objetos SÓLIDOS (donde el jugador rebota o choca)
    private void OnCollisionEnter(Collision collision)
    {
        ManejarMuerte(collision.gameObject);
    }

    // Para objetos FANTASMA (sensores, zonas de vacío con "Is Trigger")
    private void OnTriggerEnter(Collider other)
    {
        ManejarMuerte(other.gameObject);
    }

    // Función centralizada para no repetir código
    private void ManejarMuerte(GameObject objetoQueToca)
    {
        if (objetoQueToca.CompareTag("Player"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (objetoQueToca.CompareTag("Sombra"))
        {
            // Buscamos el script en la sombra y la matamos
            SombraAcosadora scriptSombra = objetoQueToca.GetComponent<SombraAcosadora>();
            if (scriptSombra != null)
            {
                scriptSombra.Morir();
            }
        }
    }
}