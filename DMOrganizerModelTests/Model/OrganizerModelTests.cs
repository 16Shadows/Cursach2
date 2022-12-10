using DMOrganizerModel.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModelTests.Model
{
    [TestClass()]
    public class OrganizerModelTests
    {
        [TestMethod()]
        public void CreateReferenceNullArgumentTest()
        {
            IOrganizerModel organizer =  StorageModel.Instance.LoadOrganizer("test.dmo");

            Assert.ThrowsException<ArgumentNullException>(() => organizer.CreateReference(null));

            StorageModel.Instance.UnloadOrganizer(organizer);
        }

        class AlienItem : IItem
        {
            public string Title => throw new NotImplementedException();

            public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;

            public bool Rename(string name)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod()]
        public void CreateReferenceAlienArgumentTest()
        {
            IOrganizerModel organizer =  StorageModel.Instance.LoadOrganizer("test.dmo");

            Assert.ThrowsException<ArgumentException>(() => organizer.CreateReference(new AlienItem()));

            StorageModel.Instance.UnloadOrganizer(organizer);
        }
    }
}
