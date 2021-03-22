
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Lightbug.Kinematic2D.Implementation
{

[CustomEditor( typeof( CharacterController2D ) ) , CanEditMultipleObjects ]
public class CharacterController2DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(10f);
        
        DrawDefaultInspector();

        GUILayout.Space(10f);
    }
}

}

#endif
