using System.Collections.Generic;

namespace WPFToolbox.Condition
{
    /// <summary>
    /// Implementation of OR logical operator
    /// </summary>
    public class ConditionOr : List<ICondition>, ICondition
    {
        public bool Evaluate()
        {
            foreach (ICondition condition in this)
                if (condition.Evaluate())
                    return true;

            return false;
        }
    }
}
