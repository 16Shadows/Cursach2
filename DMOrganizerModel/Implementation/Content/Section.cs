using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Content
{
    internal sealed class Section : SectionBase
    {
        #region Properties
        public int OrderIndex { get; }
        private readonly SectionBase m_Parent;
        public SectionBase Parent
        {
            get
            {
                CheckDisposed();
                return m_Parent;
            }
        }
        #endregion

        #region Constructors
        public Section(OrganizerModel organizer, SectionBase parent, string title, string content, int orderIndex, int itemID) : base(organizer, title, content, itemID)
        {
            m_Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            OrderIndex = orderIndex;
        }
        #endregion

        #region Methods
        public override StringBuilder GetPath(int len = 0)
        {
            return m_Parent.GetPath(len + Title.Length + 1).Append('#').Append(Title);
        }

        public override Task Rename(string name)
        {
            CheckDisposed();
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Task.Run(() =>
            {
                if (!StorageModel.IsValidTitle(name))
                {
                    InvokeRenamed(OperationResultEventArgs.ErrorType.InvalidArgument, $"The following title is not valid: {name}");
                    return;
                }

                lock (SyncRoot)
                {
                    string oldTitle = Title;
                    try
                    {
                        if (Parent.GetSection(name) != null)
                        {
                            InvokeRenamed(OperationResultEventArgs.ErrorType.DuplicateTitle, "A section with the same title is already present.");
                            return;
                        }
                        Title = name;
                        Organizer.ChangeSectionTitle(this, name);
                        InvokeRenamed(OperationResultEventArgs.ErrorType.None, null);
                    }
                    catch (Exception e)
                    {
                        Title = oldTitle;
                        InvokeRenamed(OperationResultEventArgs.ErrorType.InternalError, e.ToString());
                    }
                }
            });
        }
        #endregion
    }
}
