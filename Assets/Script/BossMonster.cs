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

    public GameObject Spell;
    public bool isAttack = false;

    protected EdgeCollider2D weapon;
    void Awake()
    {
        base.Awake();

        weapon = transform.Find("MonsterAttack").GetComponent<EdgeCollider2D>();
        weapon.enabled = false;

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
            if (currentState == State.Hit || currentState == State.Death)
                yield break;

            runTime -= Time.deltaTime;
            MyAnimSetTrigger("Walk");

            if (!isHit)
            {
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

        if (currentState != State.Attack || currentState != State.Skill)
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

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    IEnumerator Attack()
    {
        yield return null;

        canAtk = false;

        MyAnimSetTrigger("Attack");

        float attackDuration = 1f;
        float timer = 0f;
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            if (currentState == State.Hit || currentState == State.Death)
                yield break;

            yield return null;
        }

        

        currentState = State.Idle;
    }

    IEnumerator Skill()
    {
        yield return null;

        canAtk = false;

        MyAnimSetTrigger("Skill");

        float attackDuration = 1f;
        float timer = 0f;
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            if (currentState == State.Hit || currentState == State.Death)
                yield break;

            yield return null;
        }

        currentState = State.Idle;
    }

    void Cast()
    {
        Vector2 genPoint = new Vector2(GameManager.Instance.player.transform.position.x, transform.position.y);

        GameObject spellClone = Instantiate(Spell, genPoint, Quaternion.identity);
        if (spellClone != null)
        {
            Physics2D.IgnoreCollision(spellClone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
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
