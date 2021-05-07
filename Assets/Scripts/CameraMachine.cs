using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraMachine : MonoBehaviour
{
    [SerializeField] WadeMachine machine;
    CameraBox currentCameraBox;
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
    private float ySpawnPosition =0;
    private float enemyTimerLeft=0;
    private float enemyTimerRight=0;
    private float nextEnemyTimeLeft;
    private float nextEnemyTimeRight;
    private bool ySpawnGoingUp;
    private float spawnTimeMin = 3f;
    private float spawnTimeMax = 8f;
    


    private void Start()
    {
        nextEnemyTimeRight = Random.Range(spawnTimeMin, spawnTimeMax);
        nextEnemyTimeLeft = Random.Range(spawnTimeMin, spawnTimeMax);
        machine = GameData.Instance.machine;
        size.x = GetComponent<PixelPerfectCamera>().refResolutionX;
        size.y = GetComponent<PixelPerfectCamera>().refResolutionY;
    }

    
    void FixedUpdate()
    {


        UpdateBounds();

        if (spawnEnemies)
        {
            SpawnEnemies();
        }
        
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
                boxTransform.position.x - (boxTransform.localScale.x / 2f) + (size.x/2),
                boxTransform.position.x + (boxTransform.localScale.x / 2f) -(size.x/2)
            );


            Target.y = Mathf.Clamp(
                Target.y,
                boxTransform.position.y - (boxTransform.localScale.y / 2f) + (size.y/2),
                boxTransform.position.y + (boxTransform.localScale.y / 2f) - (size.y/2)
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



        RaycastHit2D hitInfoRight = Physics2D.Linecast(rightPoint, rightPoint + (Vector3.down * 90));
        RaycastHit2D hitInfoLeft = Physics2D.Linecast(leftPoint, leftPoint + Vector3.down * 90);

        float distanceToTarget = Vector3.Distance(Target, transform.position);



        if(distanceToTarget < 60)
        {
            if (hitInfoRight)
            {

                if (hitInfoRight.distance > 32 && enemyTimerRight > nextEnemyTimeRight && GameData.Instance.IsOnScreen(hitInfoRight.point, Vector3.zero))
                {
                    if (Vector3.Distance(machine.transform.position, hitInfoRight.point) > 48)
                    {
                        enemyTimerRight = 0;
                        nextEnemyTimeRight = Random.Range(spawnTimeMin, spawnTimeMax + 1);
                        Instantiate(runner, hitInfoRight.point, Quaternion.identity);
                    }
                }
            }

            if (hitInfoLeft)
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

}
