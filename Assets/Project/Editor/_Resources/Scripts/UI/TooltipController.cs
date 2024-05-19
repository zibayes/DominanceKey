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
            TankController tankController = selectManager.selectedArmy[0].tankController;
            for (int i = 0; i < tankController.crewAmount; i++)
            {
                PlayerController soldier = tankController.crew[i];
                if (soldier != null)
                {
                    if (soldier.placeInTank == index)
                    {
                        TooltipName.text = soldier.selection.personName;
                        TooltipSpecialization.text = soldier.selection.specialization;
                    }
                }
            }
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