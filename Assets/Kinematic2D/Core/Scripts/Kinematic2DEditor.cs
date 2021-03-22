#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Lightbug.Kinematic2D.Core
{

class CCPAssetPostprocessor : AssetPostprocessor
{
    public const string RootFolder = "Assets/Kinematic2D";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
    {
        
        foreach( string importedAsset in importedAssets )
        {            
            if( importedAsset.Equals( RootFolder ) )
            {
                WelcomeWindow window =  ScriptableObject.CreateInstance<WelcomeWindow>(); 
                window.ShowUtility();
            }
        }
    }
}

public class Kinematic2DEditor : Editor
{
    [MenuItem( "Kinematic 2D/Welcome" )]
    public static void WelcomeMessage()
    {

        WelcomeWindow window = EditorWindow.GetWindow<WelcomeWindow>( true , "Welcome");                
        window.Show();
    }

    [MenuItem( "Kinematic 2D/Documentation" )]
    public static void Documentation()
    {
        Application.OpenURL( "https://docs.google.com/document/d/11_CB3sclSnEPjonVNLfRQXR9enBX1h3oBqyt99XMkXs/edit#heading=h.ed9qk9qflb2k" );
    }
    

    [MenuItem( "Kinematic 2D/About" )]
    public static void About()
    {
        AboutWindow window = EditorWindow.GetWindow<AboutWindow>( true , "About" );                
        window.Show();
    }
    
}

public abstract class Kinematic2DWindow : EditorWindow
{
    protected GUIStyle subtitleStyle = new GUIStyle();
    protected GUIStyle descriptionStyle = new GUIStyle();
    
    protected Texture bannerTexture = null;

    protected virtual void OnEnable()
    {        
        subtitleStyle.fontSize = 18;
        subtitleStyle.alignment = TextAnchor.MiddleCenter;
        subtitleStyle.padding.top = 4;
        subtitleStyle.padding.bottom = 4;
        subtitleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        descriptionStyle.fontSize = 15;
        descriptionStyle.wordWrap = true;
        descriptionStyle.padding.left = 10;
        descriptionStyle.padding.right = 10;
        descriptionStyle.padding.top = 4;
        descriptionStyle.padding.bottom = 4;
        descriptionStyle.richText = true;
        descriptionStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        bannerTexture = Resources.Load<Texture>( "Banner" );
    }
}

public class AboutWindow : Kinematic2DWindow
{
    const float Width = 200f;
    const float Height = 100f;
    

    protected override void OnEnable()
    {
        // base.OnEnable();
        this.position = new Rect( (Screen.width - Width ) / 2f , (Screen.height - Height ) / 2f , Width , Height );
        this.maxSize = this.minSize = this.position.size;
        this.titleContent = new GUIContent("About");
    }

    void OnGUI()
    {
        EditorGUILayout.SelectableLabel( "Version: 2.4.1" , GUILayout.Height(15f) );
        EditorGUILayout.SelectableLabel( "Author : Juan Sálice (Lightbug)" , GUILayout.Height(15f) );
        EditorGUILayout.SelectableLabel( "Mail : lightbug14@gmail.com" , GUILayout.Height(15f) );
    }
}


public class WelcomeWindow : Kinematic2DWindow
{

    

    protected override void OnEnable()
    {
        base.OnEnable();
    
        this.position = new Rect( 10f , 10f , 566f , 800f );
        this.maxSize = this.minSize = this.position.size;
    }

    void OnGUI()
    {
        GUILayout.Box( bannerTexture , GUILayout.Width(560f));

        GUILayout.Space(20f);

        GUILayout.Label(
        "Hi, welcome to <b>Kinematic 2D</b>."
        , descriptionStyle );

        GUILayout.Space(20f);

        GUILayout.BeginVertical( EditorStyles.helpBox );

        GUILayout.Label("Important" , subtitleStyle );        

        
        GUILayout.Label(
        "The demo scenes included in this package require you to set up some settings in your project (inputs and layers). " + 
        "<b>This is required only for demo purposes, the asset by itself (without the demo content involved) does not require any previous setup in order to work properly.</b>" , descriptionStyle );

        GUILayout.Label("Demo Setup" , subtitleStyle );

        GUILayout.Label(
        "1. Open the <b>Input manager settings</b>.\n" + 
        "2. Load the <b>Kinematic2D_Inputs</b> preset.\n" + 
        "3. Open the <b>Tags and Layers settings</b>.\n" +
        "4. Load the <b>Kinematic2D_Layers</b> preset.\n" , descriptionStyle );

        
        GUILayout.Label( "For more information about the setup, please visit the \"Setup\" section from the documentation."
        , descriptionStyle );
        
        GUILayout.EndVertical();

        GUILayout.Label("You can open this window by using the top menu: \n<i>Kinematic 2D/Welcome</i>" , descriptionStyle );
        
    }

}

}

#endif
