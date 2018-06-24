using UnityEngine;
using System.Collections;
using System;

public class PilumsVolleyBehaviour : AExecutableBehaviour
{
    ConsumableStack stack;
    bool throwPilums = true;

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
                stack.Consumable = null;
            DisposeMe(this);
        }
    }

    public override void Behave()
    {
        if (stack.Consumable != null)
        {
            if (controller.AttackPlayer && throwPilums && controller.DistanceToPlayer <= ((ConsumablePilumsVolley.PilumsVolleyStats)stack.ConsumableStats).Distance)
            {
                stack.Consumable.Init(controller.ConstrolledSquad, controller.TargetSquad.CenterSquad, stack.Count);
                stack.Consumable.Execute(stack.ConsumableStats);
                throwPilums = false;
            }
            if (!controller.AttackPlayer)
            {
                throwPilums = true;
            }
        }
    }
}
