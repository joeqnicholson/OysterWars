using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditorInternal;

namespace Lightbug.Kinematic2D.Implementation
{

[CustomEditor( typeof(CharacterAISequenceBehaviour) )]
public class CharacterAISequenceBehaviourEditor : Editor
{
     ReorderableList reorderableList = null;

     SerializedProperty actionSequence = null;

     void OnEnable()
     {
          actionSequence = serializedObject.FindProperty("actionSequence");
          reorderableList = new ReorderableList( 
               serializedObject , 
               actionSequence , 
               true , 
               true , 
               true , 
               true
          );

          

          reorderableList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;

          reorderableList.drawElementCallback += OnDrawElement;
          reorderableList.drawHeaderCallback += OnDrawHeader;
          reorderableList.elementHeightCallback += OnElementHeight;
     }

     void OnDisable()
     {
          reorderableList.drawElementCallback -= OnDrawElement;
          reorderableList.drawHeaderCallback -= OnDrawHeader;
          reorderableList.elementHeightCallback -= OnElementHeight;
     }

     void OnDrawHeader( Rect rect )
     {
          GUI.Label( rect , "Sequence" );
     }

     void OnDrawElement( Rect rect , int index , bool isActive , bool isFocused )
     {
          SerializedProperty element = actionSequence.GetArrayElementAtIndex( index );
          
          SerializedProperty duration = element.FindPropertyRelative("duration");
          SerializedProperty action = element.FindPropertyRelative("action");

          GUI.Box( rect , "" , EditorStyles.helpBox );

          Rect fieldRect = rect;
          fieldRect.height = EditorGUIUtility.singleLineHeight;
          fieldRect.x += 20;
          fieldRect.width -= 30;

          fieldRect.y += 0.5f * fieldRect.height;
          
          EditorGUI.PropertyField( fieldRect , duration );
          fieldRect.y += 2 * fieldRect.height;

          EditorGUI.PropertyField( fieldRect , action , true );
          fieldRect.y += fieldRect.height;


          
     }

     float OnElementHeight( int index )
     {
          SerializedProperty element = actionSequence.GetArrayElementAtIndex( index );          
          SerializedProperty action = element.FindPropertyRelative("action");
          
          float actionHeight = action.isExpanded ? EditorGUI.GetPropertyHeight( action ) : EditorGUIUtility.singleLineHeight;
          return 3 * EditorGUIUtility.singleLineHeight + actionHeight;
          
     }

     public override void OnInspectorGUI()
     {
          serializedObject.Update();

          GUILayout.Space(15);

          reorderableList.DoLayoutList();

          GUILayout.Space(10);

          serializedObject.ApplyModifiedProperties();
     }
}

}

#endif
