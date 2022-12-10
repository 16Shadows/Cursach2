using DMOrganizerModel.Interface;
using System.Collections.ObjectModel;

namespace DMOrganizerModel.Implementation
{
    /// <summary>
    /// An implementation of IObservableList<T>
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    internal class ObservableList<T> : ObservableCollection<T>, IObservableList<T> { }
}
