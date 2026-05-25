using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Configuración de interacción")]
    [Tooltip("Texto que se muestra al jugador cuando está cerca")]
    public string promptText = "Presiona E para interactuar";
    [Tooltip("Si está activo, el objeto puede ser usado más de una vez")]
    public bool canUseMultipleTimes = true;

    private bool hasBeenUsed = false;
    
    // Llamado por PlayerInteraction cuando el jugador presiona la tecla de interacción.
  
    public void Interact()
    {
        if (!canUseMultipleTimes && hasBeenUsed)
        {
            Debug.Log($"[{gameObject.name}] Ya fue usado.");
            return;
        }

        hasBeenUsed = true;
        OnInteract();
    }
    
    // Aquí va la lógica del objeto al ser interactuado.
    // Reemplaza el Debug.Log cuando implementes el sistema de vida.
    
    private void OnInteract()
    {
        Debug.Log("recupera vida");
    }
}
