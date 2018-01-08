using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    BaseState CurrentState;//当前状态

    BaseState PreviousState;//上一个状态

    private List<BaseState> stateList;


    public StateMachine()
    {
        if (stateList == null)
        {
            stateList = new List<BaseState>();
        }
    }

    public void OnUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.OnExecute();
        }
    }

    public BaseState GetCurrentState()
    {
        return CurrentState;
    }
    public BaseState GetPreviousState()
    {
        return PreviousState;
    }

    public bool GetCurrentStateBlock()
    {
        bool block = true;
        if (CurrentState != null)
        {
            block = CurrentState.OnBlock();
        }
        return block;
    }

    public void ChangeState(BaseState newState)
    {
        if (GetCurrentStateBlock())
        {
            if (CurrentState != null)
            {
                PreviousState = CurrentState;

                PreviousState.OnExit();
            }

            CurrentState = newState;

            CurrentState.OnEnter();
        }
    }
    public void RemoveCurrentState(BaseState newState)
    {
        if (newState == CurrentState)
        {
            PreviousState = CurrentState;
            CurrentState = null;
        }
    }
}
