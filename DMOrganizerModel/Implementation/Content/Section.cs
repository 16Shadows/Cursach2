using DMOrganizerModel.Implementation.Model;
using System;
using System.Text;

namespace DMOrganizerModel.Implementation.Content
{
    internal class Section : SectionBase
    {
        #region Properties
        #endregion

        #region Fields
        private readonly SectionBase m_Parent;
        #endregion

        #region Constructors
        public Section(OrganizerModel organizer, SectionBase parent, string title, string content) : base(organizer, title, content)
        {
            m_Parent = parent ?? throw new ArgumentNullException(nameof(parent));
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
