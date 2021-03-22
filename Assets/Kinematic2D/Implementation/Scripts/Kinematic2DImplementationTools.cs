#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

public class Kinematic2DImplementationTools : EditorWindow
{
    string labelField = "";
    static UnityEngine.Object selectedObject = null;

    [ MenuItem( "Assets/Create/Kinematic 2D/Implementation/Character Ability" )]
    static void Init()
    {

        selectedObject = Selection.activeObject;

        Kinematic2DImplementationTools window = ScriptableObject.CreateInstance<Kinematic2DImplementationTools>();
        window.position = new Rect( Screen.width / 2 , Screen.height / 2 , 400 , 50);
        window.ShowPopup();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical( "Box" );
        GUILayout.BeginHorizontal();

        GUILayout.Label("Ability Name ", EditorStyles.wordWrappedLabel );
        labelField = EditorGUILayout.TextField( labelField );

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        
        if (GUILayout.Button("Create"))
        {
            string path = selectedObject != null ? AssetDatabase.GetAssetPath( selectedObject ) + "/" : "Assets/";
            
            bool result = CreateAbility( labelField , path );

            if( result )
                Debug.Log( "Ability created successfully." );

            this.Close();

        }

        if (GUILayout.Button("Cancel"))
            this.Close();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    bool CreateAbility( string name , string path )
    {       
        if( string.Equals( name , "" ) )
        {
            Debug.Log("Empty name");
            return false;
        }
        
        name = name.Replace(" ","_");
        name = name.Replace("-","_");

        string fullPath = path + name + ".cs";
        

        if( File.Exists(fullPath))
        {
            Debug.Log("File already exist!.");
            return false;
        }

        

        using ( StreamWriter outfile = new StreamWriter(fullPath) )
        {				
            outfile.WriteLine( "using System.Collections;" );
            outfile.WriteLine( "using System.Collections.Generic;" );
            outfile.WriteLine( "using UnityEngine;" );
            outfile.WriteLine( "using Lightbug.Kinematic2D.Implementation;" );
            outfile.WriteLine( "" );
            outfile.WriteLine( "" );
            outfile.WriteLine( "[System.Serializable]" );
            outfile.WriteLine( "public class " + name + " : CharacterAbility" );
            outfile.WriteLine( "{" );
            outfile.WriteLine( "");
            outfile.WriteLine( "    // Write your initialization code here" );
            outfile.WriteLine( "    protected override void Awake()" );
            outfile.WriteLine( "    {" );
            outfile.WriteLine( "        base.Awake();" );
            outfile.WriteLine( "    }" );
            outfile.WriteLine( "" );
            outfile.WriteLine( "    // Write your update code here" );
            outfile.WriteLine( "	public override void Process( float dt )" );
            outfile.WriteLine( "    {" );
            outfile.WriteLine( "        " );
            outfile.WriteLine( "	}" );
            outfile.WriteLine( "" );
            outfile.WriteLine( "	public override string GetInfo()" );
            outfile.WriteLine( "	{" );
            outfile.WriteLine( "		return \"Describe your ability here!\";" );
            outfile.WriteLine( "	}" );
            outfile.WriteLine( "" );
            outfile.WriteLine( "}");

            
        }
        

        AssetDatabase.Refresh();

        return true;
        
    }
}

#endif