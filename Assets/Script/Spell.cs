using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public bool colliderOn = false;
    public float lifetime = 1f;

    protected EdgeCollider2D skill;

    void Awake()
    {
        Destroy(gameObject, lifetime);

        skill = GetComponent<EdgeCollider2D>();
        skill.enabled = false;
    }


    public void SkillColliderOnOff()
    {
        if (colliderOn == false)
        {
            colliderOn = true;
            skill.enabled = true;
        }

        else
        {
            colliderOn = false;
            skill.enabled = false;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
