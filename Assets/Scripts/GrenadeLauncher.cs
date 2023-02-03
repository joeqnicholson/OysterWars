using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    [SerializeField] private bool active;
    public GameObject grenade;


    void Start()
    {
        
    }


    void Update()
    {
        if(GameData.Instance.IsOnScreen(transform.position, Vector3.zero))
        {
            if(!active)
            {
                active = true;
                LaunchGrenade();
            }
        }
        else
        {
            if(active)
            {
                active = false;
            }
        }
    }

    void LaunchGrenade()
    {
        if(active)
        {
            print("grenade");
            GameObject newGrenade = Instantiate(grenade, transform.position,Quaternion.identity);
            Bullet bulletScript = newGrenade.GetComponent<Bullet>();
            bulletScript.ChangeSpeed(120);
            bulletScript.MakeLob(250);
            bulletScript.ChangeMoveDirection(new Vector2(.2f,1));
            Invoke("LaunchGrenade", 3);
        }
        
    }
}
