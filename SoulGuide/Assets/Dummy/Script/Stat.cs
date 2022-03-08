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
    public float attack1DurationTime;
    public float attack2DurationTime;
    public float attack3DurationTime;

    private void Awake()
    {
  
        level = 1;  //����
        hp = 100;  //���� HP
        maxHp = 100;  //�ִ� HP
        attackDamage = 50;  //���ݷ�
        defense = 5; //����
        speedWeight = Random.Range(50, 70);  //�ӵ� ����ġ
        speedStack = 0;  //100�� �Ǳ� �� ���ǵ� ����ġ
        attack1DurationTime = 1;  //1�� ������ ����Ǵ� ���� �ð�
        attack2DurationTime = 2;  //2�� ������ ����Ǵ� ���� �ð�
        attack3DurationTime = 3;  //3�� ������ ����Ǵ� ���� �ð�
    }

    public void TakeDamage(int attackDamage)
    {
        if (attackDamage > defense)
        {
            hp -= attackDamage - defense;
        }
        
        if(hp <= 0)
        {
            this.gameObject.SetActive(false);
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
