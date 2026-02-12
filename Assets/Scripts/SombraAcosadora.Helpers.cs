using UnityEngine;

public partial class SombraAcosadora : MonoBehaviour
{
    // Deja la sombra en estado no bloqueante antes de deshabilitar scripts/objeto.
    public void PrepararMuerte()
    {
        muerta = true;
        ejecutandoAccion = false;
        pasosPermitidos = 0;
        teleportsSinTurno = 0;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    // API publica: bloqueo del jugador
    public bool BloqueaJugador()
    {
        return EstaActiva() && EstaOcupada();
    }

    // API publica: paso sincronizado
    public void RegistrarPasoJugador()
    {
        SincronizarPaso();
    }

    // Teleport de la sombra (opcionalmente consume paso)
    public void Teletransportar(Vector3 destino, bool consumirPaso = false)
    {
        TeleportUtils.Teleportar(rb, transform, destino, true);
        teleportsSinTurno++;
        if (teleportsSinTurno > maxTeleportsSinTurno)
        {
            MatarPorExcesoTeleports();
            return;
        }

        if (consumirPaso) CompletarPasoPorTeleport();
    }

    // Método para colisiones con trampas o muerte directa
    // API publica: muerte directa
    public void Morir()
    {
        PrepararMuerte();
        gameObject.SetActive(false);
    }

    // Helpers de turno
    private void CompletarPasoPorTeleport()
    {
        ejecutandoAccion = false;
        indicePasosSombra++; // Consumimos el paso
        pasosPermitidos--;
        if (pasosPermitidos < 0) pasosPermitidos = 0;
    }

    // Seguridad: evita loops de teleports
    private void MatarPorExcesoTeleports()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            gm.EntidadMuere(gameObject);
        }
        else
        {
            Morir();
        }
    }

    // Estado de pasos pendientes
    public bool TienePasosPendientes() => pasosPermitidos > 0; // Similar a comprobar si el jugador puede moverse

    // Ocupacion: se usa para bloquear input del jugador
    public bool EstaOcupada()
    {
        if (!activa || muerta || HaTerminadoReproduccion()) return false;
        if (TurnCoordinator.TeleportBloqueaMovimiento()) return true;
        // Ocupada si está moviéndose o cayendo
        bool cayendo = Mathf.Abs(rb.velocity.y) > 0.1f;
        return ejecutandoAccion || cayendo || TienePasosPendientes();
    }
}
