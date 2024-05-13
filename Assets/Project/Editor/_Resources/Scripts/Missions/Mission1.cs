using UnityEngine;

public class Mission1 : MonoBehaviour
{
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    public GameObject exitButton;
    public GameObject task;
    public SelectManager selectManager;
    public int playerSoldiersCount;
    public int enemySoldiersCount;

    void Start()
    {
        task.SetActive(true);
        Invoke("setTaskInactive", 5f);
    }

    void Update()
    {
        playerSoldiersCount = 0;
        enemySoldiersCount = 0;
        foreach (SelectableCharacter soldier in selectManager.selectableChars)
        {
            if (soldier.health > 0)
            {
                if (soldier.player == selectManager.player)
                {
                    playerSoldiersCount += 1;
                }
                else
                {
                    enemySoldiersCount += 1;
                }
            }   
        }
        if (playerSoldiersCount == 0)
        {
            defeatScreen.SetActive(true);
            exitButton.SetActive(true);
        }
        if (enemySoldiersCount == 0)
        {
            victoryScreen.SetActive(true);
            exitButton.SetActive(true);
        }
    }

    void setTaskInactive()
    {
        task.SetActive(false);
    }
}
