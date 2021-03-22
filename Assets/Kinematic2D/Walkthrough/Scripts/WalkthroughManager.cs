using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WalkthroughManager : MonoBehaviour
{
	
	int currentSceneIndex = 0;
	int scenesNumber;

	void Awake()
	{
		DontDestroyOnLoad( gameObject );
	}

	void Update()
	{
		if( Input.GetKeyDown( KeyCode.E ) )
		{
			NextScene();
		}
		else if( Input.GetKeyDown( KeyCode.Q ) )
		{
			PreviousScene();
		}
		else if( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Exit();
		}
	}

	public void NextScene()
	{
		if( currentSceneIndex ==  SceneManager.sceneCountInBuildSettings - 1 )
			return;
		
		currentSceneIndex++;		
		SceneManager.LoadScene( currentSceneIndex );


	}

	public void PreviousScene()
	{
		if( currentSceneIndex == 0 )		
			return;

		currentSceneIndex--;		
		SceneManager.LoadScene( currentSceneIndex );

	}

	

	public void Exit()
	{
		Application.Quit();
	}

	
}
