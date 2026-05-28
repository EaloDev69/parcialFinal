using UnityEngine;

public class Pausa : MonoBehaviour
{
    public bool enPausa = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && !enPausa)
        {
            Time.timeScale = 0;
            enPausa = true;
        }
        if (Input.GetKey(KeyCode.Escape) && enPausa)
        {
            Time.timeScale = 1;
            enPausa = false;
        }
    }
}
