using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Core
{

/// <summary>
/// This component is responsible for updating the position and rotation of all the actors in the scene (character and kinematic objects), guaranteeing a nice interpolated movement.
/// </summary>
[DefaultExecutionOrder(-100)]
public sealed class SceneController : MonoBehaviour
{
    static SceneController instance = null;
    public static SceneController Instance
    {
        get
        {
            return instance;
        }
    }

    public static void CreateSceneController()
    {
        GameObject sceneController = new GameObject("Scene Controller");			
		sceneController.AddComponent<SceneController>();
    }


    [Tooltip("Disable this field if you want to manually simulate the scene.")]
    [SerializeField]
    bool autoSimulation = true;

    [Tooltip("Whether or not to use the default Unity's interpolation.")]
    [SerializeField]
    bool useInterpolation = true;
    
    List<CharacterMotor> characterActors = new List<CharacterMotor>();
    List<KinematicPlatform> kinematicPlatforms = new List<KinematicPlatform>();
  
    #region events

    public event System.Action<float> OnSimulationStart;
    public event System.Action<float> OnSimulationEnd;
    public event System.Action<float> OnCharacterSimulationStart;
    public event System.Action<float> OnCharacterSimulationEnd;

    #endregion

    void Awake()
    {
        if( instance == null )
        {
            instance = this;
            DontDestroyOnLoad( gameObject );
        }
        else
        {
            Destroy( gameObject );
        }
       
             
    }

    // Add actor -----------------------------------------------

    public void AddActor( CharacterMotor characterMotor )
    {
        characterActors.Add( characterMotor );
    }

    public void AddActor( KinematicPlatform kinematicPlatform )
    {
        kinematicPlatforms.Add( kinematicPlatform );
    }

    // Remove actor -----------------------------------------------
    public void RemoveActor( CharacterMotor characterMotor )
    {
        characterActors.Remove( characterMotor );
    }

    public void RemoveActor( KinematicPlatform kinematicPlatform )
    {
        kinematicPlatforms.Remove( kinematicPlatform );
    }


    void InterpolateRigidbodyComponent( RigidbodyComponent rigidbodyComponent )
    {
            
        Vector3 startPosition = rigidbodyComponent.transform.position;  
        Vector3 endPosition = rigidbodyComponent.Position;
        
        Quaternion startRotation = rigidbodyComponent.transform.rotation;
        Quaternion endRotation = rigidbodyComponent.Rotation;

        rigidbodyComponent.Position = startPosition;
        rigidbodyComponent.Rotation = startRotation;

        rigidbodyComponent.Interpolate( endPosition , endRotation );
    }

    void FixedUpdate()
    {
        if( !autoSimulation )
            return;
        
        float dt = Time.deltaTime;
        
        
        Simulate( dt );        
    }

    /// <summary>
    /// Updates and interpolates all the actors in the scene.
    /// </summary>
    public void Simulate( float dt )
    {
        if( OnSimulationStart != null )
            OnSimulationStart( dt );
        
        for( int i = 0 ; i < kinematicPlatforms.Count ; i++ )
        {
            KinematicPlatform kinematicPlatform = kinematicPlatforms[i];

            if( !kinematicPlatform.enabled )
                continue;
            
            kinematicPlatform.UpdatePlatform( dt );
        }

        if( OnCharacterSimulationStart != null )
            OnCharacterSimulationStart( dt );
        
        for( int i = 0 ; i < characterActors.Count ; i++ )
        {      
            CharacterMotor characterMotor = characterActors[i];

            
            if( !characterMotor.enabled )
                continue;
            
            characterMotor.UpdateCharacter( dt );            
        }   

        if( OnCharacterSimulationEnd != null )
            OnCharacterSimulationEnd( dt );     

       


        // Interpolation ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        if( useInterpolation )
        {
        
            for( int i = 0 ; i < kinematicPlatforms.Count ; i++ )
            {
                KinematicPlatform kinematicPlatform = kinematicPlatforms[i];  

                if( !kinematicPlatform.enabled )
                    continue;

                InterpolateRigidbodyComponent( kinematicPlatform.RigidbodyComponent );
            }     

            for( int i = 0 ; i < characterActors.Count ; i++ )
            {
                CharacterMotor characterMotor = characterActors[i];  

                if( !characterMotor.enabled )
                    continue;

                InterpolateRigidbodyComponent( characterMotor.RigidbodyComponent );
            }  


        }


        if( OnSimulationEnd != null )
            OnSimulationEnd( dt );

    }
    
}


}