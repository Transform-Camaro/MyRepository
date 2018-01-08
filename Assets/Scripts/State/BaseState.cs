using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public abstract bool OnBlock();
    public abstract void OnEnter();
    public abstract void OnExecute();
    public abstract void OnExit();
}
