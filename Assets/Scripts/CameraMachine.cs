using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMachine : MonoBehaviour
{
    [SerializeField] WadeMachine machine;
    CameraBox currentCameraBox;
    Transform boxTransform;
    Vector3 Target;

    private void Start()
    {
        machine = GameData.Instance.machine;
    }

    
    void FixedUpdate()
    {
        currentCameraBox = machine.currentCameraBox;
        boxTransform = currentCameraBox.transform;

        if (machine.currentCameraBox)
        {
            


            Target = new Vector3(
                    currentCameraBox.onTrackX ? machine.transform.position.x : boxTransform.position.x,
                    currentCameraBox.onTrackY ? machine.transform.position.y : boxTransform.position.y,
                    -10
                    );

            Target.x = Mathf.Clamp(
                Target.x,
                boxTransform.position.x - (boxTransform.localScale.x / 2f) +240,
                boxTransform.position.x + (boxTransform.localScale.x / 2f) -240
            );


            Target.y = Mathf.Clamp(
                Target.y,
                boxTransform.position.y - (boxTransform.localScale.y / 2f) + 135f,
                boxTransform.position.y + (boxTransform.localScale.y / 2f) - 135f
            );
            
      
        }

        


    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, Target, 4 * Time.deltaTime);
    }

    public void ChangeTarget(CameraBox currentCamera)
    {
        
    }

}
