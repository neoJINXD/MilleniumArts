using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        rt = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        float hoverHeight = rt.rect.height * 0.05f;
        /*Top*/
        rt.offsetMax += new Vector2(0, hoverHeight);
        /*Bottom*/
        rt.offsetMin += new Vector2(0, hoverHeight);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        float hoverHeight = rt.rect.height * 0.05f;
        /*Top*/
        rt.offsetMax += new Vector2(0, -hoverHeight);
        /*Bottom*/
        rt.offsetMin += new Vector2(0, -hoverHeight);
    }
}
