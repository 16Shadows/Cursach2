using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System.Threading.Tasks;
using System.Collections.Generic;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface.References;
using DMOrganizerModel.Implementation.References;
using CSToolbox;

namespace DMOrganizerModel.Implementation.Items
{
    internal class Section : NamedContainerItem<ISection>, ISection, IReferenceableBase
    {
        public Section(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region ISection
        public WeakEvent<ISection, SectionContentChangedEventArgs> SectionContentChanged { get; } = new();
        public WeakEvent<ISection, SectionItemCreatedEventArgs> SectionItemCreated { get; } = new();

        protected void InvokeSectionContentChanged(string content, bool requested)
        {
            SectionContentChanged.Invoke(this, new SectionContentChangedEventArgs(content, requested));
        }

        protected void InvokeSectionItemCreated(string name, SectionItemCreatedEventArgs.ResultType result)
        {
            SectionItemCreated.Invoke(this, new SectionItemCreatedEventArgs(name, result));
        }

        public void CreateSection(string name)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                if (!NamingRules.IsValidName(name))
                {
                    InvokeSectionItemCreated(name, SectionItemCreatedEventArgs.ResultType.InvalidName);
                    return;
                }

                bool isUnique = false;
                ISection item = null;
                lock(Lock)
                {
                    isUnique = CanHaveItemWithName(name);
                    if (isUnique)
                        item = Organizer.GetSection(Query.CreateSection(Organizer.Connection, name, ItemID), this);
                }
                if (isUnique)
                {
                    InvokeSectionItemCreated(name, SectionItemCreatedEventArgs.ResultType.Success);
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<ISection>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<ISection>.ResultType.Success);
                }
                else
                    InvokeSectionItemCreated(name, SectionItemCreatedEventArgs.ResultType.DuplicateName);
            });
        }

        public void RequestSectionContentUpdate()
        {
            CheckDeleted();
            Task.Run(() => InvokeSectionContentChanged(Query.GetSectionContent(Organizer.Connection, ItemID), true));
        }

        public void UpdateContent(string content)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                Query.SetSectionContent(Organizer.Connection, ItemID, content);
                InvokeSectionContentChanged(content, false);
            });
        }

        public IReference GetReference()
        {
            return new Reference(this);
        }
        #endregion

        public override string GetName()
        {
            return Query.GetSectionName(Organizer.Connection, ItemID);
        }

        public override void SetName(string name)
        {
            _ = Query.SetSectionName(Organizer.Connection, ItemID, name);
        }

        protected override IEnumerable<ISection> GetContent()
        {
            List<ISection> res = new List<ISection>();
            foreach (int id in Query.GetSectionsInSection(Organizer.Connection, ItemID))
                res.Add(Organizer.GetSection(id, this));
            return res;
        }

        protected override bool CanBeParentOf(ISection item)
        {
            if (item is not Section sec)
                throw new ArgumentTypeException(nameof(item), "Invalid item type.");

            return CanHaveItemWithName(sec.GetName());
        }

        public override bool CanHaveItemWithName(string name)
        {
            return Query.HasNameInSection(Organizer.Connection, name, ItemID);
        }

        protected override bool HasItem(ISection item)
        {
            if (item is not Section sec)
                throw new ArgumentTypeException(nameof(item), "Invalid item type.");

            return Query.SectionHasSection(Organizer.Connection, sec.ItemID, ItemID);
        }

        protected override bool DeleteItemInternal()
        {
            return Query.DeleteSection(Organizer.Connection, ItemID);
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            if (parent is not Section sec)
                throw new ArgumentTypeException(nameof(parent), "Invalid parent type.");

            Query.SetSectionParent(Organizer.Connection, ItemID, sec.ItemID);
        }
    }
}