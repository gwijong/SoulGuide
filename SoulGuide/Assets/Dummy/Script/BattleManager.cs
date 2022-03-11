using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager { get; private set; }

    public List<Stat> allyList; // �Ʊ� ĳ���͵� �� ������ ���� �� ����
    public List<Stat> enemyList; // 1���̺� �� ĳ���͵�


    public List<Stat> turnQueue = new List<Stat>();
    public Queue<AnimationContainer> animQueue = new Queue<AnimationContainer>();

    public Stat nextTurnCharacter;  //���� ���� ������ ĳ����

    public Transform[] spawnArea; //�� �ε� �� �Ʊ� ĳ���� ��ȯ ��ġ
    public Transform[] enemySpawnArea; //�� �ε� �� �� ĳ���� ��ȯ ��ġ

    public float speedStackLimit = 100; //�ӵ������Ѱ�ġ�� ���� ���� ���� �� ��ġ
    public int maxPriorityCount = 10; //�켱���� �ִ����

    int turnCount = 0;          //���� ���� �� �ϳ� �þ�� "���� �� �� * ����" �޴� �ֵ� �������� �α� �� ����
    public Stat target;
    public Text text;
    bool gameStartWait = true;

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
        sort();
        StartCoroutine("GameStart");

    }

    private void Update()
    {
        if (gameStartWait)
        {
            return;
        }

        TextOutput();


        if(animQueue.Count > 0)  //�̵��̳� ���� �ִϸ��̼��� ���������� üũ
        {
            AnimationContainer currentAnim = animQueue.Peek();
            
            if(currentAnim.Calculate())//�������� Ȯ��
            {
                animQueue.Dequeue();//����!
            };
        }
        else 
        {
            
            AnnihilationCheck();
            if (AnnihilationCheck())
            {
                return;
            }
            if (nextTurnCharacter.tag == "Ally" && Input.GetMouseButtonDown(0))
            {
                Touch();
                if (target.tag=="Enemy")
                {
                    Attack(nextTurnCharacter, target);
                    sort();
                }else if (target == null)
                {
                    Attack(nextTurnCharacter, enemyList[0]);
                    sort();
                }
            }
            else if (nextTurnCharacter.tag == "Enemy")
            {
                int allyAttackTarget = UnityEngine.Random.Range(0, allyList.Count);
                Attack(nextTurnCharacter, allyList[allyAttackTarget]);               
                sort();
            }
        };
    }

    void sort()
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
        if (allCharacter[fastestCharacterIndex] == null)
        {
            allCharacter.Remove(allCharacter[fastestCharacterIndex]);
            sort();
        }
            nextTurnCharacter = allCharacter[fastestCharacterIndex];

            //Debug.Log(nextTurnCharacter);
            nextTurnCharacter.speedStack -= speedStackLimit;
        
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

    
    void Attack(Stat attacker, Stat victim)
    {
        victim.hp -= attacker.attackDamage;
        if(attacker.tag == "Ally")
        {
            animQueue.Enqueue(new AnimationContainer(AnimationType.Move, 0.2f, attacker.transform, victim.transform.position + new Vector3(-1.0f, 0, 0)));
        }
        else if(attacker.tag == "Enemy")
        {
          
            animQueue.Enqueue(new AnimationContainer(AnimationType.Move, 0.2f, attacker.transform, victim.transform.position + new Vector3(1.0f, 0, 0)));
        }
        else
        {
            Debug.Log("���ݺκ� ����");
        }
        
        animQueue.Enqueue(new AnimationContainer(AnimationType.Wait, 1.0f));

        if(victim.hp <= 0)
        {
            animQueue.Enqueue(new AnimationContainer(AnimationType.Die, 1.0f, victim.gameObject));
            if (victim.tag == "Ally")
            {
                allyList.Remove(victim);
            }
            else
            {
                enemyList.Remove(victim);
            }
            
        };
        animQueue.Enqueue(new AnimationContainer(AnimationType.Move, 0.2f, attacker.transform, attacker.transform.position));
    }

    //�Ʒ� ���� �޼���� ȭ�� �����ʿ� �� ĳ���ͺ� �� ���� �����Ȳ�� �����ֱ� ���� �ڵ��, ���ӿ� ���� ������ ��ġ�� �ڵ尡 �ƴ�
    void SpeedSort()  // ������ ������ ������ ����Ǵ� ĳ���͵��� ���ϱ�
    {
        //Debug.Log("------------------------------------");
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
                    //Debug.Log(allCharList[maxIndex]);
                    expectArray[maxIndex] -= speedStackLimit; //���� ���� ĳ���� ���ǵ带 limit ��ŭ ����
                }
                else
                {
                    break;
                };
            };
        };

    }
    void Touch()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider == null)
        {
            target = enemyList[0];
            return;
        }
        if(hit.collider.tag == "Enemy")
        {
            target = hit.transform.GetComponent<Stat>();
        }

    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1.5f);
        gameStartWait = false;
    }


    void TextOutput()
    {
        if (animQueue.Count > 0)
        {
            return;
        }
        if (nextTurnCharacter.tag == "Ally")
        {
            text.text = $"�Ʊ� {nextTurnCharacter.name} ��";
        }
        else if (nextTurnCharacter.tag == "Enemy")
        {
            text.text = $"���� {nextTurnCharacter.name} ��";
        }
        if (AnnihilationCheck())
        {
            text.text = $"���� ����";
        }
    }
}
