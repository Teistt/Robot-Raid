using UnityEngine;

public class BlinkShader : MonoBehaviour
{
    Material material;


    public float timeON = 0.5f;
    public float blinkTime = 1f;
    private float elapsedtime = 0f;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        if (elapsedtime > blinkTime)
        {
            elapsedtime = 0;
        }
        if (elapsedtime > timeON)
        {
            material.SetInt("_Blinked", 0);
        }
        else
        {
            material.SetInt("_Blinked", 1);
        }
        elapsedtime += Time.deltaTime;
    }
}
