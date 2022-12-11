using DMOrganizerModel.Implementation.Model;
using System;
using System.Text;

namespace DMOrganizerModel.Implementation.Content
{
    internal class Section : SectionBase
    {
        #region Properties
        public int OrderIndex { get; }
        #endregion

        #region Fields
        private readonly SectionBase m_Parent;
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

        public override bool Rename(string name)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateContent(string text)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
