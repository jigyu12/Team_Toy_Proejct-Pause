using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleMonster : Monster
{
    public enum State
    {
        Fly,
        Attack,
        Hit,
        Death,
    };

    public State currentState = State.Fly;


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
                case State.Fly:
                    yield return StartCoroutine(Fly());
                    break;
                case State.Attack:
                    yield return StartCoroutine(Attack());
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

    IEnumerator Fly()
    {
        float runTime = Random.Range(2f, 4f);
        while (runTime >= 0f)
        {
            runTime -= Time.deltaTime;
            MyAnimSetTrigger("Fly");
            if (!isHit)
            {
                Move();

                if (canAtk && IsPlayerDir())
                {
                    if (Random.value > 0.7f)
                    {
                        if (Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
                        {
                            currentState = State.Attack;
                            yield break;
                        }
                    }
                }
            }
            yield return null;
        }

        if (currentState != State.Attack && !IsPlayerDir())
        {
            MonsterFlip();
        }
    }

    IEnumerator Attack()
    {
        yield return null;

        canAtk = false;

        MyAnimSetTrigger("Attack");

        float attackSpeed = 10f;

        Vector2 direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * attackSpeed, direction.y * attackSpeed);

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        currentState = State.Fly;
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
        if (IsPlayerDir() && Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
        {
            Vector2 direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
            if (Random.value > 0.5f)
                rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
            else
                rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * -moveSpeed);
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
