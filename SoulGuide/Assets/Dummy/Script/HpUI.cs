using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    public GameObject HpBarUI;
    public Image Hp;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HpBarUI.transform.position = transform.position;
        Hp.fillAmount = (float)gameObject.GetComponent<Stat>().hp / 100;
    }
}

