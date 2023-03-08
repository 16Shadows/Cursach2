namespace DMOrganizerModel.Interface
{
    public delegate void TypedEventHandler<SenderType, ArgumentsType>(SenderType sender, ArgumentsType e);
    public delegate void TypedEventHandler<SenderType>(SenderType sender);
}
