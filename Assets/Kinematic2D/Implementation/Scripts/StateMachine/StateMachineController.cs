using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;

namespace Lightbug.Kinematic2D.Implementation
{


public class StateMachineController<T> where T : 
// #if UNITY_2018_3_OR_NEWER
// 	IComparable, IFormattable, IConvertible // System.Enum
// #else
	IComparable, IFormattable, IConvertible
// #endif
{

	T currentState = default(T);
	public T CurrentState
	{
		get
		{ 
			return currentState;
		} 
	}

	T previousState = default(T);
	public T PreviousState
	{
		get
		{ 
			return previousState;
		} 
	}


	protected CharacterMotor characterMotor = null;
	public CharacterMotor CharacterMotor
	{
		get
		{ 
			return characterMotor; 
		}
	}

	protected CharacterAnimation characterAnimation = null;
	public CharacterAnimation CharacterAnimation
	{
		get
		{ 
			return characterAnimation; 
		} 
	} 	
	
	
	/// <summary>
	/// Checks if the character is currently on a specific state.
	/// </summary>
	/// <param name="stateName">name of the state.</param>
	/// <returns>return true if the state current state name matched with the argument.</returns>
	public bool isCurrentlyOnState( T state )
	{
		return EqualityComparer<T>.Default.Equals( state, currentState );	
	}
	
	/// <summary>
	/// Sets the character state.
	/// </summary>
	public void SetState( T state )
	{	
		currentState = state;
	}
		
	
	/// <summary>
	/// Initializes the state controller.
	/// </summary>
	public virtual void Initialize( GameObject reference )
	{          
		characterMotor = reference.GetComponent<CharacterMotor>();
		characterAnimation = reference.GetComponent<CharacterAnimation>();
		
	}	


	

}

}



