using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager { get; private set; }

    public List<Stat> allyList; // 아군 캐릭터들 앞 오른쪽 왼쪽 뒤 순서
    public List<Stat> enemyList; // 1웨이브 적 캐릭터들


    public List<Stat> turnQueue = new List<Stat>();
    public Queue<AnimationContainer> animQueue = new Queue<AnimationContainer>();

    public Stat nextTurnCharacter;  //지금 때릴 현재턴 캐릭터

    public Transform[] spawnArea; //씬 로드 시 아군 캐릭터 소환 위치
    public Transform[] enemySpawnArea; //씬 로드 시 적 캐릭터 소환 위치

    public float speedStackLimit = 100; //속도스택한계치가 가득 차면 공격 턴 배치
    public int maxPriorityCount = 10; //우선순위 최대숫자

    int turnCount = 0;          //턴이 끝날 때 하나 늘어나서 "지난 턴 수 * 버프" 받는 애들 전용으로 두긴 할 거임
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


        if(animQueue.Count > 0)  //이동이나 공격 애니메이션이 진행중인지 체크
        {
            AnimationContainer currentAnim = animQueue.Peek();
            
            if(currentAnim.Calculate())//끝났는지 확인
            {
                animQueue.Dequeue();//삭제!
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
            Debug.Log("아군 전멸 전투종료");
            return true;
        }
        else if (enemyList.Count == 0)
        {
            Debug.Log("적군 전멸 전투종료");
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
            Debug.Log("공격부분 에러");
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

    //아래 정렬 메서드는 화면 오른쪽에 각 캐릭터별 턴 예상 진행상황을 보여주기 위한 코드로, 게임에 직접 영향을 미치는 코드가 아님
    void SpeedSort()  // 다음에 공격할 것으로 예상되는 캐릭터들을 구하기
    {
        //Debug.Log("------------------------------------");
        turnQueue.Clear();  // 기존 턴 초기화

        List<Stat> allCharList = new List<Stat>();  //적과 아군 리스트 합침
        allCharList.AddRange(allyList);
        allCharList.AddRange(enemyList);


        float[] expectArray = new float[allCharList.Count];  // 예상 스택 배열

        for (int i = 0; i < expectArray.Length; i++)
        {
            expectArray[i] = allCharList[i].speedStack;  // 모든 캐릭터 스피드 누적치 대입
        };

        while(turnQueue.Count< maxPriorityCount)
        {
            int fastestTurn = Stat.GetFastestTurn(allCharList, expectArray);

            //가장 빠른 캐릭터가 100 이상 될 때까지 모든 애들 뱅뱅 돌려줌
            for (int i = 1; i < expectArray.Length; i++)
            {
                expectArray[i] += fastestTurn * allCharList[i].speedWeight;
            };

            while (turnQueue.Count < maxPriorityCount)
            {
                int maxIndex = Stat.GetFastestCharacter(allCharList, expectArray);

                if (expectArray[maxIndex] >= speedStackLimit) //제일 빠른 캐릭터 스피드 누적치가 100 이상이면
                {
                    turnQueue.Add(allCharList[maxIndex]);  //제일 빠른 캐릭터 큐에 넣고
                    //Debug.Log(allCharList[maxIndex]);
                    expectArray[maxIndex] -= speedStackLimit; //제일 빠른 캐릭터 스피드를 limit 만큼 빼줌
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
            text.text = $"아군 {nextTurnCharacter.name} 턴";
        }
        else if (nextTurnCharacter.tag == "Enemy")
        {
            text.text = $"적군 {nextTurnCharacter.name} 턴";
        }
        if (AnnihilationCheck())
        {
            text.text = $"전투 종료";
        }
    }
}
