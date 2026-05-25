using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMaximaE;
    public float vidaActualE;
 
    void Start()
    {
        vidaActualE = vidaMaximaE;
    }
 
    public void RecibirDanio(float cantidad)
    {
        vidaActualE -= cantidad;
        vidaActualE = Mathf.Clamp(vidaActualE, 0f, vidaMaximaE);

 
        if (vidaActualE <= 0f)
        {
            Morir();
        }
    }
    void Morir()
    {
        Destroy(gameObject);
    }
}
