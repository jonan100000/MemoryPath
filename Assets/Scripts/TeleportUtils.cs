using UnityEngine;

public static class TeleportUtils
{
    // Utilidad central de teletransporte
    public static void Teleportar(Rigidbody rb, Transform t, Vector3 destino, bool resetVelocity)
    {
        if (t != null) t.position = destino;
        if (rb == null) return;

        rb.position = destino;
        if (resetVelocity) rb.velocity = Vector3.zero;
    }
}
