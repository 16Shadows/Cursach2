using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFToolbox
{
    public class TemplateSelectorEx : DataTemplateSelector
    {
        public List<DataTemplate> Templates { get; } = new();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (Templates == null)
                return base.SelectTemplate(item, container);

            foreach (DataTemplate template in Templates)
            {
                if (template.DataType is not Type type)
                    continue;
                else if (!item.GetType().IsSubclassOf(type) && item.GetType() != type)
                    continue;
                return template;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
