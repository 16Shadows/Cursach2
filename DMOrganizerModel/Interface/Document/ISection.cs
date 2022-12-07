using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.Document
{
    public interface ISection
    {
        string Title { get; set; }
        string Content { get; set; }
        ICollection<ISection> Children { get; }
    }
}
