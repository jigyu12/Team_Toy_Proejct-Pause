using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatMonster : Monster
{
    public enum State
    {
        Idle,
        Fly,
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
                case State.Fly:
                    yield return StartCoroutine(Fly());
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

        yield return new WaitForSeconds(1f);

        if (Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < 10f)
            currentState = State.Fly;
    }

    IEnumerator Fly()
    {
        float runTime = Random.Range(2f, 4f);
        while (runTime >= 0f)
        {
            if (currentState == State.Hit || currentState == State.Death)
                yield break;

            runTime -= Time.deltaTime;
            MyAnimSetTrigger("Fly");

            if (!isHit)
            {
                Move();

            }
            yield return null;
        }

        if (!IsPlayerDir())
        {
            MonsterFlip();
        }
    }

    IEnumerator Hit()
    {
        MyAnimSetTrigger("Hit");

        yield return new WaitForSeconds(0.5f); // Hit 애니메이션 재생 시간

        currentState = State.Fly;
    }

    IEnumerator Death()
    {
        MyAnimSetTrigger("Death");

        rb.gravityScale = 1;

        capsuleCollider.enabled = false;

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    public override void Move()
    {
        if (GameManager.Instance.player == null)
        {
            return;
        }

        if (IsPlayerDir() && Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
        {
            Vector2 direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
            //if (Random.value > 0.5f)
            //    rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
            //else
            //    rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * -moveSpeed);
            rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
        }

        Vector2 currentPos = transform.position; // 현재 위치 기준

        Vector2 frontVec = new Vector2(currentPos.x + transform.localScale.x, currentPos.y); // 앞 방향
        Vector2 downVec = new Vector2(transform.position.x, transform.position.y); // 아래 방향
        Vector2 upVec = new Vector2(transform.position.x, transform.position.y); // 위 방향

        Debug.DrawRay(frontVec, MonsterDirLeft ? Vector3.right : Vector3.left, new Color(0, 1, 0));
        Debug.DrawRay(downVec, Vector3.down, new Color(0, 1, 0));
        Debug.DrawRay(upVec, Vector3.up, new Color(0, 1, 0));

        RaycastHit2D front = Physics2D.Raycast(frontVec, MonsterDirLeft ? Vector3.right : Vector3.left, 1, LayerMask.GetMask("Platform"));
        RaycastHit2D down = Physics2D.Raycast(downVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        RaycastHit2D up = Physics2D.Raycast(upVec, Vector3.up, 1, LayerMask.GetMask("Platform"));

        if (down.collider != null)
            rb.velocity = new Vector2(rb.velocity.x, moveSpeed);

        if (up.collider != null)
            rb.velocity = new Vector2(rb.velocity.x, -moveSpeed);

        if (front.collider != null)
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
}
