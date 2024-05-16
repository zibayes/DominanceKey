using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SelectManager selectManager;
    public GameObject toolTip;
    public TextMeshProUGUI TooltipName;
    public TextMeshProUGUI TooltipSpecialization;
    public int index;

    void Awake()
    {
        toolTip.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.SetActive(true);
        
        if (selectManager.selectedArmy.Count == 1 && selectManager.selectedArmy[0].tankController != null)
        {
            TooltipName.text = selectManager.selectedArmy[0].tankController.crew[index].selection.personName;
            TooltipSpecialization.text = selectManager.selectedArmy[0].tankController.crew[index].selection.specialization;
        }
        else
        {
            TooltipName.text = selectManager.selectedArmy[index].personName;
            TooltipSpecialization.text = selectManager.selectedArmy[index].specialization;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.SetActive(false);
    }
}