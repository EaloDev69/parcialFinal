using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalasHud : MonoBehaviour
{
    [Header ("Referencias UI")]
    public Text TextoBalas;
    public Text TextoCargadores;
    [Header("Vida")]
    public Slider barraVida;

    void OnEnable()
    {
        Prefab.OnAmmoChanged += ActualizarHUD;
        BalaComun.OnAmmoChanged += ActualizarHUDRay;
        PlayerManager.OnVidaCambiada += ActualizarVida;
        Escopeta.OnAmmoChanged += ActualizarHUD;
    }

    void OnDisable()
    {
        Prefab.OnAmmoChanged -= ActualizarHUD;
        BalaComun.OnAmmoChanged -= ActualizarHUDRay;
        PlayerManager.OnVidaCambiada -= ActualizarVida;
        Escopeta.OnAmmoChanged -= ActualizarHUD;
    }
    void ActualizarHUD(int balas, int maxBalas, int cargadores, bool recargando)
    {
        TextoBalas.text  = recargando ? "Recargando..." : $"{balas}/{maxBalas}";
        TextoCargadores.text =$"x {cargadores}";
    }
    void ActualizarHUDRay(int balas, int maxBalas, int cargadores, bool recargando)
    {
        TextoBalas.text = recargando ? "Recargando..." : $"{balas}/{maxBalas}";
        TextoCargadores.text =$"x {cargadores}";
    }
    void ActualizarVida(float vidaActual, float vidaMaxima)
    {
        barraVida.value = vidaActual / vidaMaxima; // siempre entre 0 y 1
    }
}
