using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;

public partial class WadeMachine : CharacterMotor
{
    public class WadeState
    {
        public bool canGetHit;
        public bool canFlip = true;
        
    }

    public WadeState CurrentWadeState;

    private WadeState StNormal = new WadeState { canGetHit = true };

    private WadeState StChest = new WadeState { canGetHit = false };

    private WadeState StHit = new WadeState { canGetHit = true, canFlip = false };






    public void TransitionToState(WadeState newState)
    {
        WadeState tmpInitialState = CurrentWadeState;
        OnStateExit(tmpInitialState, newState);
        CurrentWadeState = newState;
        OnStateEnter(newState, tmpInitialState);
    }

    void OnStateEnter(WadeState toState, WadeState fromState)
    {
        if(toState == StNormal)
        {

        }

        if(toState == StHit)
        {
            ForceNotGroundedState();
            Speed.x = 100 * -directionInt;
            Speed.y = 70;
            Time.timeScale = .1f;
        }

        if (toState == StChest)
        {
            Speed.x = 0;
            Speed.y = 0;
        }

    }

    void OnStateExit(WadeState fromState, WadeState toState)
    {
        if (fromState == StNormal)
        {

        }

        if (fromState == StHit)
        {
            invincibiltyTimer = 0;
            Time.timeScale = 1;
        }
    }

}
