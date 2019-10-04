﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

//using UnityEditorInternal;

/// <summary>
/// The container object for the Shop, which itself can contain PlayerCardContainers.
/// </summary>
public class ShopContainer : PlayerCardContainer
{
    private static ShopContainer _instance;

    // Singleton pattern
    public static ShopContainer Instance
    {
        get { return _instance; }
    }
    public ShopDeck shopDeck;


    public delegate void _cardDrawnLocationCreated(PlayerCardHolder cardDrawn, Vector3 freeSpot);

    public static event _cardDrawnLocationCreated CardDrawnLocationCreated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            //Debug.Log("\tDestroying ShopContainer");
            Destroy(this);
        }
        else
        {
            //Debug.Log("\tShopContainer Instance = this");
            _instance = this;
        }
    }

    /// <summary>
    /// Maximum number of cards in the shop at any given time.
    /// </summary>
    private int shopCardCount = 6;

    // Start is called before the first frame update
    void Start()
    {
        InitialCardDisplay();
    }

    private void OnEnable()
    {
        PurchaseHandler.CardPurchased += DisplayNewCard;
        
    }

    private void OnDisable()
    {
        PurchaseHandler.CardPurchased -= DisplayNewCard;
        
    }

    /// <summary>
    /// Initializes the Shop's card's placements.
    /// </summary>
    protected override void InitialCardDisplay()
    {
        for (int i = 0; i < shopCardCount; i++)
        {
            if (shopDeck.cardsInDeck.Count <= 0)
            {
                Debug.Log("Shop deck is " + shopDeck.cardsInDeck.Count);
                return;
            }

            HandleDisplayOfACard();

            // Draw a Card from the ShopDeck
        }

    }

    private void HandleDisplayOfACard()
    {
        PlayerCard cardDrawn = null;
        cardDrawn = (PlayerCard)shopDeck.cardsInDeck.Pop();

        // PlayerCardHolder
        holder.card = cardDrawn;

        PlayerCardHolder cardHolder = Instantiate(holder, containerGrid.freeLocations.Pop(), Quaternion.identity, this.transform);
        cardHolder.enabled = true;

        Vector3 freeSpot = cardHolder.gameObject.transform.position;
        
        CardDrawnLocationCreated?.Invoke(cardHolder, freeSpot);
        
        if (!containerGrid.cardLocationReference.ContainsKey(freeSpot))
            containerGrid.cardLocationReference.Add(freeSpot, cardHolder);
        else
            containerGrid.cardLocationReference[freeSpot] = cardHolder;
    }

    private void DisplayNewCard(PlayerCardHolder cardBought)
    {
        Vector3 freeSpot = cardBought.gameObject.transform.position;

        containerGrid.freeLocations.Push(freeSpot);
        HandleDisplayOfACard();
    }
}