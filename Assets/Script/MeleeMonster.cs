using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeleeMonster : Monster
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
        moveSpeed = 2f;
        jumpPower = 5f;

        StartCoroutine(FSM());
    }


    IEnumerator FSM()
    {
        while (true)
        {
            Debug.Log("Current State: " + currentState);
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
            Move();
        }
        yield return null;
    }

    IEnumerator Hit()
    {
        Debug.Log("Entering Hit state");

        MyAnimSetTrigger("Hit");

        yield return new WaitForSeconds(0.5f); // Hit 애니메이션 재생 시간

        currentState = State.Run;
    }

    IEnumerator Death()
    {
        Debug.Log("Entering Death state");
        MyAnimSetTrigger("Death");

        // Death 애니메이션 재생 시간만큼 대기
        yield return new WaitForSeconds(2f);

        // 죽음 처리 로직 (예: 오브젝트 삭제)
        Destroy(gameObject);
    }

    //void FixedUpdate()
    //{

    //    if (!isHit)
    //    {
    //        Move();
    //    }

    //    if (currentHp <= 0)
    //        MyAnimSetTrigger("Death");
    //}

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


    protected void OnTriggerEnter2D(Collider2D collision) // 플레이어와 부딪히면 방향 전환
    {
        base.OnTriggerEnter2D(collision);
        if (collision.transform.CompareTag("PlayerHitBox"))
        {
            Debug.Log("PlayerHitBox hit, flipping monster");
            MonsterFlip();
            Debug.Log("Monster flipped");
        }
    }


}
