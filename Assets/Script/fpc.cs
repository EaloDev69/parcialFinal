using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class fpc : MonoBehaviour
{
    public CharacterController controlador;
    public float veloMovi = 2f;
    public float gravedad = -9.81f;
    public float salto = 4f;
    public float Correr = 3f;
    public Transform checkPiso;
    public float distanciaPiso = 0.4f;
    public LayerMask Piso;

    bool enPiso;

    Vector3 velocidad;

    // Update is called once per frame
    void Update()
    {
     enPiso = Physics.CheckSphere(checkPiso.position, distanciaPiso, Piso);
     
     if(enPiso && velocidad.y < 0)
     {
         velocidad.y = -2f;
     }
     
     float x = Input.GetAxis("Horizontal");
     float z = Input.GetAxis("Vertical");
     
        Vector3 movimiento = transform.right * x + transform.forward * z;
        controlador.Move(movimiento * veloMovi * Time.deltaTime);
        if(Input.GetButtonDown("Jump") && enPiso)
        {
            velocidad.y = Mathf.Sqrt(salto * -2f * gravedad);
        }

        if (Input.GetButtonDown("Run") && enPiso)
        {
            veloMovi = veloMovi*Correr;
        }
        if (Input.GetButtonUp("Run") && enPiso)
        {
            veloMovi = veloMovi/Correr;
        }

        velocidad.y += gravedad * Time.deltaTime;
        controlador.Move(velocidad * Time.deltaTime);
    }
}
