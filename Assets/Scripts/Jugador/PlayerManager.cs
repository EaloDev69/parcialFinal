using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMaxima;
    public float vidaActual;
    public delegate void VidaCambiadaDelegate(float vidaActual, float vidaMaxima);
    public static event VidaCambiadaDelegate OnVidaCambiada;
 
    void Start()
    {
        vidaActual = vidaMaxima;
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima); 
    }
 
    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
        OnVidaCambiada?.Invoke(vidaActual, vidaMaxima);
 
        if (vidaActual <= 0f)
        {
            Morir();
        }
    }
    void Morir()
    {
        Debug.Log("Mori");
    }

}
