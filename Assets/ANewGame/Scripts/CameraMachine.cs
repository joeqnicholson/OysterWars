using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraMachine : MonoBehaviour
{
    [SerializeField] WadeMachine machine;
    public CameraBox currentCameraBox;
    Transform boxTransform;
    public Vector3 Target;
    public Transform sideTest;
    public float leftSide;
    public float rightSide;
    public float bottomSide;
    public float topSide;
    public Vector3 shakeVector;
    private Vector2 size;
    [SerializeField] private GameObject runner;
    [SerializeField] private bool spawnEnemies;
    [SerializeField] private float ySpawnPosition =0;
    [SerializeField] private float enemyTimerLeft=0;
    [SerializeField] private float enemyTimerRight=0;
    [SerializeField] private float nextEnemyTimeLeft;
    [SerializeField] private float nextEnemyTimeRight;
    [SerializeField] private bool ySpawnGoingUp;
    private float spawnTimeMin = 3f;
    private float spawnTimeMax = 8f;
    private float ChangeTargetSpeed = .25f;
    private float timeVariable = 1;
    [SerializeField] LayerMask layerMask;
    public Transform thingsR;
    public Transform thingsL;
    public Vector3 velocity = Vector3.zero;



    private void Start()
    {
        nextEnemyTimeRight = Random.Range(spawnTimeMin, spawnTimeMax);
        nextEnemyTimeLeft = Random.Range(spawnTimeMin, spawnTimeMax);
        machine = FindObjectOfType<WadeMachine>();
        size.x = 480;
        size.y = 270;

    }

    
    void LateUpdate()
    {

        UpdateBounds();

        boxTransform = currentCameraBox.transform;

        

            Target = new Vector3(
                    machine.transform.position.x,
                    machine.transform.position.y + 16,
                    -10
                    );


        if (currentCameraBox)
        {
            Target.x = Mathf.Clamp(
                Target.x,
                boxTransform.position.x - (boxTransform.localScale.x / 2f) + (size.x / 2),
                boxTransform.position.x + (boxTransform.localScale.x / 2f) - (size.x / 2)
            );


            Target.y = Mathf.Clamp(
                Target.y,
                boxTransform.position.y - (boxTransform.localScale.y / 2f) + (size.y / 2),
                boxTransform.position.y + (boxTransform.localScale.y / 2f) - (size.y / 2)
            );

        }

        transform.position = Vector3.SmoothDamp(transform.position, Vector3Int.RoundToInt(Target), ref velocity, ChangeTargetSpeed) + shakeVector;

    }

   
    public void UpdateBounds()
    {
        topSide = transform.position.y + (size.y / 2);
        bottomSide = transform.position.y - (size.y / 2);
        rightSide = transform.position.x + (size.x / 2);
        leftSide = transform.position.x - (size.x / 2);
    }

    public IEnumerator CameraShake(float magnitude, float duration)
    {
        float elapsed = 0;
        while(elapsed < duration)
        {
            shakeVector = new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeVector = Vector3.zero;

    }

    private void SpawnEnemies()
    {

        enemyTimerRight += Time.deltaTime;
        enemyTimerLeft += Time.deltaTime;

        if (ySpawnGoingUp)
        {
            ySpawnPosition += 1;
        }
        else
        {
            ySpawnPosition -= 1;
        }

        if(ySpawnPosition > transform.position.y + size.y / 2)
        {
            ySpawnGoingUp = false;
        }

        if(ySpawnPosition < transform.position.y - size.y / 2)
        {
            ySpawnGoingUp = true;
        }

        Vector3 rightPoint = new Vector3(transform.position.x + (size.x / 2) - 15, ySpawnPosition, 0);
        Vector3 leftPoint = new Vector3(transform.position.x - (size.x / 2) + 15, ySpawnPosition, 0);

        RaycastHit2D hitInfoRight = Physics2D.Linecast(rightPoint, rightPoint + (Vector3.down * 90), layerMask);
        RaycastHit2D hitInfoLeft = Physics2D.Linecast(leftPoint, leftPoint + (Vector3.down * 90), layerMask);

        float distanceToTarget = Vector3.Distance(Target, transform.position);
     
   
      


        if(distanceToTarget < 50)
        {
            if (hitInfoRight && hitInfoRight.collider.gameObject.layer != 10)
            {
                if (hitInfoRight.distance > 32 && enemyTimerRight > nextEnemyTimeRight && GameData.Instance.IsOnScreen(hitInfoRight.point, Vector3.zero))
                {
                    print('2');
                    if (Vector3.Distance(machine.transform.position, hitInfoRight.point) > 48)
                    {
                        print('3');
                        enemyTimerRight = 0;
                        nextEnemyTimeRight = Random.Range(spawnTimeMin, spawnTimeMax + 1);
                        Instantiate(runner, hitInfoRight.point, Quaternion.identity);
                    }
                }
            }

            if (hitInfoLeft && hitInfoLeft.collider.gameObject.layer != 10)
            {
                if (hitInfoLeft.distance > 16 && enemyTimerLeft > nextEnemyTimeLeft && GameData.Instance.IsOnScreen(hitInfoLeft.point, Vector3.zero))
                {
                    if (Vector3.Distance(machine.transform.position, hitInfoLeft.point) > 48)
                    {
                        enemyTimerLeft = 0;
                        nextEnemyTimeLeft = Random.Range(spawnTimeMin, spawnTimeMax + 2);
                        Instantiate(runner, hitInfoLeft.point, Quaternion.identity);
                    }
                }
            }
        }
        



    }
    private void OnDrawGizmos()
    {
        
       
    }
}



