#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Lightbug.Kinematic2D.Implementation
{

[CustomEditor(typeof(CharacterAbility) , true ) , CanEditMultipleObjects]
public class CharacterAbilityEditor : Editor
{
	GUIStyle titleStyle = new GUIStyle();

	CharacterAbility monobehaviour;

	void OnEnable()
	{
		titleStyle.alignment = TextAnchor.MiddleCenter;
		titleStyle.normal.textColor = Color.black;
		titleStyle.fontSize = 16;

		monobehaviour = target as CharacterAbility;
	}
	

	public override void OnInspectorGUI()
	{		
		GUILayout.Space( 10 );
		//GUILayout.BeginVertical( (GUIStyle)"IN ThumbnailShadow" );

		GUILayout.Space( 5 );

		string info = monobehaviour.GetInfo();
		if( info != "" )
			GUILayout.Label( info , EditorStyles.helpBox );
		
		DrawDefaultInspector();

		GUILayout.Space( 5 );
		
		//GUILayout.EndVertical();

		GUILayout.Space( 10 );
	}
}

}

#endif
