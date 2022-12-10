using System;
using System.Collections.Generic;

using DMOrganizerModel.Interface.Model;

namespace DMOrganizerModel.Implementation.Model
{
    public sealed class StorageModel
    {
        #region Properties
        public static StorageModel Instance { get; }
        public static readonly char[] ForbiddenNameSymbols = { '/', '#', '$' };
        #endregion

        #region Fields
        private readonly Dictionary<string, OrganizerModel> m_Organizers;
        #endregion

        #region Constructors
        static StorageModel()
        {
            Instance = new StorageModel();
        }

        private StorageModel()
        {
            m_Organizers = new Dictionary<string, OrganizerModel>();
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
        public IOrganizerModel LoadOrganizer(string identifier)
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

        /// <summary>
        /// Unloads an organizer from memory, releasing all associated resources
        /// </summary>
        /// <param name="organizer">The organizer to unload</param>
        /// <exception cref="ArgumentException">Thrown when organizer was not created by LoadOrganizer of this instance</exception>
        public void UnloadOrganizer(IOrganizerModel organizer)
        {
            OrganizerModel? model = organizer as OrganizerModel;
            if (model == null)
                throw new ArgumentException("Organizer was not created by this StorageModel", nameof(organizer));

            if (!m_Organizers.ContainsKey(model.Identifier))
                throw new ArgumentException("Organizer was not created by this StorageModel", nameof(organizer));

            m_Organizers.Remove(model.Identifier);
        }

        #endregion
    }
}