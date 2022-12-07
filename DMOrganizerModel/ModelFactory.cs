using DMOrganizerModel.Interface.Model;
using DMOrganizerModel.Implementation.Model;

namespace DMOrganizerModel
{
    public static class ModelFactory
    {
        public static IModel CreateModel()
        {
            return new Model();
        }
    }
}
