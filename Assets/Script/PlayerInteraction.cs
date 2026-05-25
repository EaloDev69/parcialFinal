using UnityEngine;
using TMPro;

// Coloca este script en el GameObject del jugador.
// Asigna en el Inspector el TextMeshProUGUI que creaste en el Canvas.
public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public float interactionRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    [Tooltip("aquí el TextMeshPro del Canvas")]
    public TextMeshProUGUI interactText;

    private InteractableObject currentTarget;

    private void Start()
    {
        if (interactText == null)
            Debug.Log("[PlayerInteraction] Asigna el InteractText en el Inspector.");
        else
            interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        DetectInteractable();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
            currentTarget.Interact();
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                if (currentTarget != interactable)
                {
                    currentTarget = interactable;

                    if (interactText != null)
                    {
                        interactText.text = string.IsNullOrEmpty(currentTarget.promptText)
                            ? "Presiona E para interactuar"
                            : currentTarget.promptText;
                        interactText.gameObject.SetActive(true);
                    }

                    Debug.Log($"[PlayerInteraction] Mirando: {currentTarget.gameObject.name}");
                }
                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget = null;
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * interactionRange);
    }
}