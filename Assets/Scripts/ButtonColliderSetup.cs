using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ButtonColliderSetup : MonoBehaviour
{
    void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }
}

