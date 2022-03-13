using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerSubject 
{
    void addObserver<T>(IObserver observer);
    void unsubscribeObserver<T>(IObserver observer);
}
