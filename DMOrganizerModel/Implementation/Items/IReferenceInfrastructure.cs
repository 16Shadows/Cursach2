using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal interface IReferenceInfrastructure
    {
        StringBuilder GetPath(int len);
    }
}
