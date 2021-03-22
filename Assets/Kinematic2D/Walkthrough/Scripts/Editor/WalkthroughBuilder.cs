using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Lightbug.Kinematic2D.Walkthrough
{

public class WalkthroughBuilder : EditorWindow
{
	const string Path = "Assets/Kinematic2D/Walkthrough/Scenes";

    static List<SceneAsset> sceneAssets = new List<SceneAsset>();

	
   	[MenuItem("Kinematic 2D/Walkthrough Builder")]
    public static void ShowWindow()
	{        

		bool result = EditorUtility.DisplayDialog( 
			"Walkthrough builder" ,
			 $"This action will remove all the current scenes from your build settings and replace them with the walkthrough scenes instead.\n\n{Path}/..." ,
			 "I understand" ,
			 "Don't do it!"
		);

		if( result )
			Build();
		
	}
	
	
	static void Build()
	{
		sceneAssets.Clear();

		List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

		string[] guids = AssetDatabase.FindAssets( 
			"t:Scene" , 
			new[]
			{ 
				Path
			}
		);
		
		Debug.Log( guids.Length + " scenes added to the build settings scene list.");
		
		foreach (var guid in guids)
		{
		    string assetPath = AssetDatabase.GUIDToAssetPath( guid );
		    sceneAssets.Add ((SceneAsset)AssetDatabase.LoadAssetAtPath( assetPath , typeof(SceneAsset) ) );
		}
		

		foreach (var sceneAsset in sceneAssets)
		{
			string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
			if (!string.IsNullOrEmpty(scenePath))
				editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
		}

		EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
	}
	
	

}

}