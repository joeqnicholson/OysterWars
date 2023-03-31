
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public class FPSCounter : MonoBehaviour
{
    public int avgFrameRate;
    public TextMeshProUGUI text;
 
    public void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public void Update ()
    {
        Application.targetFrameRate = -1;
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;
        text.text = avgFrameRate.ToString() + " FPS";
    }
}