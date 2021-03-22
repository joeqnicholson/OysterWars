using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;

namespace Lightbug.Kinematic2D.Implementation
{	


public abstract class CharacterAbility : MonoBehaviour 
{
     
	public delegate void CharacterAbilityEvent();

     
     protected CharacterBody characterBody;
     protected CharacterBrain characterBrain;
     protected CharacterController2D characterController2D;   
     
     protected StateMachineController<MovementState> movementController; 
     protected StateMachineController<PoseState> poseController; 
         

     protected virtual void Awake()
     {          
          characterBrain = transform.root.GetComponentInChildren<CharacterBrain>();
          characterBody = transform.root.GetComponentInChildren<CharacterBody>();
          characterController2D = transform.root.GetComponentInChildren<CharacterController2D>();

          if( characterController2D == null )
          {
               Debug.Log( "CharacterController2D missing!" );
               this.enabled = false;
          }
          else
          {
               movementController = characterController2D.MovementController;
               poseController = characterController2D.PoseController;
          }
          

     }

     // Required to see the checkbox in the inspector.
     protected virtual void OnEnable(){}

	public virtual void Process( float dt ){}

     public virtual string GetInfo()
	{ 
		return ""; 
	}
	
}

}
