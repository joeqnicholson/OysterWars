using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR

using UnityEditor;

namespace Lightbug.Kinematic2D.Implementation
{
 

[CustomEditor( typeof(CharacterHybridBrain) ) , CanEditMultipleObjects]
public class CharacterHybridBrainEditor : Editor
{
    

    SerializedProperty isAI = null;

    SerializedProperty inputData = null;   

    SerializedProperty behaviourType = null;	
    SerializedProperty sequenceBehaviour = null;
	SerializedProperty targetObject = null;
    SerializedProperty reachDistance = null;
	SerializedProperty refreshTime = null;

    Editor inputEditor = null;
    Editor sequenceEditor = null;

    void OnEnable()
    {
        isAI = serializedObject.FindProperty("isAI");

        inputData = serializedObject.FindProperty("inputData");   

        behaviourType = serializedObject.FindProperty("behaviourType");	

        sequenceBehaviour = serializedObject.FindProperty("sequenceBehaviour");

        targetObject = serializedObject.FindProperty("targetObject");

        reachDistance = serializedObject.FindProperty("reachDistance");

        refreshTime = serializedObject.FindProperty("refreshTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

            Utilities.CustomUtilities.DrawMonoBehaviourField<CharacterHybridBrain>( (CharacterHybridBrain)target);

        GUILayout.Space(10);

        //GUILayout.BeginVertical( EditorStyles.helpBox );

        GUILayout.BeginHorizontal();

        //GUI.enabled = isAI.boolValue;
        GUI.color = isAI.boolValue ? Color.white : Color.green;
        if( GUILayout.Button( "Human" , EditorStyles.miniButton ) )
        {
            isAI.boolValue = false;
        }
        
        //GUI.enabled = !isAI.boolValue;
        GUI.color = !isAI.boolValue ? Color.white : Color.green;
        if( GUILayout.Button( "AI" , EditorStyles.miniButton ) )
        {
            isAI.boolValue = true;
        }

        GUI.color = Color.white;

        //GUI.enabled = true;

        GUILayout.EndHorizontal();

            Utilities.CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray );

        GUILayout.Space(15);

        if( isAI.boolValue )
        {
            EditorGUILayout.PropertyField( behaviourType );

            GUILayout.Space(10);

                Utilities.CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray );

            //GUILayout.BeginVertical( EditorStyles.helpBox );

            if( behaviourType.enumValueIndex == 0)
            {

                EditorGUILayout.PropertyField( sequenceBehaviour );



                if( sequenceBehaviour.objectReferenceValue != null )
                {
                    if( sequenceEditor == null )
                        CreateCachedEditor( sequenceBehaviour.objectReferenceValue , null , ref sequenceEditor );
                    
                    sequenceEditor.OnInspectorGUI();
                }
                else
                {
                    if( sequenceEditor != null )
                        sequenceEditor = null;

                    EditorGUILayout.HelpBox( "Select a Sequence Behaviour asset" , MessageType.Warning );
                }

            }
            else
            {
                EditorGUILayout.PropertyField( targetObject );

                if( targetObject.objectReferenceValue != null )
                {
                    EditorGUILayout.PropertyField( reachDistance );

                    EditorGUILayout.PropertyField( refreshTime );

                }
                else
                {
                    EditorGUILayout.HelpBox( "Assign a target" , MessageType.Warning );
                }

                
            }

            //GUILayout.EndVertical();

        }
        else
        {
            EditorGUILayout.PropertyField( inputData );

            GUILayout.Space(10);

                Utilities.CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray );

            

            if( inputData.objectReferenceValue != null )
            {
                //GUILayout.BeginVertical( EditorStyles.helpBox );

                if( inputEditor == null )
                    CreateCachedEditor( inputData.objectReferenceValue , null , ref inputEditor );
                
                inputEditor.OnInspectorGUI();

                //GUILayout.EndVertical();
            }
            else
            {
                if( inputEditor != null )
                    inputEditor = null;
                
                EditorGUILayout.HelpBox( "Select a Character Input Data asset" , MessageType.Warning );
            }

            

        }

        GUILayout.Space(10);


        //GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
    
}

}

#endif
