﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OnDeathPlayPlayerCardEffect : OnDeathCardEffects
{
    public override void LaunchCardEffect()
    {
        Deck playerDeck = TurnManager.Instance.turnPlayer.deck;
        Graveyard playerGrave = TurnManager.Instance.turnPlayer.graveyard;

        if (playerDeck.cardsInDeck.Count != 0)
        {
            PlayerCard playerCard = (PlayerCard)playerDeck.cardsInDeck.Pop();
            TurnManager.Instance.turnPlayer.Power += playerCard.CardAttack;
        }
        else
        {
            if (playerGrave.graveyard.Count != 0)
            {
                for (int j = 0; j < playerGrave.graveyard.Count; j++)
                {
                    playerDeck.cardsInDeck.Push(playerGrave.graveyard[j]);
                    playerGrave.graveyard.Remove(playerGrave.graveyard[j]);
                }

                playerDeck.cardsInDeck = ShuffleDeck.Shuffle(playerDeck);

                PlayerCard playerCard = (PlayerCard)playerDeck.cardsInDeck.Pop();
                TurnManager.Instance.turnPlayer.Power += playerCard.CardAttack;
            }
            else
                NotificationWindowEvent.Instance.EnableNotificationWindow(
                    "deck & grave is empty nothing to destroy");
        }    
    }
}
