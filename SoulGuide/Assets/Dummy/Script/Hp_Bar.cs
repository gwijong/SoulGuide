using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hp_Bar : MonoBehaviour
{
    public Image hpBar;
    public Image backHpBar;
    public float yPos = -3;

    // Update is called once per frame
    void Update()
    {
        HpBar();
    }

    void HpBar()
    {
        hpBar.fillAmount = (float)gameObject.GetComponent<Stat>().hp / gameObject.GetComponent<Stat>().maxHp;
        hpBar.rectTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, yPos, 0));
        backHpBar.rectTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, yPos, 0));
    }
}
