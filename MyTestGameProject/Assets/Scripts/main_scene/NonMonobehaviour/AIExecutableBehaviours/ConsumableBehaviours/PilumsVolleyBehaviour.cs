using UnityEngine;
using System.Collections;
using System;

public class PilumsVolleyBehaviour : AExecutableBehaviour
{
    ConsumableStack stack;
    bool throwPilums = true;
    bool cooldown = false;

    public PilumsVolleyBehaviour(AiSquadController controller, ConsumableStack stack) : base(controller)
    {
        this.stack = stack;
        stack.Consumable.CallbackUsedCount += Volley_CallbackUsedCount;
    }

    private void Volley_CallbackUsedCount(int cnt, Squad owner)
    {
        if (owner == controller.ConstrolledSquad)
        {
            stack.Count -= cnt;
            if (stack.Count <= 0)
            {
                stack.Consumable = null;
                DisposeMe(this);
            }
        }
    }

    public override void Behave()
    {
        if (stack.Consumable != null)
        {
            float throwDistance = ((ConsumablePilumsVolley.PilumsVolleyStats)stack.ConsumableStats).Distance * 0.7f;
            float rhowAgainDistance = ((ConsumablePilumsVolley.PilumsVolleyStats)stack.ConsumableStats).Distance * 0.65f;

            if (!cooldown && throwPilums && controller.DistanceToPlayer <= throwDistance && controller.DistanceToPlayer <= controller.DistancesOptions.RadiusOfDefendArea)
            {
                throwPilums = false;

                stack.Consumable.Init(controller.ConstrolledSquad, controller.TargetSquad.CenterSquad, stack.Count);
                stack.Consumable.Execute(stack.ConsumableStats);
                
                GameManager.Instance.StartCoroutine(Cooldown(((ConsumablePilumsVolley.PilumsVolleyStats)stack.ConsumableStats).Cooldown));
            }

            if (!cooldown && controller.DistanceToPlayer > rhowAgainDistance)
            {
                throwPilums = true;
            }
        }
    }

    IEnumerator Cooldown(float duration)
    {
        cooldown = true;
        yield return new WaitForSeconds(duration);
        cooldown = false;
    }
}
