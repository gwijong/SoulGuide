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
  
        level = 1;  //레벨
        hp = 100;  //현재 HP
        maxHp = 100;  //최대 HP
        attackDamage = 50;  //공격력
        defense = 5; //방어력
        speedWeight = Random.Range(50, 70);  //속도 가중치
        speedStack = 0;  //100이 되기 전 스피드 누적치
        attack1DurationTime = 1;  //1번 공격이 진행되는 지속 시간
        attack2DurationTime = 2;  //2번 공격이 진행되는 지속 시간
        attack3DurationTime = 3;  //3번 공격이 진행되는 지속 시간
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
