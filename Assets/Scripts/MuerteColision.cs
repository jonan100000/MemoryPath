using UnityEngine;

public class MuerteColision : MonoBehaviour
{
    // Colisiones fisicas
    // Para objetos SOLIDOS (donde el jugador rebota o choca)
    private void OnCollisionEnter(Collision collision)
    {
        ManejarMuerte(collision.gameObject);
    }

    // Triggers
    // Para objetos FANTASMA (sensores, zonas de vacio con "Is Trigger")
    private void OnTriggerEnter(Collider other)
    {
        ManejarMuerte(other.gameObject);
    }

    // Logica comun de muerte
    private void ManejarMuerte(GameObject objetoQueToca)
    {
        if (!EsEntidadMortal(objetoQueToca)) return;

        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogWarning("GameManager not found; death ignored.");
            return;
        }

        gm.EntidadMuere(objetoQueToca);
    }

    private bool EsEntidadMortal(GameObject objeto)
    {
        return objeto.CompareTag("Player") || objeto.CompareTag("Sombra");
    }
}
