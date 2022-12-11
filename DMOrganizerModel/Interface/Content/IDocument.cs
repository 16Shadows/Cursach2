using DMOrganizerModel.Interface.NavigationTree;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.Content
{
    public interface IDocument : ISection
    {
        /// <summary>
        /// Tags of this document
        /// </summary>
        IObservableReadOnlyCollection<string> Tags { get; }
        
        public Task AddTag(string tag);
        
        public Task RemoveTag(string tag);
    }
}
