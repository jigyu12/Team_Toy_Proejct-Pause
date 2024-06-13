using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class Skeleton : Monster
{
    public enum State
    {
        Run,
        Hit,
        Death,
    };

    public State currentState = State.Run;

    private void Awake()
    {
        base.Awake();

        atkCoolTime = 3f;
        atkCoolTimeCalc = atkCoolTime;

        StartCoroutine(FSM());
    }


    IEnumerator FSM()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Run:
                    yield return StartCoroutine(Run());
                    break;
                case State.Hit:
                    yield return StartCoroutine(Hit());
                    break;
                case State.Death:
                    yield return StartCoroutine(Death());
                    break;
            }
        }
    }

    IEnumerator Run()
    {
        if (!isHit)
        {
            MyAnimSetTrigger("Run");
            Move();
        }
        yield return null;
    }

    IEnumerator Hit()
    {
        MyAnimSetTrigger("Hit");

        yield return new WaitForSeconds(0.5f); // Hit 애니메이션 재생 시간

        currentState = State.Run;
    }

    IEnumerator Death()
    {
        MyAnimSetTrigger("Death");

        monsterHitBox.enabled = false;
        // Death 애니메이션 재생 시간만큼 대기
        yield return new WaitForSeconds(1f);

        // 죽음 처리 로직 (예: 오브젝트 삭제)
        Destroy(gameObject);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (currentHp <= 0)
        {
            currentState = State.Death;
        }
        else
        {
            currentState = State.Hit;
        }
    }

}
