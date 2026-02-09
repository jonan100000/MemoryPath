using UnityEngine;

public static class TurnCoordinator
{
    // Configuracion de teleports
    private const float teleportCooldownSeconds = 0.2f;
    private const float teleportExtraBlockSeconds = 0.2f;
    private static float teleportCooldownUntil = -0.2f;
    private static float teleportBlockUntil = -0.2f;

    // API publica: bloqueo unificado de input
    public static bool JugadorPuedeRecibirInput(bool estaEnSuelo, bool moviendo, SombraAcosadora sombra)
    {
        if (!estaEnSuelo || moviendo) return false;
        if (TeleportBloqueaMovimiento()) return false;
        return sombra == null || !sombra.BloqueaJugador();
    }

    // API publica: sincroniza paso con la sombra
    public static void RegistrarPasoJugador(SombraAcosadora sombra)
    {
        if (sombra != null) sombra.RegistrarPasoJugador();
    }

    // API publica: inicia ventana de bloqueo y cooldown
    public static void BloquearPorTeleport()
    {
        float ahora = Time.time;
        float cooldownFin = ahora + teleportCooldownSeconds;
        float bloqueoFin = ahora + teleportCooldownSeconds + teleportExtraBlockSeconds;
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
