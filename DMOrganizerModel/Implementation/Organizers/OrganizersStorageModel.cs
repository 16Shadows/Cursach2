using CSToolbox.Weak;
using DMOrganizerModel.Interface.Organizer;
using System.IO;

namespace DMOrganizerModel.Implementation.Organizers
{
    public static class OrganizersStorageModel
    {
        private static WeakCache<string, Organizer> OrganizersCache { get; } = new(path => new Organizer(path));

        /// <summary>
        /// Loads .dmo file located at specified path
        /// </summary>
        /// <param name="path">The path to file</param>
        /// <returns>The Organizer stored in the specified file</returns>
        public static IOrganizer LoadOrganizer(string path) => OrganizersCache[path];
        public static void DeleteOrganizer(string path)
        {
            if (!File.Exists(path))
                return;
            File.Delete(path);
        }
    }
}