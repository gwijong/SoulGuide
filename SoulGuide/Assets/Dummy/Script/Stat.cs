using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public int level;
    public int hp;
    public int maxHp;
    public int attackDamage;
    public int defense;
    public float speedWeight;
    public float speedStack;

    private void Awake()
    {
  
        level = 1;
        hp = 100;
        maxHp = 100;
        attackDamage = 50;
        defense = 5;
        speedWeight = Random.Range(50, 70);
        speedStack = 0;  //100�� �Ǳ� �� ���ǵ� ����ġ
        
    }

    public void TakeDamage(int attackDamage)
    {
        if (attackDamage > defense)
        {
            hp -= attackDamage - defense;
        }      
    }

    public int Priority(float wantStack)
    {
        //Stack�� wantStack�� ������ �������� ��!
        //�� �� �ڿ� ������ �� �ִ���!

        //Stack + (Weight * time) >= limit
        // Stack - limit >= -(Weight * time)
        //limit - Stack >= Weight * time
        //(limit - Stack) / Weight >= time

        return Mathf.CeilToInt((BattleManager.battleManager.limit - wantStack) / speedWeight);
    }

    public int Priority() // ���� ����� �� �� ����ġ �����ε�
    {
        return Mathf.CeilToInt((BattleManager.battleManager.limit - speedStack) / speedWeight);
    }
}
