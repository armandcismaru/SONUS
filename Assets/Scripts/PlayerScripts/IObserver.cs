using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    public void Notify();
}

public interface IDamageObserver : IObserver
{
    public void Notify(int damage);
}

public interface IDieObserver : IObserver
{
}