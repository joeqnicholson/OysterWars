using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OysterPieceMachine : Enemy
{
    [SerializeField] private SpriteAnimation Idle;
    [SerializeField] private SpriteAnimation Spit;
    [SerializeField] private SpriteAnimation Turn;
    [SerializeField] private Transform bulletPosition;
    [SerializeField] public GameObject bullet;

    public int Index;

    public void Start()
    {
        base.Start();

        IdleState();
        transform.localScale = Vector3.zero;
        StartCoroutine(IntroGrow());
    }

    public void Update()
    {

        if(sprite.currentSprite == Spit || sprite.currentSprite == Turn)
        {
            if(sprite.stopped)
            {
                IdleState();
            }
        }

        

    }

    public void IdleState()
    {
        if(Index > 1)
        {
            sprite.direction = -1;
        }
        else
        {
            sprite.direction = 1;
        }

        sprite.Play(Idle);
    }

    public IEnumerator Move(Vector3 movePos, int positionIndex)
    {
        print("we're here bucko");

        Index = positionIndex;

        if(positionIndex == 2 || positionIndex == 0)
        {
            sprite.Play(Turn);
        }

        while(Vector2.Distance(transform.position, movePos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, movePos, 1.75f * Time.deltaTime);
            yield return null;
        }

        IdleState();

        positionIndex += 1;

    }
    public void SpitState()
    {
        sprite.Play(Spit);
        Bullet newBullet = Instantiate(bullet, bulletPosition.position, Quaternion.identity).GetComponent<Bullet>();
        float shootDirectionAngle = GameData.Instance.RegulatedWadeSignedAngle(transform.position, 1, MathHelper.SignedAngles45);
        bulletPosition.localRotation = Quaternion.Euler(0, 0, shootDirectionAngle);
        Vector3 bulletDirection = -bulletPosition.right;
        newBullet.enemyBullet = true;
        newBullet.ChangeSpeed(200);
        newBullet.ChangeMoveDirection(bulletDirection);
        GetComponent<AudioSource>().Play();
    }

    public IEnumerator IntroGrow()
    {
        float time = 0;
        while(time < .25f)
        {   
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;

        while(time < 10f)
        {
            time+=Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1,1,1), 6 * Time.deltaTime);
            yield return null;
        }
        transform.localScale = new Vector3(1,1,1);
       
    }

    public IEnumerator SpitRoutine(float waitTime)
    {
        float time = 0;
        while(time < waitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        SpitState();
    }

}
