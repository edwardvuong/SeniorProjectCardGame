﻿using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

/// <summary>
/// Holds visual information specific to minion cards. Extends EnemyCardDisplay.
/// Class will load in text, sprites, card art, and values into the card display
/// this loads depending on the card attached to it. 
/// </summary>
//[ExecuteInEditMode]
public class MinionCardDisplay : EnemyCardDisplay<MinionCard>
{
    //private byte currentCardIdenrifier = (byte)'E';

    //Delegate event to handle anything that cares if the card has been destroyed
    public static event Action<MinionCardDisplay> CardDestroyed;

    //Delegate event to handle anything that cares if the card has been clicked
    public static event Action<MinionCardDisplay> MinionCardClicked;

    public Action MinionCardSummoned;

    public Action MinionCardDestroyed; 

    void Awake()
    {
        
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        TurnPhaseManager.BattlePhaseStarted += EnableBoxCollider;
        TurnPhaseManager.BattlePhaseEnded += DisableBoxCollider;
        Event_Base.DisableMinionCards += DisableBoxCollider;
        MinionCardSummoned?.Invoke();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TurnPhaseManager.BattlePhaseStarted -= EnableBoxCollider;
        TurnPhaseManager.BattlePhaseEnded -= DisableBoxCollider;
        Event_Base.DisableMinionCards += DisableBoxCollider;
    }

    //This method will invoke the CardDestroyed event 
    protected override void OnDestroy()
    {
        Debug.Log("minion card was destroyed");
        base.OnDestroy();
        CardDestroyed?.Invoke(this);
    }

    //This method will invoke the MinionCardClicked event
    protected override void OnMouseDown()
    {
        MinionCardClicked?.Invoke(this);
        if(!PhotonNetwork.OfflineMode)
            photonView.RPC("RPCAttackMinion", RpcTarget.Others, this.photonView.ViewID);
    }

    [PunRPC]
    private void RPCAttackMinion(int cardID)
    {
        PhotonView foundCard = PhotonView.Find(cardID);
        if (foundCard)
        {
            MinionCardDisplay purchasedCard = foundCard.GetComponent<MinionCardDisplay>();
            MinionCardClicked?.Invoke(this);
            Debug.Log("Minion Attacked");
        }
        else
        {
            Debug.Log("Photon View not found. CardID: " + cardID);
        }
    }


}
