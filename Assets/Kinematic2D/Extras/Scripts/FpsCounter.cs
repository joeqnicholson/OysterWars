using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lightbug.Kinematic2D.Extras
{


public class FpsCounter : MonoBehaviour 
{
	
	public float time = 1;
    public Text text;
	float result = 0;
    int samples = 0;

    string output = "FPS : ";

    float fps = 60;
    public float Fps
    {
        get
        {
            return fps;
        }
    }

    GUIStyle style = new GUIStyle();


    void Awake()
    {
        style.fontSize = 20;
        style.normal.textColor = Color.white;        
    }
	
	void Update () 
    {
                
		if (time > 0)
        {            
			result += 1 / Time.unscaledDeltaTime;
            samples++;
            time -= Time.unscaledDeltaTime;
        }
        else
        {
        	fps = result / samples;
            output = "FPS : " + fps.ToString();
           
            if(text != null)                
                text.text = output;
            
            

			result = 0;
            samples = 0;
            time = 1;
        }        

    }

    void OnGUI()
    {
        GUILayout.BeginVertical("Box");
        GUILayout.Label(output , style , GUILayout.Width(Screen.width));
        GUILayout.EndVertical();
    }
}

}
