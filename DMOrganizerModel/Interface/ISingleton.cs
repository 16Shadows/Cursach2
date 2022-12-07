namespace DMOrganizerModel.Interface
{
    public interface ISingleton<T>
    {
        static T Instance { get; }
    }
}
