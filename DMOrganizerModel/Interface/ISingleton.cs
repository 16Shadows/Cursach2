namespace DMOrganizerModel.Interface
{
    #pragma warning disable 8618
    public interface ISingleton<T>
    {
        static T Instance { get; }
    }
}
