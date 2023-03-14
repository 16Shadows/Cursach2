using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
/*using DMOrganizerModel.Interface.Model;

namespace DMOrganizerModel.Implementation.Model
{
    public static class StorageModel
    {
        #region Properties
        public static readonly char[] ForbiddenNameSymbols = { '/', '#', '$', '\n', '\t', '[', ']' };
        public static readonly char[] ReserverTextSymbols = { '[', ']' };
        #endregion

        #region Fields
        private static readonly Dictionary<string, OrganizerModel> m_Organizers;
        private static readonly object m_SyncRoot;
        #endregion

        #region Constructors
        static StorageModel()
        {
            m_Organizers = new Dictionary<string, OrganizerModel>();
            m_SyncRoot = new object();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads an organizer instance for use.
        /// Must be unloaded by UnloadOrganizer later.
        /// </summary>
        /// <param name="identifier">Implementation-dependent identifier for organizer. This implementation uses path to a file on the disk</param>
        /// <returns>An instance of an organizer</returns>
        /// <exception cref="Exception">Can throw implementation-dependent exception if loading fails</exception>
        public static IOrganizer LoadOrganizer(string identifier)
        {
            lock (m_SyncRoot)
            {
                if (identifier == null)
                    throw new ArgumentNullException(nameof(identifier));

                identifier = identifier.ToLower();
                OrganizerModel? result;
                if (m_Organizers.TryGetValue(identifier, out result))
                    return result;

                result = new OrganizerModel(identifier);
                m_Organizers[identifier] = result;
                return result;
            }
        }

        /// <summary>
        /// Unloads an organizer from memory, releasing all associated resources
        /// </summary>
        /// <param name="organizer">The organizer to unload</param>
        /// <exception cref="ArgumentException">Thrown when organizer was not created by LoadOrganizer of this instance</exception>
        public static void UnloadOrganizer(IOrganizer organizer)
        {
            lock (m_SyncRoot)
            {
                OrganizerModel? model = organizer as OrganizerModel;
                if (model == null)
                    throw new ArgumentException("Organizer was not created by this StorageModel", nameof(organizer));

                if (!m_Organizers.ContainsKey(model.Identifier))
                    throw new ArgumentException("Organizer was not created by this StorageModel", nameof(organizer));

                m_Organizers.Remove(model.Identifier);
                model.Dispose();
            }
        }

        public static bool IsValidTitle(string title)
        {
            if (title == null)
                throw new ArgumentNullException(nameof(title));

            if (title.Length < 1 || title[0] == ' ' || title[title.Length - 1] == ' ')
                return false;

            return title.Length > 0 && title.IndexOfAny(ForbiddenNameSymbols) == -1;
        }

        public static string SanitizeTitle(string title)
        {
            StringBuilder result = new StringBuilder(title.Length);

            int i = 0,
                j = title.Length - 1;

            for (i = 0; i < title.Length && (title[i] == ' ' || title[i] == '\t' ||  title[i] == '\n'); i++);
            for (i = 0; j >= 0 && (title[i] == ' ' || title[i] == '\t' ||  title[i] == '\n'); j++);

            if (i > j)
                return "";

            for (; i <= j; i++)
            {
                if (!ForbiddenNameSymbols.Contains(title[i]))
                    result.Append(title[i]);
            }

            return result.ToString();
        }
        #endregion
    }
}*/