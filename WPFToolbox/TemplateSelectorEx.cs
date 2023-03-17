using CSToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFToolbox
{
    public class TemplateSelectorEx : DataTemplateSelector
    {
        public List<DataTemplate> Templates { get; } = new();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (Templates == null || item == null)
                return base.SelectTemplate(item, container);

            foreach (DataTemplate template in Templates)
            {
                if (template.DataType is not Type type)
                    continue;
                else if (!item.GetType().Is(type))
                    continue;
                return template;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
