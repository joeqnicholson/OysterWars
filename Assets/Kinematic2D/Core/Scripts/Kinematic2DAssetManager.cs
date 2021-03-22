#if UNITY_EDITOR

using UnityEngine;


namespace Lightbug.Kinematic2D.Core
{
	


using UnityEditor;
using UnityEditorInternal;

public class Kinematic2DAssetManager : EditorWindow
{

	[MenuItem ( "GameObject/Kinematic 2D/2D Character" , false , 10)]
	static void Create2DCharacter(MenuCommand menuCommand)
	{
		GameObject characterObj = new GameObject("2D Character");
		characterObj.AddComponent<CharacterBody2D>();
		characterObj.AddComponent<CharacterMotor>();

		GameObject graphicsObj = new GameObject("Graphics"); 
		graphicsObj.transform.SetParent( characterObj.transform );
		graphicsObj.AddComponent<CharacterGraphics>();

		Camera sceneCamera = SceneView.GetAllSceneCameras()[0];

		if(sceneCamera == null)
		{
			characterObj.transform.position = Vector3.zero;		
		}
		else
		{
			characterObj.transform.position = new Vector3( 
				sceneCamera.transform.position.x , 
				sceneCamera.transform.position.y ,
				0 
			);
		}
		
		
		SpriteRenderer spriteRenderer = graphicsObj.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = Resources.Load<Sprite>( "Sprites/BaseSprite" );

		
		GameObjectUtility.SetParentAndAlign(characterObj, menuCommand.context as GameObject);
		
		Undo.RegisterCreatedObjectUndo(characterObj, "Create Kinematic 2D Character");
		Selection.activeObject = characterObj;

	}

	[MenuItem ( "GameObject/Kinematic 2D/3D Character" , false , 10)]
	static void Create3DCharacter(MenuCommand menuCommand)
	{
		GameObject characterObj = new GameObject("3D Character");
		characterObj.AddComponent<CharacterBody3D>();
		characterObj.AddComponent<CharacterMotor>();
		
		
		GameObject graphicsObj = GameObject.CreatePrimitive( PrimitiveType.Cube );
		DestroyImmediate( graphicsObj.GetComponent<BoxCollider>() );
		graphicsObj.transform.SetParent( characterObj.transform );
		graphicsObj.transform.Translate( Vector3.up * 0.5f );
		graphicsObj.AddComponent<CharacterGraphics>();

		Camera sceneCamera = SceneView.GetAllSceneCameras()[0];

		if(sceneCamera == null)
		{
			characterObj.transform.position = Vector3.zero;		
		}
		else
		{
			characterObj.transform.position = new Vector3( 
				sceneCamera.transform.position.x , 
				sceneCamera.transform.position.y ,
				0 
			);
		}
		
		
		
		GameObjectUtility.SetParentAndAlign(characterObj, menuCommand.context as GameObject);
		
		Undo.RegisterCreatedObjectUndo(characterObj, "Create Kinematic 2D Character");
		Selection.activeObject = characterObj;

	}	

	


}



}

#endif
