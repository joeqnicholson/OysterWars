using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMachine : MonoBehaviour
{
    [SerializeField] WadeMachine machine;
    CameraBox currentCameraBox;
    Transform boxTransform;
    Vector3 Target;
    public Transform sideTest;
    public float leftSide;
    public float rightSide;
    public float bottomSide;
    public float topSide;
    public Vector3 shakeVector;

    private void Start()
    {
        machine = GameData.Instance.machine;
    }

    
    void FixedUpdate()
    {
        UpdateBounds();
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
        transform.position = Vector3.Lerp(transform.position, Target, 4 * Time.deltaTime) + shakeVector;
    }

    public void ChangeTarget(CameraBox currentCamera)
    {
        
    }

    public void UpdateBounds()
    {
        topSide = transform.position.y + 135;
        bottomSide = transform.position.y - 135;
        rightSide = transform.position.x + 240;
        leftSide = transform.position.x - 240;
    }

    public IEnumerator CameraShake(float magnitude, float duration)
    {
        float elapsed = 0;
        print("camershake");
        while(elapsed < duration)
        {
            shakeVector = Vector3.Lerp(shakeVector,new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), 0),3 * Time.deltaTime);
            print(shakeVector);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeVector = Vector3.zero;


        
    }

}
