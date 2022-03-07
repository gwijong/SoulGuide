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

    Stat attacker;
    Stat victim;

    [SerializeField]
    private int turnCount = 0;
    private bool allyTurn = true;
    private bool mouseInputStop = false;

    public float speed;
    public float limit = 100;
    public int maxPriorityCount = 20;

    Vector3 returnPos;
    bool battleFinish = false;

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
    }


    void Update()
    {

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

    }

    
    void SpeedSort()  //  ���� �� ����
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
                    Debug.Log(allCharList[maxIndex]);
                    expectArray[maxIndex] -= limit; //���� ���� ĳ���� ���ǵ带 limit ��ŭ ����
                }
                else
                {
                    break;
                }
            }
        }

    }
}
