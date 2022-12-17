using DMOrganizerModel.Interface.Content;
using System;

namespace DMOrganizerApp.ViewModels
{
    internal class DocumentViewModel : BaseViewModel

    {
        private IDocument m_Document;
        public IDocument Document
        {
            get => m_Document;
            set
            {
                m_Document = value ?? throw new ArgumentNullException(nameof(Document));
                InvokePropertyChanged(nameof(Document));
            }
        }

        public DocumentViewModel(IDocument document)
        {
            m_Document = document ?? throw new ArgumentNullException(nameof(document));
        }
    }
}
