using UnityEngine;

public class ButtonOneShot : MonoBehaviour
{
    private ButtonVisualSwitch visual;

    void Awake()
    {
        visual = GetComponent<ButtonVisualSwitch>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovimientoPorBloques25D>() != null)
        {
            visual.ActivarVisual();
        }
    }
}
