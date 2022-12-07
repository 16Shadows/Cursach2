using System.Collections.Generic;
using System.Collections.Specialized;

namespace DMOrganizerModel.Interface
{
    /// <summary>
    /// Introduces a ICollection<T>-based interface which also provides notifications about changes
    /// </summary>
    /// <typeparam name="T">Type of the collection</typeparam>
    public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged, IReadOnlyCollection<T> { }

    /// <summary>
    /// Introduces a IList<T>-based interface which also provides notifications about changes
    /// </summary>
    /// <typeparam name="T">Type of the list</typeparam>
    public interface IObservableList<T> :  IList<T>, IObservableCollection<T>, IReadOnlyList<T> {}
}
