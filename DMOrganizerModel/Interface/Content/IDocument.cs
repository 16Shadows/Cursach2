using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.Content
{
    public class TagOperationResultEventArgs : OperationResultEventArgs
    {
        public TagOperationResultEventArgs(string tag)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        }

        public string Tag { get; }
    }

    public interface IDocument : ISection
    {
        /// <summary>
        /// Tags of this document
        /// </summary>
        IObservableReadOnlyCollection<string> Tags { get; }
                
        Task AddTag(string tag);
        
        event OperationResultEventHandler<IDocument, TagOperationResultEventArgs>? TagAdded;

        Task RemoveTag(string tag);

        event OperationResultEventHandler<IDocument, TagOperationResultEventArgs>? TagRemoved;
    }
}
