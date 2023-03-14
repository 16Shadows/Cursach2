﻿using CSToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.Items
{
    public class BookItemCreatedEventArgs : EventArgs
    {
        public enum ResultType
        {
            /// <summary>
            /// Successfully created
            /// </summary>
            Success,

            /// <summary>
            /// Some kind of failure
            /// </summary>
            Failure
        }
        // position of new or deleted page
        public int Position { get; }
        public ResultType Result { get; }

        public BookItemCreatedEventArgs(int position, ResultType result)
        {
            Position = position;
            Result = result;
        }
    }
    /// <summary>
    /// Book, container for pages.
    /// </summary>
    public interface IBook : IOrganizerItem, IItemContainer<IPage>
    {
        //Name, ID of parent category


        // create page (page will delete itself)
        // change pages order ?
        // change book name (INamedItem)
        // request book name (INamedItem)
        // request book pages (IContainerItem)
        // content change ivents (IContainerItem)
        WeakEvent<IBook, BookItemCreatedEventArgs> BookItemCreated { get; }

        //Need BookItemChangedEventArgs
        /// <summary>
        /// Creates page in book, at last position.
        /// </summary>
        void AddPage();

        //Need BookItemChangedEventArgs
        /// <summary>
        /// Creates page at given position, need to change all other pages' positions.
        /// </summary>
        /// <param name="position">Position for page in book</param>
        void AddPage(int position);

        //Need BookItemChangedEventArgs
        /// <summary>
        /// Removes page from book by position, deleting it and its content. Need to change all other pages' positions.
        /// </summary>
        void RemovePage(int position); //args?              надо ли это?

        //Need BookItemChangedEventArgs
        /// <summary>
        /// Changes position of child page, affecting db info and view. Need to change all other pages' positions.
        /// </summary>
        /// /// <param name="oldPosition">What page we're moving</param>
        /// /// <param name="newPosition">To what position we're moving</param>
        void ChangePagePosition(int oldPosition, int newPosition);
    }
}