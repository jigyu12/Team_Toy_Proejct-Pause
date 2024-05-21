using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BossMonster : Monster
{
    public enum State
    {
        Idle,
        Walk,
        Attack,
        Skill,
        Hit,
        Death,
    };

    public State currentState = State.Idle;

    public bool isAttack = false;

    protected int count = 0;
    protected EdgeCollider2D weapon;
    protected Animator spellAnimator;
    void Awake()
    {
        base.Awake();

        weapon = transform.Find("MonsterAttack").GetComponent<EdgeCollider2D>();
        weapon.enabled = false;

        spellAnimator = transform.Find("MonsterSkill").GetComponent<Animator>();

        atkCoolTime = 3f;
        atkCoolTimeCalc = atkCoolTime;

        StartCoroutine(FSM());
    }

    //void FixedUpdate()
    //{
    //    GroundCheck();
    //}

    IEnumerator FSM()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Idle:
                    yield return StartCoroutine(Idle());
                    break;
                case State.Walk:
                    yield return StartCoroutine(Walk());
                    break;
                case State.Attack:
                    yield return StartCoroutine(Attack());
                    break;
                case State.Skill:
                    yield return StartCoroutine(Skill());
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
        
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        currentState = State.Walk;
    }

    IEnumerator Walk()
    {
        float runTime = Random.Range(2f, 4f);
        while (runTime >= 0f)
        {
            runTime -= Time.deltaTime;

            if (!isHit)
            {
                MyAnimSetTrigger("Walk");

                Move();

                if (canAtk && IsPlayerDir()) // Attack, Skill
                {
                    if (Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < 15f)
                    {
                        int nextState = Random.Range(0, 2);

                        if (nextState == 0)
                        {
                            currentState = State.Skill;
                            yield break;
                        }

                        else
                        {
                            currentState = State.Attack;
                            yield break;
                        }
                    }
                }
            }
            yield return null;
        }

        if (currentState != State.Attack)
        {
            currentState = Random.value >= 0.5f ? State.Idle : State.Walk;
            MonsterFlip();

            if (MonsterDirLeft)
                transform.Translate(-3, 0, 0);

            else
                transform.Translate(3, 0, 0);
        }

    }


    IEnumerator Hit()
    {
        MyAnimSetTrigger("Hit");

        yield return new WaitForSeconds(0.5f); // Hit 애니메이션 재생 시간

        currentState = State.Idle;
    }

    IEnumerator Death()
    {
        MyAnimSetTrigger("Death");

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    IEnumerator Attack()
    {
        yield return null;

        canAtk = false;

        MyAnimSetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        currentState = State.Idle;
    }

    IEnumerator Skill()
    {
        yield return null;

        canAtk = false;

        MyAnimSetTrigger("Skill");

        spellAnimator.SetTrigger("Spell");

        yield return new WaitForSeconds(1f);

        currentState = State.Idle;

        spellAnimator.SetTrigger("Idle");
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

    public override void Move()
    {
        rb.velocity = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);

        Vector2 currentPos = transform.position; // 현재 위치 기준
        Vector2 frontVec = new Vector2(currentPos.x + transform.localScale.x, currentPos.y + 2f); // 앞 방향


        Debug.DrawRay(frontVec, MonsterDirLeft ? Vector3.right : Vector3.left, new Color(0, 1, 0));

        RaycastHit2D front = Physics2D.Raycast(frontVec, MonsterDirLeft ? Vector3.right : Vector3.left, 1, LayerMask.GetMask("Platform"));

        if (front.collider != null)
        {
            MonsterFlip();

            if (MonsterDirLeft)
                transform.Translate(-3, 0, 0);

            else
                transform.Translate(3, 0, 0);
        }
    }

    public void AttackColliderOnOff()
    {
        if (isAttack == false)
        {
            isAttack = true;
            weapon.enabled = true;
        }

        else
        {
            isAttack = false;
            weapon.enabled = false;
        }
    }
}
