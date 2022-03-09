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
    public float[] attackDurationTime;

    private void Awake()
    {
  
        level = 1;  //레벨
        hp = 100;  //현재 HP
        maxHp = 100;  //최대 HP
        attackDamage = 50;  //공격력
        defense = 5; //방어력
        speedWeight = Random.Range(50, 70);  //속도 가중치
        speedStack = 0;  //100이 되기 전 스피드 누적치
        attackDurationTime = new float[3] { 1, 2, 3 };  //1번 공격이 진행되는 지속 시간
    }

    public void TurnPass(int amount)
    {
        speedStack += speedWeight * amount;
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

        return Mathf.CeilToInt((BattleManager.battleManager.speedStackLimit - wantStack) / speedWeight);
    }

    public int Priority() // 실제 계산할 때 쓸 예상치 오버로딩
    {
        return Mathf.CeilToInt((BattleManager.battleManager.speedStackLimit - speedStack) / speedWeight);
    }

    public static int GetFastestTurn(List<Stat> CharacterList, float[] expectArray = null)
    {
        //예상하려고 한 것이 아니라 지금 상태 그대로 알려줬으면 해서 준거라면?
        if(expectArray == null)
        {
            expectArray = new float[CharacterList.Count];
            for(int i = 0; i < expectArray.Length; i++)
            {
                expectArray[i] = CharacterList[i].speedStack;
            };
        };

        int fastestTurn = CharacterList[0].Priority(expectArray[0]); // 제일 빨리 도는 캐릭터 턴

        //제일 빨리 도는 캐릭터의 턴 수 찾기
        for (int i = 1; i < CharacterList.Count; i++)
        {
            int currentPriority = CharacterList[i].Priority(expectArray[i]);

            if (fastestTurn > currentPriority)
            {
                fastestTurn = currentPriority;
            };
        };

        return fastestTurn;
    }

    public static int GetFastestCharacter(List<Stat> CharacterList, float[] expectArray = null)
    {
        //예상하려고 한 것이 아니라 지금 상태 그대로 알려줬으면 해서 준거라면?
        if (expectArray == null)
        {
            expectArray = new float[CharacterList.Count];
            for (int i = 0; i < expectArray.Length; i++)
            {
                expectArray[i] = CharacterList[i].speedStack;
            };
        };

        float maxSpeed = expectArray[0];  // 가장 빠른 캐릭터 누적치
        int maxIndex = 0; //예상 배열의 가장 빠른캐릭터

        for (int j = 0; j < expectArray.Length; j++)  //예상 배열 길이만큼 뱅뱅 반복
        {
            if (maxSpeed < expectArray[j]) // 가장 빠른 캐릭터 스피드 비교
            {
                maxSpeed = expectArray[j]; // 예상 배열의 가장 빠른캐릭터의 스피드 누적치 대입
                maxIndex = j; //예상 배열의 가장 빠른캐릭터의 인덱스
            };
        };

        return maxIndex;               //제일 빠른 놈의 번호
        //return CharacterList[maxIndex]; //제일 빠른 놈
    }
}
