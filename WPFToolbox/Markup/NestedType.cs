using System;
using System.Reflection;
using System.Windows.Markup;

namespace WPFToolbox.Markup
{
    public class NestedType : MarkupExtension
    {
        public string? TypeName { get; set; }

        public NestedType() { }

        public NestedType(string typename)
        {
            TypeName = typename;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IXamlTypeResolver)) == null)
                return null;

            if (TypeName == null)
                throw new ArgumentNullException(nameof(TypeName));

            string[] comps = TypeName.Split('.');
            TypeExtension ext = new TypeExtension(comps[0]);
            Type? t = (Type)ext.ProvideValue(serviceProvider);
            if (t == null)
                throw new ArgumentException("Invalid type", nameof(TypeName));
            for(int i = 1; i < comps.Length; i++)
            {
                t = t.GetNestedType(comps[i], BindingFlags.Static | BindingFlags.Public);
                if (t == null)
                    throw new ArgumentException("Invalid type", nameof(TypeName));
            }
            return t;
        }
    }
}
