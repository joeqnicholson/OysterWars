using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Core
{

[AddComponentMenu("Kinematic2D/Core/Debug/Character Info")]
public class CharacterDebug_Info : MonoBehaviour
{

	CharacterMotor characterMotor;	

	void Awake ()
	{
		characterMotor = transform.root.GetComponentInChildren<CharacterMotor>();			
	}	
	

	void OnGUI()
	{		
		GUILayout.BeginVertical("Box" , GUILayout.Width(320));
		GUILayout.Label(characterMotor.ToString() );
		GUILayout.EndVertical();		
	
	}

	
}

}

