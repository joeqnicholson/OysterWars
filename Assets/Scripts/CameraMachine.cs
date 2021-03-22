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

    void Update()
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
            boxTransform.position.x - (boxTransform.localScale.x / 2f) + 112.5f,
            boxTransform.position.x + (boxTransform.localScale.x / 2f) - 112.5f
        );

            Target.y = Mathf.Clamp(
                Target.y,
                boxTransform.position.y - (boxTransform.localScale.y / 2f) + 112.5f,
                boxTransform.position.y + (boxTransform.localScale.y / 2f) - 112.5f
            );

            transform.position = Vector3.Lerp(transform.position, Target, 6 * Time.deltaTime);
            
                    
        }

        
    }

    public void ChangeTarget(CameraBox currentCamera)
    {
        
    }

}
