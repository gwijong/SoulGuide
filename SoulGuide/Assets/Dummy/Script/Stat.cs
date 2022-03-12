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
  
        level = 1;  //����
        hp = 100;  //���� HP
        maxHp = 100;  //�ִ� HP
        attackDamage = 50;  //���ݷ�
        defense = 5; //����
        speedWeight = Random.Range(50, 70);  //�ӵ� ����ġ
        speedStack = 0;  //100�� �Ǳ� �� ���ǵ� ����ġ
        attackDurationTime = new float[3] { 1, 2, 3 };  //1�� ������ ����Ǵ� ���� �ð�
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
        //Stack�� wantStack�� ������ �������� ��!
        //�� �� �ڿ� ������ �� �ִ���!

        //Stack + (Weight * time) >= limit
        // Stack - limit >= -(Weight * time)
        //limit - Stack >= Weight * time
        //(limit - Stack) / Weight >= time

        return Mathf.CeilToInt((BattleManager.battleManager.speedStackLimit - wantStack) / speedWeight);
    }

    public int Priority() // ���� ����� �� �� ����ġ �����ε�
    {
        return Mathf.CeilToInt((BattleManager.battleManager.speedStackLimit - speedStack) / speedWeight);
    }

    public static int GetFastestTurn(List<Stat> CharacterList, float[] expectArray = null)
    {
        //�����Ϸ��� �� ���� �ƴ϶� ���� ���� �״�� �˷������� �ؼ� �ذŶ��?
        if(expectArray == null)
        {
            expectArray = new float[CharacterList.Count];
            for(int i = 0; i < expectArray.Length; i++)
            {
                expectArray[i] = CharacterList[i].speedStack;
            };
        };

        int fastestTurn = CharacterList[0].Priority(expectArray[0]); // ���� ���� ���� ĳ���� ��

        //���� ���� ���� ĳ������ �� �� ã��
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
        //�����Ϸ��� �� ���� �ƴ϶� ���� ���� �״�� �˷������� �ؼ� �ذŶ��?
        if (expectArray == null)
        {
            expectArray = new float[CharacterList.Count];
            for (int i = 0; i < expectArray.Length; i++)
            {
                expectArray[i] = CharacterList[i].speedStack;
            };
        };

        float maxSpeed = expectArray[0];  // ���� ���� ĳ���� ����ġ
        int maxIndex = 0; //���� �迭�� ���� ����ĳ����

        for (int j = 0; j < expectArray.Length; j++)  //���� �迭 ���̸�ŭ ��� �ݺ�
        {
            if (maxSpeed < expectArray[j]) // ���� ���� ĳ���� ���ǵ� ��
            {
                maxSpeed = expectArray[j]; // ���� �迭�� ���� ����ĳ������ ���ǵ� ����ġ ����
                maxIndex = j; //���� �迭�� ���� ����ĳ������ �ε���
            };
        };

        return maxIndex;               //���� ���� ���� ��ȣ
        //return CharacterList[maxIndex]; //���� ���� ��
    }
}
