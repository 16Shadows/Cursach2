namespace WPFToolbox.Condition
{
    /// <summary>
    /// Provides an object-based method to define logical conditions
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Evalutes this condition
        /// </summary>
        /// <returns>The result of evaluating this condition</returns>
        bool Evaluate();
    }
}