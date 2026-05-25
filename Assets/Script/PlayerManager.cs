using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float salud;

    public void getDamage(float dmg)
    {
        salud -= dmg;

        if (salud <= 0)
        {
            Destroy(gameObject);
            Debug.Log("moriste");
        }
    }
}
