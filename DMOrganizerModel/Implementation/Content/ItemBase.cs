﻿using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Text;

namespace DMOrganizerModel.Implementation.Content
{
    internal abstract class ItemBase : OrganizerEntryBase, IItem
    {
        #region Properties
        public string Title { get; protected set; }
        public int ItemID { get; }
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;
        #endregion

        #region Constructors
        public ItemBase(OrganizerModel organizer, string title, int itemID) : base(organizer)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            ItemID = itemID;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Is used to efficently construct path to this instance
        /// Specific implementation depends on the object (Section or Document)
        /// </summary>
        /// <param name="len">The number of characters required by previous steps of the algorithm to construct full path</param>
        /// <returns></returns>
        public abstract StringBuilder GetPath(int len = 0);
        public abstract bool Rename(string name);
        #endregion
    }
}
