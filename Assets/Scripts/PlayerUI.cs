using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image m_placeButton;
    [SerializeField] private GameObject m_endTurnButton;
    
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

    public void PlayTestCard()
    {
        TurnManager.instance.placingEnemyUnit = true;
    }
    
    //TODO: read m_player cards and hide update card UI, likely on update
}