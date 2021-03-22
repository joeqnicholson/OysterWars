using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

public class CharacterHybridBrain : CharacterBrain
{
    
    [SerializeField]
    bool isAI = false;


    // Human brain ----------------------------------------------------------------------------
    public CharacterInputData inputData;


    // AI brain -------------------------------------------------------------------------------
    public AIBehaviourType behaviourType;

	
	[SerializeField]
	CharacterAISequenceBehaviour sequenceBehaviour = null;


	[Tooltip("This field is used when the character needs to reach a \"target\" GameObject.")]
	[SerializeField] Transform targetObject = null;

	[SerializeField] float reachDistance = 1f;

	[Tooltip("The wait time between actions updates.")]
	[Range_NoSlider(true)] [SerializeField] float refreshTime = 0.5f;
	
    //------------------------------------------------------------------------------------------------
  
	int currentActionIndex = 0;

	float waitTime = 0f;
	float time = 0f;
	
    void Start()
    {
        SetBrainType( isAI );
    }
    
    public void SetBrainType( bool AI )
    {
        if( !AI )
        {
            if( inputData == null )
            {
                Debug.Log( "The input data field is null" );
                return;
            }
            
        }
        else
        {
            SetAIBehaviour( behaviourType );        
        }

        this.isAI = AI;
        
        ResetActions();

    }

    public void SetAIBehaviour( AIBehaviourType type )
    {
        switch( type )
        {
            // Sequence
            case AIBehaviourType.Sequence:

                if(sequenceBehaviour == null)
                {
                    Debug.Log( "Follow behaviour is null" );
                    return;
                }

                currentActionIndex = 0;
                	

            break;

            // FOllow
            case AIBehaviourType.Follow:

			if(targetObject == null)
			{
				Debug.Log( "Sequence behaviour is null" );
				return;
			}                

				waitTime = refreshTime;

            break;
        }
	

        behaviourType = type;	

        time = 0;
    }
	

	public override bool IsAI
	{
		get
		{
			return isAI;
		}
	}
	
		
	/// <summary>
	/// Checks for human inputs and updates the action struct.
	/// </summary>
	protected override void Update()
	{

		if( isAI )
            UpdateAIBrain();
		else
            UpdateHumanBrain();		

	}

    #region Human

    void UpdateHumanBrain()
    {
		
        if( inputData == null || Time.timeScale == 0 )
			return;

		characterActions.right |= GetAxis( inputData.horizontalAxisName ) > 0;
		characterActions.left |= GetAxis( inputData.horizontalAxisName ) < 0;
		characterActions.up |= GetAxis( inputData.verticalAxisName ) > 0;
		characterActions.down |= GetAxis( inputData.verticalAxisName ) < 0;
		
		characterActions.jumpPressed |= GetButtonDown( inputData.jumpName );
		characterActions.jumpReleased |= GetButtonUp( inputData.jumpName );

		characterActions.dashPressed |= GetButtonDown( inputData.dashName );
		characterActions.dashReleased |= GetButtonUp( inputData.dashName );

		characterActions.jetPack |= GetButton( inputData.jetPackName );
    }

	
	protected virtual float GetAxis( string axisName , bool raw = true )
	{
		float value = 0f;
		try
		{
			value = raw ? Input.GetAxisRaw( axisName ) : Input.GetAxis( axisName );
		}
		catch (System.Exception)
		{
			Debug.LogWarning( $"{axisName} action not found. Did you set up the project inputs settings?" );
		}
		return value;
	}

	protected virtual bool GetButton( string actionInputName )
	{
		bool value = false;
		try
		{
			value = Input.GetButton( actionInputName );
		}
		catch (System.Exception)
		{
			Debug.LogWarning( $"{actionInputName} action not found. Did you set up the project inputs settings?" );
		}
		return value;
	}

	protected virtual bool GetButtonDown( string actionInputName )
	{
		bool value = false;
		try
		{
			value = Input.GetButtonDown( actionInputName );
		}
		catch (System.Exception)
		{
			Debug.LogWarning( $"{actionInputName} action not found. Did you set up the project inputs settings?" );
		}
		return value;
	}

	protected virtual bool GetButtonUp( string actionInputName )
	{
		bool value = false;
		try
		{
			value = Input.GetButtonUp( actionInputName );
		}
		catch (System.Exception)
		{
			Debug.LogWarning( $"{actionInputName} action not found. Did you set up the project inputs settings?" );
		}
		return value;
	}

    #endregion


    #region AI
    
    void UpdateAIBrain()
	{		
		
		if( time >= waitTime )
		{
			switch( behaviourType )
			{
				case AIBehaviourType.Sequence:

					UpdateSequenceBehaviour();					

				break;

				case AIBehaviourType.Follow:

					UpdateFollowBehaviour();					

				break;
			}
			
			time = 0;			
		}
		else
		{
			time += Time.deltaTime;
		}

		
		
		
	}

	// Sequence Behaviour --------------------------------------------------------------------------------------------------

	void UpdateSequenceBehaviour()
	{
		if(sequenceBehaviour == null)
		{
			return;
		}

		characterActions.Reset();
		characterActions = sequenceBehaviour.ActionSequence[currentActionIndex].action;		
		waitTime = sequenceBehaviour.ActionSequence[currentActionIndex].duration;

		if( currentActionIndex == ( sequenceBehaviour.ActionSequence.Count - 1 ) )
			currentActionIndex = 0;
		else
			currentActionIndex++;
	}


	// Follow Behaviour --------------------------------------------------------------------------------------------------

	void UpdateFollowBehaviour()
	{
		if( targetObject == null )
			return;
		
		characterActions.Reset();

		
		float signedAngle = Vector3.SignedAngle( transform.up , Vector3.up , Vector3.forward );
		Vector3 delta = targetObject.position - transform.position;
		Vector3 rotatedDelta = Quaternion.AngleAxis( signedAngle , Vector3.forward ) * delta;

				
		if( Mathf.Abs( rotatedDelta.x ) <= reachDistance )
		{			
			characterActions.Reset();
			return;
		}
		else
		{
			if( rotatedDelta.x > 0 )
			{
				characterActions.right = true;
			}
			else
			{
				characterActions.left = true;
			}

		}		

		
	}

    #endregion
}

}
