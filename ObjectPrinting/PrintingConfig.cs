using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        public PrintingConfig()
        {
            propertySerializators = new Dictionary<string, Delegate>();
            excludedTypes = new HashSet<Type>();
            excludedProperties = new HashSet<string>();
            typeSerializators = new Dictionary<Type, Delegate>();
            cultures = new Dictionary<Type, CultureInfo>();
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0, obj.GetType().ToString());
        }
        
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Type[] FinalTypes = 
        {
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(sbyte),
            typeof(byte),
            typeof(double), 
            typeof(float),
            typeof(decimal),
            typeof(string),
            typeof(DateTime), 
            typeof(TimeSpan),
            typeof(Guid)
        };

        private string PrintToString(object obj, int nestingLevel, string path)
        {
            if (obj == null)
                return "null" + Environment.NewLine;
            
            if (FinalTypes.Contains(obj.GetType()))
            {
                if (obj is string s && !(length is null))
                    return s.Substring(0, length.Value) + Environment.NewLine;
                return obj + Environment.NewLine;
            }

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                var propertyPath = path + "." + propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;
                if(excludedProperties.Contains(propertyPath))
                    continue;
                if (excludedTypes.Contains(propertyType))
                    continue;
                sb.Append(identation + propertyInfo.Name + " = " 
                          + AppendNextProperty(propertyType, propertyPath, obj, propertyInfo, nestingLevel));
            }
            return sb.ToString();
        }

        private string AppendNextProperty(Type propertyType,
            string propPath, object obj, PropertyInfo propertyInfo, int nestingLevel)
        {
            if (propertySerializators.ContainsKey(propPath))
                return propertySerializators[propPath].DynamicInvoke(propertyInfo.GetValue(obj)) + Environment.NewLine;
            if (typeSerializators.ContainsKey(propertyType))
            {
                if (propertyInfo.GetValue(obj) is null)
                    return "null" + Environment.NewLine;
                return typeSerializators[propertyType].DynamicInvoke(propertyInfo.GetValue(obj))
                           + Environment.NewLine;
            }
            if (cultures.ContainsKey(propertyType))
                return ((IFormattable)propertyInfo.GetValue(obj)).ToString(null, cultures[propertyType])
                           + Environment.NewLine;
            return PrintToString(propertyInfo.GetValue(obj),
                    nestingLevel + 1, propPath);
        }

        public SerializeConfig<TOwner, TType> Printing<TType>()
        {
            return new SerializeConfig<TOwner, TType>(this);
        }

        public SerializeConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> propertySelector)
        {
            var propertyPath = ExtractPropertyPath(propertySelector);
            return new SerializeConfig<TOwner, TPropType>(this, propertyPath);
        }
        

        private readonly HashSet<Type> excludedTypes;
        public PrintingConfig<TOwner> ExcludingType<TType>()
        {
            excludedTypes.Add(typeof(TType));
            return this;
        }

        private readonly HashSet<string> excludedProperties;
        public PrintingConfig<TOwner> ExcludingProperty<TType>(Expression<Func<TOwner, TType>> propertySelector)
        {
            excludedProperties.Add(ExtractPropertyPath(propertySelector));
            return this;
        }


        private readonly Dictionary<string, Delegate> propertySerializators;
        void IPrintingConfig<TOwner>.AddCustomPropertySerializator(string propertyPath, Delegate serializator)
        {
            propertySerializators[propertyPath] = serializator;
        }

        private readonly Dictionary<Type, Delegate> typeSerializators;
        void IPrintingConfig<TOwner>.AddCustomTypeSerializator(Type type, Delegate serializator)
        {
            typeSerializators[type] = serializator;
        }

        private readonly Dictionary<Type, CultureInfo> cultures;
        void IPrintingConfig<TOwner>.AddCustomCulture(Type type, CultureInfo culture)
        {
            cultures[type] = culture;
        }

        private int? length;
        void IPrintingConfig<TOwner>.AddMaxLength(int length)
        {
            this.length = length;
        }

        private string ExtractPropertyPath<TType>(Expression<Func<TOwner, TType>> propertySelector)
        {
            var propertyPath = typeof(TOwner).ToString();
            var member = (propertySelector.Body as MemberExpression).ToString();
            var index = member.IndexOf('.');
            member = member.Substring(index);
            return propertyPath + member;
        }
    }

    public interface IPrintingConfig<TOwner>
    {
        void AddCustomPropertySerializator(string propertyPath, Delegate serializator);
        void AddCustomTypeSerializator(Type type, Delegate serializator);
        void AddCustomCulture(Type type, CultureInfo culture);
        void AddMaxLength(int length);
    }

}