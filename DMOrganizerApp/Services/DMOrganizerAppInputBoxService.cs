using DMOrganizerViewModel;
using MVVMToolbox;
using MVVMToolbox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerApp.Services
{
    internal class DMOrganizerAppInputBoxService : IInputBoxService<OrganizerInputBoxScenarios>
    {
        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<OrganizerInputBoxScenarios, ReturnType> configuration)
        {
            throw new NotImplementedException();
        }
    }
}
