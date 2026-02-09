using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    [Header("Configuraci�n de Muerte")]
    // Configuracion
    public float retrasoReiniciar = 1.5f;
    public GameObject efectoMuertePrefab; // Opcional: para part�culas

    // Ciclo de vida Unity
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // API publica: punto unico de muerte
    public void EntidadMuere(GameObject entidad)
    {
        Debug.Log(entidad.name + " ha muerto.");

        // 1. Efectos visuales comunes
        if (efectoMuertePrefab != null)
            Instantiate(efectoMuertePrefab, entidad.transform.position, Quaternion.identity);

        // 2. L�gica espec�fica seg�n qui�n sea
        if (entidad.CompareTag("Player"))
        {
            ProcesarMuerteJugador(entidad);
        }
        else if (entidad.GetComponent<SombraAcosadora>() != null)
        {
            ProcesarMuerteSombra(entidad);
        }
    }

    // Logica de muerte por tipo
    private void ProcesarMuerteJugador(GameObject player)
    {
        // Desactivamos el movimiento para que no siga recibiendo input
        player.SetActive(false);

        // Reiniciamos el nivel tras un tiempo
        Invoke("ReiniciarNivel", retrasoReiniciar);
    }

    private void ProcesarMuerteSombra(GameObject sombra)
    {
        // La sombra simplemente desaparece
        sombra.SetActive(false);
        // Aqu� podr�as a�adir l�gica extra: �da puntos? �suelta un objeto?
    }

    // Accion final: reinicia la escena
    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
