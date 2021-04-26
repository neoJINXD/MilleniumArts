using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class CardMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AudioSource mouseEnter;
    
    private RectTransform rt;

    [SerializeField] private GameObject cardZoomInPanel;
    [SerializeField] private GameObject cardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rt = this.GetComponent<RectTransform>();
        cardZoomInPanel = GameplayUIManager.instance.cardZoomInPanel;
        cardPrefab = GameplayUIManager.instance.cardPrefab;

        mouseEnter = GameObject.FindWithTag("CardHover").GetComponent<AudioSource>();
    }

    // Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mouseEnter.Play();
        
        float hoverHeight = rt.rect.height * 0.05f;
        /*Top*/
        rt.offsetMax += new Vector2(0, hoverHeight);
        /*Bottom*/
        rt.offsetMin += new Vector2(0, hoverHeight);
        
        cardZoomInPanel.SetActive(true);

        UnitCard unitCard;
        SpellCard spellCard;

        if(transform.childCount > 0)
        {
            GameObject cardGO = Object.Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

            if (transform.GetChild(0).GetComponent<UnitCard>() != null)
            {
                unitCard = transform.GetChild(0).GetComponent<UnitCard>();
                cardGO.AddComponent(typeof(UnitCard));
                cardGO.GetComponent<UnitCard>().copyUnitCard(transform.GetChild(0).GetComponent<UnitCard>());
            }
            else if (transform.GetChild(0).GetComponent<SpellCard>() != null)
            {
                spellCard = transform.GetChild(0).GetComponent<SpellCard>();
                cardGO.AddComponent(typeof(SpellCard));
                cardGO.GetComponent<SpellCard>().copySpellCard(transform.GetChild(0).GetComponent<SpellCard>());
            }

            cardGO.transform.SetParent(cardZoomInPanel.transform);
            cardGO.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            cardGO.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            cardGO.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            cardGO.AddComponent(typeof(CardUI));
        }
    }

    private void OnDisable()
    {
        if(cardZoomInPanel != null)
            cardZoomInPanel.SetActive(false);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        
        if(transform.childCount > 0)
            Destroy(cardZoomInPanel.transform.GetChild(0).gameObject);

        cardZoomInPanel.SetActive(false);

        float hoverHeight = rt.rect.height * 0.05f;
        /*Top*/
        rt.offsetMax += new Vector2(0, -hoverHeight);
        /*Bottom*/
        rt.offsetMin += new Vector2(0, -hoverHeight);
    }
}
