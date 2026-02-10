using System.Collections;
using UnityEngine;

public class DeathAnimation : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string deathTrigger = "Die";
    public GameObject deathVisual;

    [Header("Disable Options")]
    public bool disableMovementScripts = true;
    public bool disableColliders = true;
    public bool disableRigidbody = true;
    public bool disableGameObject = true;
    public bool disableSpriteRenderer = false;
    public float disableDelay = 1f;

    private bool hasPlayed = false;

    void Awake()
    {
        if (animator == null)
        {
            if (deathVisual != null)
            {
                animator = deathVisual.GetComponent<Animator>();
            }

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(true);
            }
        }
    }

    public void PlayDeath()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        if (animator != null && !string.IsNullOrEmpty(deathTrigger))
        {
            animator.SetTrigger(deathTrigger);
        }
        else
        {
            var legacy = deathVisual != null
                ? deathVisual.GetComponent<Animation>()
                : GetComponentInChildren<Animation>(true);
            if (legacy != null) legacy.Play();
        }

        if (deathVisual != null) deathVisual.SetActive(true);

        if (disableMovementScripts)
        {
            var player = GetComponent<PlayerMovement>();
            if (player != null) player.enabled = false;
            var sombra = GetComponent<SombraAcosadora>();
            if (sombra != null) sombra.enabled = false;
        }

        if (disableColliders)
        {
            foreach (var col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
        }

        if (disableRigidbody)
        {
            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        if (disableGameObject)
        {
            StartCoroutine(DisableAfterDelay());
        }
        else if (disableSpriteRenderer)
        {
            StartCoroutine(DisableSpriteAfterDelay());
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        if (disableDelay > 0f) yield return new WaitForSeconds(disableDelay);
        gameObject.SetActive(false);
    }

    private IEnumerator DisableSpriteAfterDelay()
    {
        if (disableDelay > 0f) yield return new WaitForSeconds(disableDelay);
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;
    }
}
