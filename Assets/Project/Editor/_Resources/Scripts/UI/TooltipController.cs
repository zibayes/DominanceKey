using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SelectManager selectManager;
    public GameObject toolTip;
    public int index;

    void Awake()
    {
        toolTip.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.SetActive(true);
        GameObject.Find("TooltipName").GetComponent<TextMeshProUGUI>().text = selectManager.selectedArmy[index].personName;
        GameObject.Find("TooltipSpecialization").GetComponent<TextMeshProUGUI>().text = selectManager.selectedArmy[index].specialization;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
    }
}