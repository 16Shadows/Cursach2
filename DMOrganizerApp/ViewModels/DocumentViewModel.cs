using DMOrganizerModel.Interface.Content;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Windows;

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
                if (m_Document == value)
                    return;

                RecursiveUnsubscribe(m_Document);
                m_Document = value ?? throw new ArgumentNullException(nameof(Document));
                m_Document.TagAdded += Document_TagAdded;
                m_Document.TagRemoved += Document_TagRemoved;
                RecursiveSubscribe(m_Document);
                InvokePropertyChanged(nameof(Document));
            }
        }

        #region EventHandlers
        private void Document_TagRemoved(IDocument sender, TagOperationResultEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                MessageBox.Show("Removed");
            } 
            else
            {
                MessageBox.Show(e.ErrorText);
            }
        }

        private void Document_TagAdded(IDocument sender, TagOperationResultEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                MessageBox.Show("Added");
            }
            else if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.DuplicateValue)
            {
                MessageBox.Show("Duplicate tag");
            }    
            else
            {
                MessageBox.Show(e.ErrorText);
            }
        }

        private void Section_SectionDeleted(ISection sender, SectionDeletedEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                MessageBox.Show("Deleted");
            }
            else
            {
                MessageBox.Show(e.ErrorText);
            }
        }

        private void Section_Renamed(IItem sender, DMOrganizerModel.Interface.OperationResultEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                MessageBox.Show("Renamed");
            }
            else if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.DuplicateValue)
            {
                MessageBox.Show("Duplicate title");
            }    
            else
            {
                MessageBox.Show(e.ErrorText);
            }
        }

        private void Section_SectionCreated(ISection sender, SectionCreatedEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                MessageBox.Show("Created");
            }
            else if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.DuplicateValue)
            {
                MessageBox.Show("Duplicate title");
            }    
            else
            {
                MessageBox.Show(e.ErrorText);
            }
        }

        private void Section_ContentUpdated(ISection sender, DMOrganizerModel.Interface.OperationResultEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                MessageBox.Show("Updated");
            }
            else
            {
                MessageBox.Show(e.ErrorText);
            }
        }

        private void RecursiveSubscribe(ISection section)
        {
            section.ContentUpdated += Section_ContentUpdated;
            section.SectionCreated += Section_SectionCreated;
            section.Renamed += Section_Renamed;
            section.SectionDeleted += Section_SectionDeleted;
            
            foreach (ISection child in section.Children)
                RecursiveSubscribe(child);
        }
        private void RecursiveUnsubscribe(ISection section)
        {
            section.ContentUpdated -= Section_ContentUpdated;
            section.SectionCreated -= Section_SectionCreated;
            section.Renamed -= Section_Renamed;
            section.SectionDeleted -= Section_SectionDeleted;
            
            foreach (ISection child in section.Children)
                RecursiveUnsubscribe(child);
        }
        #endregion

        public DocumentViewModel(IDocument document)
        {
            m_Document = document ?? throw new ArgumentNullException(nameof(document));
            m_Document.TagAdded += Document_TagAdded;
            m_Document.TagRemoved += Document_TagRemoved;
            RecursiveSubscribe(m_Document);
        }

        public void AddTag(string text)
        {
            Document.AddTag(text);
        }

        public void RemoveTag(string text)
        {
            Document.RemoveTag(text);
        }

        public void EditTitle(ISection section, string title)
        {
            section.Rename(title);
        }

        public void EditContent(ISection section, string title)
        {
            section.UpdateContent(title);
        }

        public void AddSection(ISection parent, string title)
        {
            parent.CreateSection(title);
        }

        public void RemoveSection(ISection section)
        {
            section.Parent.DeleteSection(section);
        }


    }
}
