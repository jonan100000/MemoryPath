using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    [Header("Configuracion de Muerte")]
    public float retrasoReiniciar = 4f;
    public AudioClip sonidoMuerte;
    public AudioSource audioSource;

    // Ciclo de vida Unity
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    // API publica: punto unico de muerte
    public void EntidadMuere(GameObject entidad)
    {
        if (entidad == null) return;

        Debug.Log(entidad.name + " ha muerto.");

        // 1. Efecto de sonido comun
        if (sonidoMuerte != null)
        {
            Vector3 pos = entidad.transform.position;
            if (audioSource != null) audioSource.PlayOneShot(sonidoMuerte);
            else AudioSource.PlayClipAtPoint(sonidoMuerte, pos);
        }

        // 2. Logica especifica segun quien sea
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
        var deathAnim = player.GetComponent<DeathAnimation>();
        if (deathAnim != null)
        {
            deathAnim.PlayDeath();
        }
        else
        {
            player.SetActive(false);
        }

        // Reiniciamos el nivel tras un tiempo
        Invoke("ReiniciarNivel", retrasoReiniciar);
    }

    private void ProcesarMuerteSombra(GameObject sombra)
    {
        var deathAnim = sombra.GetComponent<DeathAnimation>();
        if (deathAnim != null)
        {
            deathAnim.PlayDeath();
        }
        else
        {
            sombra.SetActive(false);
        }
        // Aqui podrias anadir logica extra: da puntos? suelta un objeto?
    }

    // Accion final: reinicia la escena
    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
