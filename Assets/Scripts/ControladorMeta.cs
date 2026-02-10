using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public sealed class ControladorMeta : MonoBehaviour
{
    // Referencias UI
    public GameObject panelVictoria; // Panel que se activa cuando el jugador alcanza la meta
    public GameObject animVictoria; // Animacion que aparece antes del menu
    public float retrasoVictoria = 3.5f;
    public AudioClip sonidoMeta;
    public AudioClip sonidoMetaSecundario;
    public AudioSource audioSource;
    public AudioSource audioSourceSecundario;
    public AudioSource audioFondo;
    public bool mantenerFondoEntreReinicios = true;
    public bool detenerFondoAlGanar = true;

    private bool metaActivada = false;
    private static AudioSource audioFondoPersistente;

    void Start()
    {
        if (animVictoria != null) animVictoria.SetActive(false);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSourceSecundario == null)
        {
            var sources = GetComponents<AudioSource>();
            if (sources.Length > 1) audioSourceSecundario = sources[1];
        }
        PrepararAudioFondo();
    }

    private void PrepararAudioFondo()
    {
        if (!mantenerFondoEntreReinicios || audioFondo == null) return;

        if (audioFondoPersistente != null && audioFondoPersistente != audioFondo)
        {
            Destroy(audioFondo.gameObject);
            audioFondo = audioFondoPersistente;
            return;
        }

        if (audioFondoPersistente == null)
        {
            audioFondoPersistente = audioFondo;
            if (audioFondo.transform.parent != null) audioFondo.transform.SetParent(null);
            DontDestroyOnLoad(audioFondo.gameObject);
        }
    }

    // Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (metaActivada) return;
        if (!other.CompareTag("Player")) return;

        metaActivada = true;
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        var movimiento = other.GetComponent<PlayerMovement>();
        if (movimiento != null) movimiento.enabled = false;

        StartCoroutine(MostrarVictoria());
    }

    private IEnumerator MostrarVictoria()
    {
        if (detenerFondoAlGanar && audioFondo != null) audioFondo.Stop();

        if (sonidoMeta != null)
        {
            if (audioSource != null) audioSource.PlayOneShot(sonidoMeta);
            else AudioSource.PlayClipAtPoint(sonidoMeta, transform.position);
        }
        if (sonidoMetaSecundario != null)
        {
            if (audioSourceSecundario != null) audioSourceSecundario.PlayOneShot(sonidoMetaSecundario);
            else if (audioSource != null) audioSource.PlayOneShot(sonidoMetaSecundario);
            else AudioSource.PlayClipAtPoint(sonidoMetaSecundario, transform.position);
        }

        if (animVictoria != null) animVictoria.SetActive(true);

        yield return new WaitForSeconds(retrasoVictoria);

        if (panelVictoria != null) panelVictoria.SetActive(true);

        // Congelar el juego
        Time.timeScale = 0f;

        // Liberar el cursor para poder interactuar con la UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // UI: volver al menu principal
    public void IrAlMenu()
    {
        // IMPORTANTE: Resetear el tiempo antes de cambiar de escena
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    // UI: cargar el siguiente nivel o volver al menu
    public void SiguienteNivel()
    {
        // Despausar el juego
        Time.timeScale = 1f;

        int escenaActual = SceneManager.GetActiveScene().buildIndex;

        // Si la siguiente escena existe en la lista de Build Settings...
        if (escenaActual + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(escenaActual + 1);
        }
        else
        {
            // Si es el ultimo nivel, vuelve al menu principal
            SceneManager.LoadScene("MenuPrincipal");
        }
    }
}
