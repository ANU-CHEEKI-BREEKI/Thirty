using UnityEngine;
using System.Collections;
using System;

public class SkillChargeBehaviour : AExecutableBehaviour, IGizmosDrawable
{
    bool cooldown = false;
    SkillStack stack;

    public SkillChargeBehaviour(AiSquadController controller, SkillStack stack) : base(controller)
    {
        this.stack = stack;
    }
    
    public override void Behave()
    {
        if(controller.AttackPlayer)
        {
            //ПРИБЛИЗИТЕЛЬНО вычисляем дистанцию натиска
            var stats = ((SkillCharge.ChargeStats)stack.SkillStats);

            float dist = stats.duration * controller.ConstrolledSquad.CurrentSpeed;

            if (stats.modifyer.Speed.VType == UnitStatsModifier.Modifyer.ValueType.ADD)
                dist *= stats.modifyer.Speed.Value;
            else
                dist *= 1 + stats.modifyer.Speed.Value;

            float firstRowDistToPlayer = controller.DistanceToPlayer - controller.ConstrolledSquad.UnitCount / controller.ConstrolledSquad.SQUAD_LENGTH / 2;

            if (!cooldown && (firstRowDistToPlayer <= dist *0.7f && firstRowDistToPlayer >= dist * 0.3f))
            {
                stack.Skill.Init(controller.ConstrolledSquad);
                stack.Skill.Execute(stack.SkillStats);

                GameManager.Instance.StartCoroutine(Cooldown(stats.cooldown));
            }


        }
    }

    IEnumerator Cooldown(float duration)
    {
        cooldown = true;
        yield return new WaitForSeconds(duration);
        cooldown = false;
    }

    public void OnDrawGizmos()
    {
        var stats = ((SkillCharge.ChargeStats)stack.SkillStats);
        
        Gizmos.color = Color.red;

        float dist = stats.duration * controller.ConstrolledSquad.CurrentSpeed;

        if (stats.modifyer.Speed.VType == UnitStatsModifier.Modifyer.ValueType.ADD)
            dist *= stats.modifyer.Speed.Value;
        else
            dist *= 1 + stats.modifyer.Speed.Value;

        Gizmos.DrawWireSphere(
            controller.ConstrolledSquad.CenterSquad + (Vector2)controller.ConstrolledSquad.PositionsTransform.up * (controller.ConstrolledSquad.UnitCount / controller.ConstrolledSquad.SQUAD_LENGTH / 2),
           dist * 0.7f                
        );
        Gizmos.DrawWireSphere(
            controller.ConstrolledSquad.CenterSquad + (Vector2)controller.ConstrolledSquad.PositionsTransform.up * (controller.ConstrolledSquad.UnitCount / controller.ConstrolledSquad.SQUAD_LENGTH / 2),
            dist * 0.3f
        );
    }

}
