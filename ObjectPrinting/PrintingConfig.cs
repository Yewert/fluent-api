using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
            return PrintToString(obj, 0);
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
            typeof(TimeSpan)
        };

        private string PrintToString(object obj, int nestingLevel)
        {
            //TODO apply configurations
            if (obj == null)
                return "null" + Environment.NewLine;
            
            if (FinalTypes.Contains(obj.GetType()))
            {
                return obj + Environment.NewLine;
            }

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                string toAppend;
                var propertyType = propertyInfo.PropertyType;
                if (excludedTypes.Contains(propertyType))
                {
                    continue;
                }
                if (typeSerializators.ContainsKey(propertyType))
                {
                    if (propertyInfo.GetValue(obj) is null)
                    {
                        toAppend = "null" + Environment.NewLine;
                    }
                    else
                    {
                        toAppend = typeSerializators[propertyType].DynamicInvoke(propertyInfo.GetValue(obj))
                                   + Environment.NewLine;
                    }
                }
                else if (cultures.ContainsKey(propertyType))
                {
                    toAppend = ((IFormattable)propertyInfo.GetValue(obj)).ToString(null, cultures[propertyType])
                               + Environment.NewLine;
                }
                else
                {
                    toAppend = PrintToString(propertyInfo.GetValue(obj),
                        nestingLevel + 1);
                }
                sb.Append(identation + propertyInfo.Name + " = " + toAppend);
            }
            return sb.ToString();
        }

        public SerializeConfig<TOwner, TType> Printing<TType>()
        {
            return new SerializeConfig<TOwner, TType>(this);
        }

        public SerializeConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> propertySelector)
        {
            return new SerializeConfig<TOwner, TPropType>(this, propertySelector);
        }
        

        private readonly HashSet<Type> excludedTypes;
        public PrintingConfig<TOwner> ExcludingType<TType>()
        {
            excludedTypes.Add(typeof(TType));
            return this;
        }

        private readonly HashSet<string> excludedProperties;
        public PrintingConfig<TOwner> Excluding<TType>(Expression<Func<TOwner, TType>> propertySelector)
        {
            excludedProperties.Add(propertySelector.Body.ToString());
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
    }

    public interface IPrintingConfig<TOwner>
    {
        void AddCustomPropertySerializator(string propertyPath, Delegate serializator);
        void AddCustomTypeSerializator(Type type, Delegate serializator);
        void AddCustomCulture(Type type, CultureInfo culture);
    }

}