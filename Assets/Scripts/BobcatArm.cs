using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobcatArm : MonoBehaviour
{
    [SerializeField] SpriteAnimation ArmRotation;
    public SpriteRenderer spriteRenderer;
    private int imageIndex;
    private float wadeAngle;
    private Vector3 shootPoint;
    private float shotTimer;
    private Vector2 shootDirection;
    [SerializeField] GameObject currentBullet;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.sprite = ArmRotation.Keyframe(imageIndex);
        wadeAngle = GameData.Instance.RegulatedWadeAngle(transform.position,1);

        float x = -39;
        float y = 0;
        shotTimer += Time.deltaTime;
        switch (wadeAngle)
        {
            case 0:
                {
                    x = -39;
                    y = 0;
                    imageIndex = 0;
                    break;
                }
            case 45:
                {
                    x = -29;
                    y = 29;
                    imageIndex = 1;
                    break;
                }
            case 90:
                {
                    x = 0;
                    y = 39;
                    imageIndex = 2;
                    break;
                }
            case 135:
                {
                    x = 29;
                    y = 29;
                    imageIndex = 3;
                    break;
                }
            case 180:
                {
                    x = 39;
                    y = 0;
                    imageIndex = 4;
                    break;
                }
            case 225:
                {
                    x = 29;
                    y = -29;
                    imageIndex = 5;
                    break;

                }
            case 270:
                {
                    y = -39;
                    x = 0;
                    imageIndex = 6;
                    break;
                }
            case 315:
                {
                    x = -29;
                    y = -29;
                    imageIndex = 7;
                    break;
                }
            case 360:
                {
                    x = -39;
                    y = 0;
                    imageIndex = 0;
                    break;
                }
        }
        
        if (shotTimer > 2)
        {
            print("jimbo");
            shootDirection = new Vector2(x, y);
            shootDirection = Vector2.ClampMagnitude(shootDirection, 1);
            shootPoint = transform.position + new Vector3(x, y, 0);
            Bullet newBullet = Instantiate(currentBullet, shootPoint, Quaternion.identity).GetComponent<Bullet>();
            newBullet.GetComponent<Bullet>().moveDirection = shootDirection;
            newBullet.GetComponent<Bullet>().enemyBullet = true;
            shotTimer = 0;

        }
        
    }
}
