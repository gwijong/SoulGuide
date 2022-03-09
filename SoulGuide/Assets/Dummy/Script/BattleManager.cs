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
    public Queue<AnimationContainer> animQueue = new Queue<AnimationContainer>();

    Stat nextTurnCharacter;

    public Transform[] spawnArea; //�� �ε� �� �Ʊ� ĳ���� ��ȯ ��ġ
    public Transform[] enemySpawnArea; //�� �ε� �� �� ĳ���� ��ȯ ��ġ

    public float speedStackLimit = 100;
    public int maxPriorityCount = 10;

    int turnCount = 0;          //���� ���� �� �ϳ� �þ�� "���� �� �� * ����" �޴� �ֵ� �������� �α� �� ����

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
        //for(int i = 0; i < 8; i++)
        //{
        //    Attack(allyList[0], enemyList[0]);
        //};
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            List<Stat> allCharacter = new List<Stat>();
            allCharacter.AddRange(allyList);
            allCharacter.AddRange(enemyList);

            int fastestTurn = Stat.GetFastestTurn(allCharacter);

            for (int i = 0; i < allCharacter.Count; i++)
            {
                allCharacter[i].TurnPass(fastestTurn);
            };

            int fastestCharacterIndex = Stat.GetFastestCharacter(allCharacter);

            nextTurnCharacter = allCharacter[fastestCharacterIndex];

            Debug.Log(nextTurnCharacter);
            nextTurnCharacter.speedStack -= speedStackLimit;
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            SpeedSort();
        };

        if(animQueue.Count > 0)
        {
            AnimationContainer currentAnim = animQueue.Peek();
            
            if(currentAnim.Calculate())//�������� Ȯ��
            {
                animQueue.Dequeue();//����!
            };
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Attack(allyList[0], enemyList[0]);
                Debug.Log("1�� ����");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Attack(allyList[0], enemyList[1]);
                Debug.Log("2�� ����");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Attack(allyList[0], enemyList[2]);
                Debug.Log("3�� ����");
            };
        };
    }


    void TurnEnd()
    {
        turnCount++;
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
        };
    }

    void Battle()
    {
        if (AnnihilationCheck())
        {
            return;
        }
    }

    void Attack(Stat attacker, Stat victim)
    {
        victim.hp -= attacker.attackDamage;
        animQueue.Enqueue(new AnimationContainer(AnimationType.Move, 0.2f, attacker.transform, victim.transform.position + new Vector3(-1.0f, 0, 0)));
        animQueue.Enqueue(new AnimationContainer(AnimationType.Wait, 1.0f));

        if(victim.hp <= 0)
        {
            animQueue.Enqueue(new AnimationContainer(AnimationType.Die, 1.0f, victim.gameObject));
            enemyList.Remove(victim);
        };

        animQueue.Enqueue(new AnimationContainer(AnimationType.Move, 0.2f, attacker.transform, attacker.transform.position));
    }

    void SpeedSort()  // ������ ������ ������ ����Ǵ� ĳ���͵��� ���ϱ�
    {
        Debug.Log("------------------------------------");
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
            int fastestTurn = Stat.GetFastestTurn(allCharList, expectArray);

            //���� ���� ĳ���Ͱ� 100 �̻� �� ������ ��� �ֵ� ��� ������
            for (int i = 1; i < expectArray.Length; i++)
            {
                expectArray[i] += fastestTurn * allCharList[i].speedWeight;
            };

            while (turnQueue.Count < maxPriorityCount)
            {
                int maxIndex = Stat.GetFastestCharacter(allCharList, expectArray);

                if (expectArray[maxIndex] >= speedStackLimit) //���� ���� ĳ���� ���ǵ� ����ġ�� 100 �̻��̸�
                {
                    turnQueue.Add(allCharList[maxIndex]);  //���� ���� ĳ���� ť�� �ְ�
                    Debug.Log(allCharList[maxIndex]);
                    expectArray[maxIndex] -= speedStackLimit; //���� ���� ĳ���� ���ǵ带 limit ��ŭ ����
                }
                else
                {
                    break;
                };
            };
        };
    }



}
