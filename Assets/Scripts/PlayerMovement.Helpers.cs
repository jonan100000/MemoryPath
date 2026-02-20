using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    // Helpers de suelo y fisicas
    // Comprueba si el jugador estÃ¡ sobre el suelo mediante raycast
    void ComprobarSuelo()
    {
        estaEnSuelo = Physics.Raycast(transform.position, Vector3.down, distanciaSuelo, capaSuelo);
    }

    // Congela el eje Y y rotaciones innecesarias para movimiento por bloques
    void CongelarY()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY 
                       | RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX 
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    // Libera la posiciÃ³n Y para permitir saltos o teletransportes
    void LiberarY()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX 
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    // Helpers de turno/input
    public bool PuedeRecibirInput()
    {
        return TurnCoordinator.JugadorPuedeRecibirInput(estaEnSuelo, moviendo, scriptSombra);
    }

    // MÃ©todo para teletransportar al jugador (por ejemplo, portal)
    // Teleport principal del jugador
    public void Teletransportar(Vector3 destino)
    {
        TurnCoordinator.BloquearPorTeleport(teleportsSinTurno);
        MarcarUsoEscaleraTrasTeleport();
        enTeletransporte = true; // Indicamos que estamos en teletransporte
        LiberarY();              // Permitimos que se mueva verticalmente
        moviendo = false;        // Cancelamos movimiento actual
        destino.z = rb.position.z; // El jugador nunca debe desplazarse en Z
        TeleportUtils.Teleportar(rb, transform, destino, true);
        ReproducirSonidoTeleport();
        xObjetivo = destino.x;   // Actualizamos objetivo X

        teleportsSinTurno++;
        if (teleportsSinTurno > maxTeleportsSinTurno)
        {
            MatarPorExcesoTeleports();
        }

        if (pasoPendiente)
        {
            CompletarPaso();
        }
    }

    // Evita que una escalera procese mas de un teleport seguido por frame/input retenido.
    public bool PuedeUsarEscaleraAhora()
    {
        if (TurnCoordinator.TeleportBloqueaMovimiento()) return false;
        if (ultimoFrameTeleportEscalera == Time.frameCount) return false;
        return Time.time >= bloqueoEscaleraHasta;
    }

    public void MarcarUsoEscaleraTrasTeleport(float bloqueoExtraSegundos = 0.08f)
    {
        ultimoFrameTeleportEscalera = Time.frameCount;
        float bloqueoObjetivo = Time.time + Mathf.Max(0f, bloqueoExtraSegundos);
        if (bloqueoObjetivo > bloqueoEscaleraHasta) bloqueoEscaleraHasta = bloqueoObjetivo;
    }

    // Helpers de turno
    private void CompletarPaso()
    {
        if (!pasoPendiente) return;
        movimientosRealizados++;
        teleportsSinTurno = 0;
        pasoPendiente = false;
        TurnCoordinator.RegistrarPasoJugador(scriptSombra);
    }

    // Registrar paso de escalera
    // Escalera: registra paso y sincroniza sombra
    public void RegistrarPasoDeEscalera()
    {
        movimientosRealizados++;                        // Incrementamos contador de movimientos
        historialComandos.Add(TipoMovimiento.Escalera); // Registramos en historial
        teleportsSinTurno = 0;                           // Termina el turno del jugador
        pasoPendiente = false;
        TurnCoordinator.RegistrarPasoJugador(scriptSombra);            // Sincronizamos paso con la sombra
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
            gameObject.SetActive(false);
        }
    }
}


