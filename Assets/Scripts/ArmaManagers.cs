using UnityEngine;

public class ArmaManagers : MonoBehaviour
{
    public GameObject[] Arma;
    public int ArmaDefault;

    void Start()
    {
        foreach (GameObject arma in Arma)
        {
            arma.SetActive(false);
        }
        ActiveArmaDefault(ArmaDefault);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivarSolos(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivarSolos(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivarSolos(2);
        
        if(Arma.Length <= 0)
        {
            Debug.Log("No hay armas carajo mierda >:C");
        }
        
    }
    void ActivarSolos(int index)
    {
        for (int i = 0; i < Arma.Length; i++)
        Arma[i].SetActive(i == index);
        Debug.Log(index);
    }
    void ActiveArmaDefault(int index)
    {
        for (int i = ArmaDefault; i < Arma.Length; i++)
        Arma[i].SetActive(i == index);
    }
}
