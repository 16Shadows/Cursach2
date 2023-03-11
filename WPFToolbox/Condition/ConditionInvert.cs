namespace WPFToolbox.Condition
{
    /// <summary>
    /// Implementation of NOT logical operator
    /// </summary>
    public class ConditionInvert : ICondition
    {
        public ICondition? Condition { get; set; }

        public bool Evaluate()
        {
            return Condition == null ? false : !Condition.Evaluate();
        }
    }
}