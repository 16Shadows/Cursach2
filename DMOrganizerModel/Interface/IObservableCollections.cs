using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DMOrganizerModel.Interface
{

    public interface IObservableReadOnlyCollection<T> : IReadOnlyCollection<T>, INotifyCollectionChanged { }

    public interface IObservableCollection<T> : ICollection<T>, IObservableReadOnlyCollection<T> { }

    public interface IObservableReadOnlyList<T> : IReadOnlyList<T>, IObservableReadOnlyCollection<T>, INotifyCollectionChanged {}
    public interface IObservableList<T> :  IList<T>, IObservableCollection<T>, IObservableReadOnlyList<T> {}
}
