using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;

public partial class WadeMachine : Actor
{
    public class WadeState
    {
        public bool canGetHit = true;
        public bool canFlip = true;
        public string name = "Generic Name";
    }

    public WadeState CurrentWadeState;

    private WadeState StNormal = new WadeState { canGetHit = true, name = "Normal" };

    private WadeState StChest = new WadeState { canGetHit = false };

    private WadeState StHit = new WadeState { canGetHit = false, canFlip = false };

    private WadeState StSwing = new WadeState { canGetHit = true, name = "Swing" };

    private WadeState StClimb = new WadeState { canGetHit = true };

    private WadeState StLaunch = new WadeState { name = "Launch" };

    public void TransitionToState(WadeState newState)
    {
        WadeState tmpInitialState = CurrentWadeState;
        OnStateExit(tmpInitialState, newState);
        CurrentWadeState = newState;
        OnStateEnter(newState, tmpInitialState);
    }

    void OnStateEnter(WadeState toState, WadeState fromState)
    {
        stateTimer = 0;
        if(toState == StNormal)
        {
            print(Speed);
        }

        if(toState == StHit)
        {
            Sound.PlayWadeHit();
            StartCoroutine(GameData.Instance.cameraMachine.CameraShake(2, .2f));
            Speed.x = 100 * -directionInt;
            Speed.y = 70;
            Time.timeScale = .1f;
        }

        if (toState == StChest)
        {
            Speed.x = 0;
            Speed.y = 0;
        }

        if (toState == StSwing)
        {
            Speed.x = 0;
            Speed.y = 0;
            stillTimer = 0;
        }
    }

    void OnStateExit(WadeState fromState, WadeState toState)
    {
        if (fromState == StNormal)
        {

        }

        if (fromState == StHit)
        {
            if (teleportHit)
            {
                transform.position = spawnPoint;
            }

            teleportHit = false;

            invincibiltyTimer = 0;

            Time.timeScale = 1;

        }

        if(fromState == StChest)
        {
            canInteract = false;
        }
    }

}
