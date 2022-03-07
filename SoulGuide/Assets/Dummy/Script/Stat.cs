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
        speedStack = 0;  //100이 되기 전 스피드 누적치
        
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
        //Stack이 wantStack인 것으로 가정했을 때!
        //몇 턴 뒤에 공격할 수 있는지!

        //Stack + (Weight * time) >= limit
        // Stack - limit >= -(Weight * time)
        //limit - Stack >= Weight * time
        //(limit - Stack) / Weight >= time

        return Mathf.CeilToInt((BattleManager.battleManager.limit - wantStack) / speedWeight);
    }

    public int Priority() // 실제 계산할 때 쓸 예상치 오버로딩
    {
        return Mathf.CeilToInt((BattleManager.battleManager.limit - speedStack) / speedWeight);
    }
}
