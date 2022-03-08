using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager { get; private set; }

    public List<Stat> allyList; // �Ʊ� ĳ���͵� �� ������ ���� �� ����
    public List<Stat> enemyList; // 1���̺� �� ĳ���͵�

    public List<Stat> turnQueue = new List<Stat>();

    public Transform[] spawnArea; //�� �ε� �� �Ʊ� ĳ���� ��ȯ ��ġ
    public Transform[] anemySpawnArea; //�� �ε� �� �� ĳ���� ��ȯ ��ġ

    public float speed;
    public float limit = 100;
    public int maxPriorityCount = 400;

    Stat attacker;
    Stat victim;

    bool attacking = false;
    bool move = false;
    public float attackMoveSpeed = 1.0f;
    //[SerializeField]

    int allyArrayCount = 0;
    int enemyArrayCount = 0;

    Vector3[] returnPosition = new Vector3[40];
    int turnCount = 0;



    private void Awake()
    {
        if (battleManager == null)
        {
            battleManager = this;
        }
        else
        {
            Destroy(this);
        };
    }

    void Start()
    {
        SpeedSort();
        attacker = turnQueue[turnCount];


        for(int i =0; i<turnQueue.Count; i++)
        {
            returnPosition[i] = turnQueue[i].transform.position;
        }

    }


    void Update()
    {
        Battle();
    }

    void OnAttack(Stat attacker, Stat victim)
    {
        victim.TakeDamage(attacker.attackDamage);
    }

    void Dead(Stat victim)
    {
        if (victim.hp <= 0)
        {
            victim.gameObject.SetActive(false);
            Debug.Log($"{ victim.name} ���");
        }        
    }
    
    void TurnEnd()
    {
        ReturnPos();
        turnQueue[turnCount].speedStack -= 100;
        turnCount++;
        attacker = turnQueue[turnCount];
    }

    void AttackMove(Stat attacker, Stat victim)
    {
        float distance = (victim.transform.position - attacker.transform.position).magnitude;
        Vector2 direction = (victim.transform.position - attacker.transform.position).normalized;
        if (distance >= 2 && attacking == true)
        {
            attacker.transform.Translate(direction * attackMoveSpeed);
        }
        else
        {
            move = false;
        }
    }

    void ReturnPos()
    {
        attacker.transform.position = returnPosition[turnCount];
    }

    bool AnnihilationCheck()
    {
        if (allyList.Count == 0)
        {
            Debug.Log("�Ʊ� ���� ��������");
            return true;
        }
        else if (enemyList.Count == 0)
        {
            Debug.Log("���� ���� ��������");
            return true;
        }
        else 
        {
            return false;
        
        }
    }

    void Battle()
    {
        if (AnnihilationCheck())
        {
            return;
        }
        if (attacker.tag == "Enemy")
        {
            if (allyList[allyArrayCount].gameObject.activeSelf == false)
            {
                allyArrayCount++;               
            }
            victim = allyList[allyArrayCount];

        }
        else if (attacker.tag == "Ally")
        {
            if (enemyList[enemyArrayCount].gameObject.activeSelf == false)
            {
                enemyArrayCount++;
            }
            victim = enemyList[enemyArrayCount];
        }

        if (!attacking)
        {
            attacking = true;
            StartCoroutine("Attacking");
        }
        
        if(move)
        {
            AttackMove(attacker, victim);
        }
        
    }



    void SpeedSort()  //  ��������
    {
        turnQueue.Clear();  // ���� �� �ʱ�ȭ

        List<Stat> allCharList = new List<Stat>();  //���� �Ʊ� ����Ʈ ��ħ
        allCharList.AddRange(allyList);
        allCharList.AddRange(enemyList);


        float[] expectArray = new float[allCharList.Count];  // ���� ���� �迭

        for (int i = 0; i < expectArray.Length; i++)
        {
            expectArray[i] = allCharList[i].speedStack;  // ��� ĳ���� ���ǵ� ����ġ ����
        };

        while(turnQueue.Count< maxPriorityCount)
        {   
            int fastestTurn = allCharList[0].Priority(expectArray[0]); // ���� ���� ���� ĳ���� ��

            //���� ���� ���� ĳ������ �� �� ã��
            for (int i = 1; i < expectArray.Length; i++)
            {
                int currentPriority = allCharList[i].Priority(expectArray[i]);

                if (fastestTurn > currentPriority)
                {
                    fastestTurn = currentPriority;
                };
            };

            //���� ���� ĳ���Ͱ� 100 �̻� �� ������ ��� �ֵ� ��� ������
            for (int i = 1; i < expectArray.Length; i++)
            {
                expectArray[i] += fastestTurn * allCharList[i].speedWeight;
            };


            while (turnQueue.Count < maxPriorityCount)
            {
                float maxSpeed = expectArray[0];  // ���� ���� ĳ���� ����ġ
                int maxIndex = 0; //���� �迭�� ���� ����ĳ����

                for (int j = 0; j < expectArray.Length; j++)  //���� �迭 ���̸�ŭ ��� �ݺ�
                {
                    if (maxSpeed < expectArray[j]) // ���� ���� ĳ���� ���ǵ� ��
                    {
                        maxSpeed = expectArray[j]; // ���� �迭�� ���� ����ĳ������ ���ǵ� ����ġ ����
                        maxIndex = j; //���� �迭�� ���� ����ĳ������ �ε���
                    }
                }

                if (maxSpeed >= limit) //���� ���� ĳ���� ���ǵ� ����ġ�� 100 �̻��̸�
                {
                    turnQueue.Add(allCharList[maxIndex]);  //���� ���� ĳ���� ť�� �ְ�
                    
                    //Debug.Log(allCharList[maxIndex]);
                   
                    expectArray[maxIndex] -= limit; //���� ���� ĳ���� ���ǵ带 limit ��ŭ ����
                }
                else
                {
                    break;
                }
            }
        }
    }
    IEnumerator Attacking()
    {
        move = true;
        yield return new WaitForSeconds(turnQueue[turnCount].attack1DurationTime);
        OnAttack(attacker, victim);
        move = false; 
        attacking = false;
        TurnEnd();
    }


}
