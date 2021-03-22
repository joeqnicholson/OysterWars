using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.Kinematic2D.Implementation
{

/// <summary>
/// Contains the character actions internal values.
/// </summary>
[System.Serializable]
public struct CharacterActions 
{ 		
	
	public bool right;
	public bool left;
	public bool up;
	public bool down;

	public bool jumpPressed;
	public bool jumpReleased;

	public bool dashPressed;
	public bool dashReleased;

	public bool jetPack;

	public void Reset()
	{
		right = false;
		left = false;
		up = false;
		down = false;
		jumpPressed = false;
		jumpReleased = false;
		dashPressed = false;
		dashReleased = false;
		jetPack = false;
	}

	public bool isEmpty()
	{
		return !right &&
		!left &&
		!up &&
		!down &&
		!jumpPressed &&
		!jumpReleased &&
		!dashPressed &&
		!dashReleased &&
		!jetPack;
	}

}


public abstract class CharacterBrain : MonoBehaviour
{	
	
	protected CharacterActions characterActions = new CharacterActions();
	public CharacterActions CharacterActions
	{
		get
		{
			return characterActions;
		}
	}
	 
	public abstract bool IsAI { get; }	
	

	public void ResetActions()
	{
		characterActions.Reset();
	}

	public void SetAction( CharacterActions characterAction )
	{
		this.characterActions = characterAction;
	}

	protected abstract void Update();

	void OnDisable()
	{
		ResetActions();
	}
}

}
