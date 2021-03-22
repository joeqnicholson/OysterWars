#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Lightbug.Kinematic2D.Core
{

[ CustomEditor( typeof(CharacterMotor) , true ) , CanEditMultipleObjects ]
public class CharacterMotorEditor : Editor
{
	CharacterMotor monobehaviour;


	void OnEnable()
	{
		monobehaviour = target as CharacterMotor;
	}

		

}

}

#endif
