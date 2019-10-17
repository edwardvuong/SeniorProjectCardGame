﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    private Queue<Event_Base> stateQueue = new Queue<Event_Base>();

    private bool eventActive = false;
    
    private static GameEventManager _instance;

    public static GameEventManager Instance
    {
        get => _instance;
    }
    

    private void Awake()
    {
        if (_instance == null && _instance != this)
            _instance = this;
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void AddStateToQueue(Event_Base eventState)
    {
        if (stateQueue.Count == 0)
        {
            //Debug.Log("Adding to empty queue");

            stateQueue.Enqueue(eventState);
            StartCoroutine(HandleState());
        }
        else
        {
            //Debug.Log("Adding to nonempty queue");
            stateQueue.Enqueue(eventState);
        }
    }

    public void EndEvent()
    {
        //Debug.Log("event ended");

        eventActive = false;
    }

    IEnumerator HandleState()
    {
        Event_Base nextEventToRun = stateQueue.Peek();
        
        nextEventToRun.EventState();
        
        eventActive = true;

        while (eventActive)
        {
            yield return null;
        }

        stateQueue.Dequeue();

        if (stateQueue.Count > 0)
        {
            StartCoroutine(HandleState());
        }
    }


}
