using System;
using UnityEngine;

public enum AnimationType
{
    Move,
    Attack,
    Wait,
    Die
}

public class AnimationContainer
{
    public AnimationType type;      //���ϴ� �ִϸ��̼�
    public float timeMax;           //�ִϸ��̼��� ���� �ð�
    public float timeCurrent = 0;   //�ִϸ��̼��� �󸶳� ��������
    public object arg1;
    public object arg2;
    public object arg3;
    private Action animationFunction;   //�ִϸ��̼� ������Ʈ�� �Լ�
    private Action initialFunction;     //�ִϸ��̼� �ʱ�ȭ�� �Լ�
    private Action animationEnd;

    public AnimationContainer(AnimationType wantType, float wantTime, object wantArg1 = null, object wantArg2 = null, object wantArg3 = null)
    {
        type = wantType;
        switch(type)
        {
            case AnimationType.Wait:
                animationFunction = CalculateWait;
                break;
            case AnimationType.Move:
                animationFunction = CalculateMove;
                initialFunction = InitializeMove;
                break;
            case AnimationType.Die:
                animationEnd = EndDie;
                break;
            default: 
                animationFunction = null;
                break;
        };
        timeMax = wantTime;
        arg1 = wantArg1;
        arg2 = wantArg2;
        arg3 = wantArg3;
    }

    public bool Calculate()
    {
        //���� ���������ϱ�   �׸��� �ʱ�ȭ �Լ��� �־�!         �ʱ�ȭ!
        if (timeCurrent == 0 && initialFunction != null) initialFunction();

        timeCurrent += Time.deltaTime; //�󸶳� �������� ���� ���ؼ� �ð��� �־��ֱ�!

        if (timeCurrent >= timeMax || animationFunction == null)
        {
            if (animationEnd != null)
            {
                animationEnd();
            }
            return true;
        }
        
        animationFunction(); //�ִϸ��̼� ��� �����ϱ�


        return false; //���� �ȳ�����!
    }

    public void CalculateWait()
    {

    }

    public void InitializeMove()
    {
        arg3 = ((Transform)arg1).position;
    }

    public void CalculateMove()
    {
        //arg1 : �����̴� ���� Transform
        //arg2 : ������        Vector3
        //arg3 : ��������      Vector3
        float timeLeft = Mathf.Clamp((timeMax - timeCurrent) / timeMax, 0, 1); //���� �ð��� ���� (0 ~ 1)

        //���� �ð��� ũ�� Ŭ���� ���������� ������
        //���� �ð��� �������� �������� ������

        //                                            0 ~ 1                                 1 ~ 0
        // ĳ������ ��ġ              =    �������� * �����ð�              ������      *   (1 - �����ð�)        
        ((Transform)arg1).position = (((Vector3)arg3) * timeLeft) + (((Vector3)arg2) * (1 - timeLeft));
    }

    public void EndDie()  // ��� �ִϸ��̼��� �� ������
    {
        GameObject.Destroy((GameObject)arg1);
    }
}