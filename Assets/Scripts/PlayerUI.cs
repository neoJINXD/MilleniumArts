using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Player m_player;
    [SerializeField] private Image m_placeButton;
    [SerializeField] private GameObject m_endTurnButton;

    private void Update()
    {
        if (GameLoop.instance.GetCurrentPlayer().PlayerId == m_player.PlayerId)
            ActivePlayerUI();
        else
            InactivePlayerUI();
    }

    private void ActivePlayerUI()
    {
        m_placeButton.color = Color.white;
        m_endTurnButton.SetActive(true);
    }

    private void InactivePlayerUI()
    {
        m_placeButton.color = Color.grey;
        m_endTurnButton.SetActive(false);
    }

    public void EndTurnButton()
    {
        m_player.EndTurn();
    }

    public void PlayCard(int cardIndex)
    {
        m_player.PlayCard(cardIndex);
    }
}
