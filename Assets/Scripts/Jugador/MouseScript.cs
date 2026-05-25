using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseScript : MonoBehaviour
{
    public float mouseSens = 10f;
    public Transform cuerpoJugador;

    float rotaX = 0f;//rota camara 
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //no mas mouse en pantalla 
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        rotaX -= mouseY;

        rotaX = Mathf.Clamp(rotaX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotaX, 0f, 0f);

        cuerpoJugador.rotation *= Quaternion.Euler(0f,mouseX, 0f);
    }
}
