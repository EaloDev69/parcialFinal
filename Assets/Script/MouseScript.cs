using UnityEngine;

public class MouseScript : MonoBehaviour
{
    public float mouseSensitivity = 10f;

    public Transform cuerpoJugador;

    float rotaX = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        rotaX -= mouseY;
        
        rotaX = Mathf.Clamp(rotaX, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(rotaX, 0f, 0f);
        cuerpoJugador.rotation *= Quaternion.Euler(0f, mouseX, 0f);
    }
}
