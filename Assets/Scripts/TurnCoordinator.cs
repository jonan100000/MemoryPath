using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public static class TurnCoordinator
{
    // Configuracion de teleports
    private const float teleportCooldownSeconds = 1f;
    private const float teleportCooldownStepSeconds = 0.1f;
    private const float teleportCooldownMinSeconds = 0.1f;
    private const float teleportExtraBlockSeconds = 0f;
    private static float teleportCooldownUntil = -1f;
    private static float teleportBlockUntil = -1f;

    // API publica: bloqueo unificado de input
    public static bool JugadorPuedeRecibirInput(bool estaEnSuelo, bool moviendo, SombraAcosadora sombra)
    {
        if (!estaEnSuelo || moviendo) return false;
        if (TeleportBloqueaMovimiento()) return false;
        return true;
    }

    // API publica: sincroniza paso con la sombra
    public static void RegistrarPasoJugador(SombraAcosadora sombra)
    {
        if (sombra != null) sombra.RegistrarPasoJugador();
    }

    // API publica: inicia ventana de bloqueo y cooldown
    public static void BloquearPorTeleport()
    {
        BloquearPorTeleport(0);
    }

    // API publica: cooldown dinamico por numero de teleports ya hechos en el turno
    public static void BloquearPorTeleport(int teleportsPreviosEnTurno)
    {
        float ahora = Time.time;
        float cooldownActual = Mathf.Max(
            teleportCooldownMinSeconds,
            teleportCooldownSeconds - (teleportCooldownStepSeconds * teleportsPreviosEnTurno)
        );

        float cooldownFin = ahora + cooldownActual;
        float bloqueoFin = ahora + cooldownActual + teleportExtraBlockSeconds;
        if (cooldownFin > teleportCooldownUntil) teleportCooldownUntil = cooldownFin;
        if (bloqueoFin > teleportBlockUntil) teleportBlockUntil = bloqueoFin;
    }

    // Estado: bloqueo activo para movimientos
    public static bool TeleportBloqueaMovimiento()
    {
        return Time.time <= teleportBlockUntil;
    }

    // Estado: disponible para teletransportar
    public static bool PuedeTeletransportar()
    {
        return Time.time >= teleportCooldownUntil;
    }
}

public static class SceneLoadService
{
    private sealed class SceneLoadRunner : MonoBehaviour { }

    private static bool isLoading;
    private static SceneLoadRunner runner;

    public static bool IsLoading() => isLoading;

    public static void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName) || isLoading) return;
        EnsureRunner();
        runner.StartCoroutine(LoadSceneRoutine(sceneName, -1));
    }

    public static void LoadSceneByIndex(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings || isLoading) return;
        EnsureRunner();
        runner.StartCoroutine(LoadSceneRoutine(null, buildIndex));
    }

    private static void EnsureRunner()
    {
        if (runner != null) return;
        var go = new GameObject("[SceneLoadService]");
        Object.DontDestroyOnLoad(go);
        runner = go.AddComponent<SceneLoadRunner>();
    }

    private static IEnumerator LoadSceneRoutine(string sceneName, int buildIndex)
    {
        isLoading = true;
        Time.timeScale = 1f;
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        yield return null;

        AsyncOperation unloadOperation = Resources.UnloadUnusedAssets();
        if (unloadOperation != null)
        {
            while (!unloadOperation.isDone) yield return null;
        }
        System.GC.Collect();
        yield return null;

        AsyncOperation operation = string.IsNullOrEmpty(sceneName)
            ? SceneManager.LoadSceneAsync(buildIndex)
            : SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            isLoading = false;
            yield break;
        }

        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f) yield return null;
        yield return null;
        operation.allowSceneActivation = true;
        while (!operation.isDone) yield return null;

        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        isLoading = false;
    }
}
