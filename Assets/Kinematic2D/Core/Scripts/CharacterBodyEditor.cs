#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Lightbug.Kinematic2D.Core
{

[ CustomEditor(typeof(CharacterBody) , true ) , CanEditMultipleObjects ]
public class CharacterBodyEditor : Editor
{
	
	CharacterBody monobehaviour;

	SerializedProperty drawBodyShapeGizmo = null;

	void OnEnable()
	{		
		if(monobehaviour == null)
			monobehaviour = (CharacterBody)target;
		
		drawBodyShapeGizmo = serializedObject.FindProperty( "drawBodyShapeGizmo" );
	}
	


	void OnSceneGUI()
	{	
		
		if (!drawBodyShapeGizmo.boolValue )
			return;		
			
	}

	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GUILayout.Space( 10f );
		GUI.enabled = false;
		EditorGUILayout.FloatField( "Skin width" , CharacterConstants.SkinWidth );
		GUI.enabled = true;
		
		EditorGUILayout.HelpBox( "To modify the skin width value please check the \"CharacterConstants.cs\" file" , MessageType.Info );
	}
}

}


#endif
