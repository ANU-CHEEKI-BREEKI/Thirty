using UnityEngine;
using System.Collections;

public static class ExecutableBehaviourFactory
{
    public static AExecutableBehaviour GetBehaviour(AExecutableStack stack, AiSquadController controller)
    {
        if (stack is SkillStack)
            return GetBehaviourSkill(stack as SkillStack, controller);
        else if (stack is ConsumableStack)
            return GetBehaviourConsumable(stack as ConsumableStack, controller);
        else
            return null;
    }

    static AExecutableBehaviour GetBehaviourSkill(SkillStack stack, AiSquadController controller)
    {
        if (stack.Skill is SkillCharge)
            return new SkillChargeBehaviour(controller, stack);
        else if (stack.Skill is SkillArrowsValley)
            return new ArrowVolleyBehaviour(controller, stack);
        else
            return null;
    }

    static AExecutableBehaviour GetBehaviourConsumable(ConsumableStack stack, AiSquadController controller)
    {
        if (stack.Consumable is ConsumablePilumsVolley)
            return new PilumsVolleyBehaviour(controller, stack);

        else
            return null;
    }
}
