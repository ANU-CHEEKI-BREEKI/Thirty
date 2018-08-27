using UnityEngine;
using System.Collections;
using System;

public class ArrowVolleyBehaviour : AExecutableBehaviour, IGizmosDrawable
{
    bool cooldown = false;
    SkillStack stack;

    public ArrowVolleyBehaviour(AiSquadController controller, SkillStack stack) : base(controller)
    {
        this.stack = stack;
    }
    
    public override void Behave()
    {
        if (!cooldown && controller.DistanceToPlayer < controller.DistancesOptions.RadiusOfAttackArea && 
                         controller.DistanceToPlayer > controller.DistancesOptions.RadiusOfDefendArea)
        {
            //ПРИБЛИЗИТЕЛЬНО вычисляем положение отряда после предупредительного залпа
            var targetPos = GerFirePos();

            stack.Skill.Init(controller.ConstrolledSquad, targetPos, controller.ConstrolledSquad.Rotation);
            stack.Skill.Execute(stack.SkillStats);

            GameManager.Instance.StartCoroutine(
                Cooldown(((SkillArrowsValley.ArrowWalleyStats)stack.Stats).cooldown)
            );

            Debug.Log("ArrowVolleyBehaviour:  player in radius of fire");
            Debug.Log("ArrowVolleyBehaviour:  executed");
            Debug.Log("ArrowVolleyBehaviour:  target pos  -  " + targetPos);
        }
    }


    Vector2 GerFirePos()
    {
        var squad = controller.TargetSquad;
        var pos = squad.CenterSquad;
        
        if (squad.IsMoving)
        {
            //ПРИБЛИЗИТЕЛЬНО вычисляем положение отряда после предупредительного залпа
            var speed = squad.CurrentSpeed;
            pos = speed * squad.Direction * ((stack.SkillStats as ISkillDelayable).Delay + 2f) + pos;
        }

        return pos;
    }

    IEnumerator Cooldown(float duration)
    {
        cooldown = true;
        yield return new WaitForSeconds(duration);
        cooldown = false;
    }

    Vector2 gizmosTargetPos;
    public void OnDrawGizmos()
    {
        if (!cooldown)
        {
            //ПРИБЛИЗИТЕЛЬНО вычисляем положение отряда после предупредительного залпа
            gizmosTargetPos = GerFirePos();
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = new Color(1, 0.5f, 1);
        }

        Gizmos.DrawWireSphere(gizmosTargetPos, (stack.SkillStats as ISkillRadiusable).Radius);
    }

}
