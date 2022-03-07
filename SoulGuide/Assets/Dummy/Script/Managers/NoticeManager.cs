using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NoticeManager : MonoBehaviour
{
    static NoticeManager manager;

    public Image NoticeImage;
    public Text NoticeText;
    string start = "모든 젤리를 해금하는 것이 목표.";
    string clear = "모든 젤리를 해금했어요!";
    string sell = "젤리를 드래그해서 주머니에 놓아 팔 수 있어요.";
    string notGelatin = "젤라틴이 부족합니다.";
    string notGold = "골드가 부족합니다.";
    string notNum = "젤리 수용량이 부족합니다";

    float timer;
    bool timeCheck = false;

    bool isNegative = true;

    void Awake()
    {
        if(manager==null) manager = this;
    }

    private void Start()
    {
        GameManager.update.UpdateMethod -= OnUpdate;
        GameManager.update.UpdateMethod += OnUpdate;
    }

    public static void Msg(string Name)
    {
        switch (Name)
        {
            case "start":
                manager.NoticeText.text = manager.start;
                manager.isNegative = false;
                break;
            case "clear":
                manager.NoticeText.text = manager.clear;
                manager.isNegative = false;
                break;
            case "sell":
                manager.NoticeText.text = manager.sell;
                manager.isNegative = false;
                break;
            case "notGelatin":
                manager.NoticeText.text = manager.notGelatin;
                manager.isNegative = true;
                break;
            case "notGold":
                manager.NoticeText.text = manager.notGold;
                manager.isNegative = true;
                break;
            case "notNum":
                manager.NoticeText.text = manager.notNum;
                manager.isNegative = true;
                break;
            default:
                manager.NoticeText.text = Name;
                manager.isNegative = true;
                break;
        }
        manager.timer = 0;
        manager.timeCheck = true;
        manager.NoticeImage.rectTransform.anchoredPosition = new Vector2(manager.NoticeImage.rectTransform.anchoredPosition.x, -3);
    }

    void OnUpdate()
    {
        timer = timer + Time.deltaTime;
        if (isNegative)
        {
            NoticeImage.GetComponent<Image>().color = new Color(255 / 255f, 100 / 255f, 100 / 255f, 255 / 255f);
            NoticeText.color = new Color(255, 255, 255, 255);
        }
        else
        {
            NoticeImage.GetComponent<Image>().color = new Color(0 / 255f, 234 / 255f, 218 / 255f, 255 / 255f);
            NoticeText.color = new Color(0, 0, 0, 255);
        }

        if (timeCheck)
        {
            if (timer > 3)
            {
                NoticeImage.rectTransform.anchoredPosition = new Vector2(NoticeImage.rectTransform.anchoredPosition.x, 20);
                timeCheck = false;
            }
        }
    }
}
