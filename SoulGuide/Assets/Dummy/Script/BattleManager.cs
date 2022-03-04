using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager { get; private set; }

    public GameObject[] allyCharacter;  // �Ʊ� ĳ���͵� �� ������ ���� �� ����
    public GameObject[] Wave1enemyCharacter;  // 1���̺� �� ĳ���͵�
    public GameObject[] Wave2enemyCharacter;  // 2���̺� �� ĳ���͵�
    public GameObject[] Wave3enemyCharacter;  // 3���̺� �� ĳ���͵�

    public GameObject[] spawnArea; //�� �ε� �� �Ʊ� ĳ���� ��ȯ ��ġ
    public GameObject[] anemySpawnArea; //�� �ε� �� �� ĳ���� ��ȯ ��ġ

    [SerializeField]
    private int turnCount = 1;
    private bool allyTurn = true;

    private void Awake()
    {
        if (battleManager == null)
        {
            battleManager = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        Battle();       
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0)&& allyTurn)
        {
            OnAttack(allyCharacter[turnCount], Wave1enemyCharacter[turnCount]);
            turnCount++;
        }else if (Input.GetMouseButtonDown(0) && !allyTurn)
        {
            OnAttack(Wave1enemyCharacter[turnCount- allyCharacter.Length], allyCharacter[turnCount- allyCharacter.Length]);
            turnCount++;
        }
        if (turnCount>= allyCharacter.Length && allyTurn)
        {
            allyTurn = false;
        }else if(turnCount >= allyCharacter.Length+Wave1enemyCharacter.Length && !allyTurn)
        {
            turnCount = 1;
            allyTurn = true;
        }
    }

    void Battle()
    {

    }

    public void OnAttack(GameObject attacker, GameObject victim)
    {
        if(attacker.activeSelf == false || victim.activeSelf == false)
        {
            return;
        }
        int damage = attacker.GetComponent<Stat>().Attack;
        int Hp = victim.GetComponent<Stat>().Hp;
        Hp = Hp - damage;
        victim.GetComponent<Stat>().Hp = Hp;
        Debug.Log
        ($"{attacker.name} ĳ���Ͱ� {victim.name} ĳ���͸� ����, \n" +
        $"������ ������{attacker.GetComponent<Stat>().Attack}, �ǰ��� ���� �����{victim.GetComponent<Stat>().Hp}");
        if (victim.GetComponent<Stat>().Hp <= 0)
        {
            victim.GetComponent<Stat>().Hp = 0;
            OnDead(victim);
        }
    }

    protected void OnDead(GameObject victim)
    {
        Debug.Log($"{victim.name} ĳ���� ���");
        victim.SetActive(false);
    }
}
