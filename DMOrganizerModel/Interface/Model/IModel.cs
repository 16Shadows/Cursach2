using DMOrganizerModel.Interface.Reference;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.Model;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Interface.Model
{
    public interface IModel
    {
        IReference CreateReference(ISection section);
        IReference DecodeReference(string reference);
        INavigationTreeCategory GetNavigationTree();
    }
}
