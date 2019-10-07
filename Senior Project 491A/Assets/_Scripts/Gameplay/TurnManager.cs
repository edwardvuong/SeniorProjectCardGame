﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
        // Player References
    public Player turnPlayer;

    public GameObject turnPlayerGameObject;

    [SerializeField]
    private GameObject player1GameObject;

    [SerializeField]
    private GameObject player2GameObject;
    

    public delegate void _PlayerSwitched();

    public static event _PlayerSwitched PlayerSwitched;

    [SerializeField]
    private Player player1;

    [SerializeField]
    private Player player2;

    private static TurnManager _instance;

    public static TurnManager Instance
    {
        get { return _instance; }
    }

    private void OnEnable()
    {
        UIHandler.EndTurnClicked += ShowHidePanel;
    }

    private void OnDisable()
    {
        UIHandler.EndTurnClicked -= ShowHidePanel;
    }


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        player2GameObject.SetActive(false);
        turnPlayer = player1;
        turnPlayerGameObject = player1GameObject;

    }

    private void Start()
    {
        PlayerSwitched?.Invoke();
    }

    public void ShowHidePanel()
    {
        if (player1GameObject != null && player2GameObject != null)
        {
            turnPlayer.Currency = 0;
            turnPlayer.Power = 0;
            
            // Switch to Player Two
            if (player1GameObject.activeSelf)
            {
                player1GameObject.SetActive(false);
                player2GameObject.SetActive(true);
                //turnManager.SetPlayerTwosTurn();
                turnPlayer = player2;
                turnPlayerGameObject = player2GameObject;
            }
            // Switch to Player One
            else if (player2GameObject.activeSelf)
            {
                player2GameObject.SetActive(false);
                player1GameObject.SetActive(true);
                //turnManager.SetPlayerOnesTurn();

                turnPlayer = player1;
                turnPlayerGameObject = player1GameObject;
            }
            
            PlayerSwitched?.Invoke();

            TextUpdate.Instance.UpdateCurrency();
            TextUpdate.Instance.UpdatePower();
        }
    }
}
