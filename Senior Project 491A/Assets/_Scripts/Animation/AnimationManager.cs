﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private static AnimationManager _instance;

    public static AnimationManager SharedInstance
    {
        get => _instance;
    }
    
    Queue<AnimationObject> cardQueue = new Queue<AnimationObject>();
    Queue<AnimationObject> shopQueue = new Queue<AnimationObject>();

    private bool cardAnimActive;

    public bool CardAnimActive
    {
        get => cardAnimActive;
    }

    private bool shopAnimActive;

    public bool ShopAnimActie
    {
        get => shopAnimActive;
    }

    void Awake()
    {
        if (_instance == null && _instance != this)
            _instance = this;
        else
        {
            Destroy(this.gameObject);
        }
    }

    #region QEUEING SYSTEM
    //TODO: Add a queue that allows only one animation to occur at a time 
    public void PlayAnimation(PlayerCardDisplay cardDisplay, Vector3 destination, float lerpTime, bool canScale = false, bool storeOriginalPosition = false, bool shouldDestroy = false, bool shouldQueue = true, bool isShopCard = false)
    {
        AnimationObject animObject = new AnimationObject(cardDisplay, destination, lerpTime, canScale, storeOriginalPosition, shouldDestroy);

        if (shouldQueue)
        {
            if(isShopCard)
                AddToShopQueue(animObject);
            else
                AddObjectToQueue(animObject);
        }
        else
        {
            StartCoroutine(TransformCardPosition(
                animObject.cardDisplay, animObject.destination, animObject.lerpTime,
                animObject.canScale, animObject.storeOriginalPosition,
                animObject.shouldDestroy, isShopCard
            ));
        }
       
    }

    void AddObjectToQueue(AnimationObject animObject)
    {
        if (cardQueue.Count == 0)
        {
            cardQueue.Enqueue(animObject);
            StartCoroutine(HandleAnim());
        }
        else
        {
            cardQueue.Enqueue(animObject);
        }
    }

    void AddToShopQueue(AnimationObject animObject)
    {
        if (shopQueue.Count == 0)
        {
            shopQueue.Enqueue(animObject);
            StartCoroutine(HandleAnim(isShopCard: true));
        }
        else
        {
            shopQueue.Enqueue(animObject);
        }
    }

    void EndCardEvent()
    {
        cardAnimActive = false;
    }

    void EndShopEvent()
    {
        shopAnimActive = false;
    }


    IEnumerator HandleAnim(bool isShopCard = false)
    {

        AnimationObject nextAnim = isShopCard ? shopQueue.Peek() : cardQueue.Peek();

        if (isShopCard)
            shopAnimActive = true;
        else
            cardAnimActive = true;

        StartCoroutine(TransformCardPosition(
            nextAnim.cardDisplay, nextAnim.destination, nextAnim.lerpTime,
            nextAnim.canScale, nextAnim.storeOriginalPosition, 
            nextAnim.shouldDestroy, isShopCard
            ));

       
        if (isShopCard)
        {
            while (shopAnimActive)
            {
                yield return null;
            }

            shopQueue.Dequeue();

            if (shopQueue.Count > 0)
            {
                StartCoroutine(HandleAnim(isShopCard: true));
            }
        }
        else
        {
            while (cardAnimActive)
            {
                yield return null;
            }

            cardQueue.Dequeue();

            if (cardQueue.Count > 0)
            {
                StartCoroutine(HandleAnim());
            }
        }
        
       

        

    }

    #endregion

    public IEnumerator TransformCardPosition(PlayerCardDisplay cardDisplay, Vector3 destination, float lerpTime, bool canScale, bool storeOriginalPosition, bool shouldDestroy, bool isShopCard, bool shouldEnd = true)
    {
        if (canScale)
        {
            var renderers = cardDisplay.GetComponentsInChildren<Renderer>();
            renderers.ToList().ForEach(x => x.enabled = false);
            cardDisplay.transform.localScale -= new Vector3(cardDisplay.transform.localScale.x, cardDisplay.transform.localScale.y, cardDisplay.transform.localScale.z);
            Debug.Log("Card Display Shrank: " + cardDisplay.transform.localScale);
        }
            

        BoxCollider2D touch = cardDisplay.GetComponent<BoxCollider2D>();
        touch.enabled = false;

        //Vector3 destination = GameObject.Find(destinationName).transform.position;

        float currentLerpTime = 0.0f;

        Vector3 startPos = cardDisplay.transform.position;

        while (cardDisplay.transform.position != destination)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime >= lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float Perc = currentLerpTime / lerpTime;

            cardDisplay.transform.position = Vector3.Lerp(startPos, destination, Perc);   

            yield return new WaitForEndOfFrame();
        }

        if (storeOriginalPosition)
        {
            cardDisplay.GetComponent<CardZoomer>().OriginalPosition = destination;
            cardDisplay.GetComponent<DragCard>().OriginalPosition = destination;
        }

        if (shouldEnd)
        {
            if (canScale && shouldDestroy)
                StartCoroutine(ScaleCardSize(cardDisplay, true));
            else if (canScale && !shouldDestroy)
                StartCoroutine(ScaleCardSize(cardDisplay, cardTouch: touch));
            else if (!canScale && shouldDestroy)
            {
                if (isShopCard)
                    EndShopEvent();
                else
                    EndCardEvent();
                Destroy(cardDisplay.gameObject);
            }
            else
            {
                touch.enabled = true;
                if (isShopCard)
                    EndShopEvent();
                else
                    EndCardEvent();
            }
        }
        else
        {
            Destroy(cardDisplay.gameObject);
        }
        


    }

    // This is called through a boolean 
    IEnumerator ScaleCardSize(PlayerCardDisplay cardDisplay, bool shouldDestroy = false, Collider2D cardTouch = null)
    {
        var renderers = cardDisplay.GetComponentsInChildren<Renderer>();
        renderers.ToList().ForEach(x => x.enabled = true);

        float currentLerpTime = 0.0f;
        float lerpTime = 0.2f;

        Vector3 startSize = cardDisplay.transform.localScale;
        Debug.Log("START SIZE: " + startSize);
        Vector3 targetSize = new Vector3(1.5f, 1.5f, 1.5f);

        while (cardDisplay.transform.localScale != targetSize)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime >= lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            float Perc = currentLerpTime / lerpTime;

            cardDisplay.transform.localScale = Vector3.Lerp(startSize, targetSize, Perc);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1.5f);

        if (!shouldDestroy)
        {
            cardTouch.enabled = true;
        }
        else
        {
            Destroy(cardDisplay.gameObject);
        }

        EndCardEvent();
    }

}

class AnimationObject
{
    public PlayerCardDisplay cardDisplay;
    public Vector3 destination;
    public float lerpTime;
    public bool canScale;
    public bool storeOriginalPosition;
    public bool shouldDestroy;
    public bool shouldQueue;

    public AnimationObject(PlayerCardDisplay cardDisplay, Vector3 destination, float lerpTime, bool canScale = false, bool storeOriginalPosition = false, bool shouldDestroy = false, bool shouldQueue = true)
    {
        this.cardDisplay = cardDisplay;
        this.destination = destination;
        this.lerpTime = lerpTime;
        this.canScale = canScale;
        this.storeOriginalPosition = storeOriginalPosition;
        this.shouldDestroy = shouldDestroy;
    }
}