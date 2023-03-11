using System.Collections.Generic;

namespace WPFToolbox.Condition
{
    /// <summary>
    /// Implementation of OR logical operator
    /// </summary>
    public class ConditionAnd : List<ICondition>, ICondition
    {
        public bool Evaluate()
        {
            foreach (ICondition condition in this)
                if (!condition.Evaluate())
                    return false;

            return true;
        }
    }
}
