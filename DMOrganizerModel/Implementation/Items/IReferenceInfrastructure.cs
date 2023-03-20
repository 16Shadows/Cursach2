using System.Text;

namespace DMOrganizerModel.Implementation.Items
{
    internal interface IReferenceInfrastructure
    {
        StringBuilder GetPath(int len);
    }
}
