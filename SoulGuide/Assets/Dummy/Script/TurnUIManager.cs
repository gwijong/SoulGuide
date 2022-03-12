using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TurnUIManager : MonoBehaviour
{
    public Image[] turnImage = new Image[5];
    public Image thisTurnImage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BattleManager.battleManager.animQueue.Count > 0)
        {
            return;
        }
        thisTurnImage.sprite = BattleManager.battleManager.nextTurnCharacter.GetComponent<SpriteRenderer>().sprite;
        BattleManager.battleManager.SpeedSort();
        for(int i = 0; i<5; i++)
        {
            turnImage[i].sprite = BattleManager.battleManager.turnQueue[i].GetComponent<SpriteRenderer>().sprite;
        }
    }
}
