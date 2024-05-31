using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimerMonster : Monster
{
    public enum State
    {
        Idle,
        Run,
        Hit,
        Death,
    };

    public State currentState = State.Idle;


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
                case State.Idle:
                    yield return StartCoroutine(Idle());
                    break;
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

    IEnumerator Idle()
    {
        MyAnimSetTrigger("Idle");

        if (Random.value > 0.5f)
        {
            MonsterFlip();
        }
        yield return new WaitForSeconds(1f);

        currentState = State.Run;
    }

    IEnumerator Run()
    {
        float runTime = Random.Range(2f, 4f);
        while (runTime >= 0f)
        {
            if (currentState == State.Hit || currentState == State.Death)
                yield break;

            runTime -= Time.deltaTime;
            MyAnimSetTrigger("Run");
            if (!isHit)
            {
                Move();
            }
            yield return null;
        }

        currentState = State.Idle;
    }

    IEnumerator Hit()
    {
        MyAnimSetTrigger("Hit");

        yield return new WaitForSeconds(0.5f); // Hit 애니메이션 재생 시간
        if (Random.value > 0.5f)
        {
            currentState = State.Idle;
        }

        else
        {
            currentState = State.Run;
        }
    }

    IEnumerator Death()
    {
        MyAnimSetTrigger("Death");

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    public override void Move()
    {
        rb.velocity = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);

        if (MonsterDirLeft == true)
            moveDir = -0.5f;
        else
            moveDir = 0.5f;

        Vector2 currentPos = transform.position; // 현재 위치 기준
        Vector2 frontVec = new Vector2(currentPos.x + transform.localScale.x, currentPos.y); // 앞 방향
        Vector2 downVec = new Vector2(transform.position.x + moveDir, transform.position.y);

        Debug.DrawRay(frontVec, MonsterDirLeft ? Vector3.right : Vector3.left, new Color(0, 1, 0));
        Debug.DrawRay(downVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D front = Physics2D.Raycast(frontVec, MonsterDirLeft ? Vector3.right : Vector3.left, 1, LayerMask.GetMask("Platform"));

        if (front.collider != null) // 벽 방향전환
            MonsterFlip();


        RaycastHit2D down = Physics2D.Raycast(downVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (down.collider == null)
            MonsterFlip();
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

    protected void OnTriggerEnter2D(Collider2D collision) // 플레이어와 부딪히면 방향 전환
    {
        if (collision.transform.CompareTag("PlayerHitBox"))
        {
            MonsterFlip();
        }
    }

}
