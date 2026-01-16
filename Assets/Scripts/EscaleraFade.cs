using UnityEngine;

public class EscaleraFadeSprite : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alphaTransparente = 0.3f; // Alpha cuando el jugador está detrás

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Este script necesita un SpriteRenderer en el mismo GameObject");
            enabled = false;
            return;
        }
        colorOriginal = spriteRenderer.color; // Guardamos el color original
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetAlpha(alphaTransparente);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetAlpha(colorOriginal.a);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}
