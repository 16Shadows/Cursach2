namespace DMOrganizerModel.Interface
{
    public class OperationResultEventArgs
    {
        /// <summary>
        /// Defines the possible error types
        /// </summary>
        public enum ErrorType
        {
            /// <summary>
            /// No error occured
            /// </summary>
            None,
            /// <summary>
            /// IModel.DecodeReference: Provided string-encoded reference is not a valid string-encoded reference
            /// </summary>
            InvalidReference,
            /// <summary>
            /// INavigationTreeCategory.CreateDocument: A document with the same title is already present in the category
            /// 
            /// </summary>
            DuplicateValue,
            /// <summary>
            /// 
            /// </summary>
            InvalidArgument,
            /// <summary>
            /// An unmanageable error occured within the model
            /// This type should always be accompanied with ErrorText
            /// </summary>
            InternalError
        }

        /// <summary>
        /// If the request fails, contains the error's type
        /// Otherwise, contains ErrorType.None
        /// </summary>
        public ErrorType Error { get; init; } = ErrorType.None;
        /// <summary>
        /// If the request fails, may contain text describing the error which caused this issue
        /// </summary>
        public string? ErrorText { get; init; } = null;
    }

    public delegate void OperationResultEventHandler<SenderType, ArgumentsType>(SenderType sender, ArgumentsType e);
    public delegate void OperationResultEventHandler<SenderType>(SenderType sender, OperationResultEventArgs e);
}
