using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // Singleton
    private static TurnManager _instance;

    // Propiedad p�blica para acceder a la instancia
    public static TurnManager Instance
    {
        get
        {
            // Si no tenemos referencia...
            if (_instance == null)
            {
                // 1. Buscamos si ya existe en la escena por si acaso
                _instance = FindObjectOfType<TurnManager>();

                // 2. Si sigue sin existir, lo creamos nosotros
                if (_instance == null)
                {
                    GameObject obj = new GameObject("TurnManager_Auto");
                    _instance = obj.AddComponent<TurnManager>();
                }
            }
            return _instance;
        }
    }

    // Variable para ver en el inspector (debug)
    // Estado de turnos
    [SerializeField] private int entidadesMoviendose = 0;

    // Ciclo de vida Unity
    void Awake()
    {
        // Control b�sico de duplicados
        if (_instance == null)
        {
            _instance = this;
            // Opcional: Si quieres que persista entre cambios de escena
            // DontDestroyOnLoad(gameObject); 
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // API publica: registro de movimientos
    public void RegistrarInicioMovimiento()
    {
        entidadesMoviendose++;
    }

    public void RegistrarFinMovimiento()
    {
        entidadesMoviendose--;
        // Seguridad por si bajamos de 0
        if (entidadesMoviendose < 0) entidadesMoviendose = 0;
    }

    // Estado: sin entidades moviendose
    public bool EsTurnoLibre()
    {
        return entidadesMoviendose == 0;
    }
}
