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
                       | RigidbodyConstraints.FreezeRotationX 
                       | RigidbodyConstraints.FreezeRotationZ;
    }

    // Libera la posiciÃ³n Y para permitir saltos o teletransportes
    void LiberarY()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX 
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
        enTeletransporte = true; // Indicamos que estamos en teletransporte
        LiberarY();              // Permitimos que se mueva verticalmente
        moviendo = false;        // Cancelamos movimiento actual
        TeleportUtils.Teleportar(rb, transform, destino, true);
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


