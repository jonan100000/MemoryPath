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
    private PlayerMovement jugador;
    private SombraAcosadora sombra;
    private bool muertePorSolapamientoProcesada = false;
    private bool colisionesIgnoradas = false;

    // Ciclo de vida Unity
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (muertePorSolapamientoProcesada) return;

        CachearEntidades();
        if (jugador == null || sombra == null) return;
        if (!sombra.EstaActiva()) return;

        IgnorarColisionEntreJugadorYSombra();

        if (!jugador.PuedeRecibirInput()) return;
        if (sombra.EstaOcupada()) return;

        if (EstanSolapados(jugador, sombra))
        {
            muertePorSolapamientoProcesada = true;
            EntidadMuere(jugador.gameObject);
            EntidadMuere(sombra.gameObject);
        }
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
        var sombraScript = sombra.GetComponent<SombraAcosadora>();
        if (sombraScript != null)
        {
            sombraScript.PrepararMuerte();
        }

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

    private void CachearEntidades()
    {
        if (jugador == null) jugador = FindObjectOfType<PlayerMovement>();
        if (sombra == null) sombra = FindObjectOfType<SombraAcosadora>();
    }

    private void IgnorarColisionEntreJugadorYSombra()
    {
        if (colisionesIgnoradas) return;

        Collider colPlayer = jugador != null ? jugador.GetComponent<Collider>() : null;
        Collider colShadow = sombra != null ? sombra.GetComponent<Collider>() : null;
        if (colPlayer == null || colShadow == null) return;

        Physics.IgnoreCollision(colPlayer, colShadow, true);
        colisionesIgnoradas = true;
    }

    private bool EstanSolapados(PlayerMovement player, SombraAcosadora shadow)
    {
        Collider colPlayer = player.GetComponent<Collider>();
        Collider colShadow = shadow.GetComponent<Collider>();
        if (colPlayer != null && colShadow != null)
        {
            return colPlayer.bounds.Intersects(colShadow.bounds);
        }

        float distancia = Vector3.Distance(player.transform.position, shadow.transform.position);
        return distancia < 0.1f;
    }
}
