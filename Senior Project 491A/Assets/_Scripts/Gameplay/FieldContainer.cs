﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldContainer : Container
{
    public EnemyCardDisplay display;
    public EnemyDeck enemyDeck;

    private void OnEnable()
    {
        MinionCardDisplay.CardDestroyed += AddFreeCardLocation;
        UIHandler.EndTurnClicked += DisplayACard;
    }

    private void OnDisable()
    {
        MinionCardDisplay.CardDestroyed -= AddFreeCardLocation;
        UIHandler.EndTurnClicked -= DisplayACard;
    }

    public void DisplayACard()
    {
        if(enemyDeck.cardsInDeck.Count <= 0)
        {
            //Debug.Log("Enemy deck is " + enemyDeck.cardsInDeck.Count);
            //Debug.Log("Enemy Deck is empty");
            return;
        }
        
        if (containerGrid.freeLocations.Count == 0)
        {
            //Debug.Log("Field Zone is full");
            return;
        }

        Vector2 freeLocation = containerGrid.freeLocations.Pop();
        //Debug.Log(containerGrid.GetNearestPointOnGrid(new Vector2(0, 3.5f)));
        if (freeLocation == containerGrid.GetNearestPointOnGrid(new Vector2(8, 2)))
        {
            //Debug.Log("Card is in boss zone");
            DisplayACard();
            return;
        }
        
        if (containerGrid.cardLocationReference.ContainsKey(freeLocation))
        {
            CreateInstanceOfCard(freeLocation);
        }
        else
        {
            CreateInstanceOfCard(freeLocation);
        }
    }

    public void CreateInstanceOfCard(Vector2 freeLocation)
    {


        EnemyCard cardDrawn = null;
        cardDrawn = (EnemyCard)enemyDeck.cardsInDeck.Pop();

        display.card = cardDrawn;
        EnemyCardDisplay cardDisplay = Instantiate(display, freeLocation, Quaternion.identity, this.transform);
        
        if (!containerGrid.cardLocationReference.ContainsKey(freeLocation))
        {
            containerGrid.cardLocationReference.Add(new Vector2(cardDisplay.gameObject.transform.position.x, 
                cardDisplay.gameObject.transform.position.y), cardDisplay);
        }
        else
        {
            containerGrid.cardLocationReference[freeLocation] = cardDisplay;
        }
        

    }

    void AddFreeCardLocation(EnemyCardDisplay cardDestroyed)
    {
        Vector2 cardLocation = cardDestroyed.gameObject.transform.position;
        //Debug.Log("Card destroyed adding free location");
        containerGrid.freeLocations.Push(cardLocation);
        
        //containerGrid.cardLocationReference[cardLocation] = null;
        
    }
    
    
}
