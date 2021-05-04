using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretFade : MonoBehaviour
{
    private Material material;
    [SerializeField] private bool startFade;
    [SerializeField] private float fadeLevel = 1;
    [SerializeField] private float fadeSpeed = 1;
    void Start()
    {
        SpriteRenderer spriteTest = GetComponent<SpriteRenderer>();
        TilemapRenderer tileTest = GetComponent<TilemapRenderer>();


        if (tileTest)
        {
            material = tileTest.material;
        }

        if (spriteTest)
        {
            material = spriteTest.material;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (startFade)
        {
            fadeLevel = Mathf.Lerp(fadeLevel, -.3f, fadeSpeed * Time.deltaTime);
            material.SetFloat("_Fade", fadeLevel);

            if(fadeLevel < -0.1f)
            {
                Destroy(gameObject);
            }

        }

        
    }

    public void StartFade()
    {
        startFade = true;
    }
}
