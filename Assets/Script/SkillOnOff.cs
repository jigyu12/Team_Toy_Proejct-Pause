using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOnOff : MonoBehaviour
{
    public bool colliderOn = false;
    public bool spriteOn = false;

    protected SpriteRenderer spriteRenderer;
    protected EdgeCollider2D skill;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        skill = GetComponent<EdgeCollider2D>();

        spriteRenderer.enabled = false;
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

    public void SkillSpriteOnOff()
    {
        if (spriteOn == false)
        {
            spriteOn = true;
            spriteRenderer.enabled = true;
        }

        else
        {
            spriteOn = false;
            spriteRenderer.enabled = false;
        }
    }
}
