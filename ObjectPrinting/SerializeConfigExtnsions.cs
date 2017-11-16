using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class SerializeConfigExtnsions
    {
        public static PrintingConfig<TOwner> Using<TOwner>(this SerializeConfig<TOwner, double> config,
            CultureInfo culture)
        {
            var conf = ((ISerializeConfig<TOwner, double>) config).PrintingConfig;
            ((IPrintingConfig<TOwner>) conf).AddCustomCulture(typeof(double), culture);
            return conf;
        }
        public static PrintingConfig<TOwner> Using<TOwner>(this SerializeConfig<TOwner, int> config,
            CultureInfo culture)
        {
            var conf = ((ISerializeConfig<TOwner, int>) config).PrintingConfig;
            ((IPrintingConfig<TOwner>) conf).AddCustomCulture(typeof(int), culture);
            return conf;
        }
        public static PrintingConfig<TOwner> Using<TOwner>(this SerializeConfig<TOwner, float> config,
            CultureInfo culture)
        {
            var conf = ((ISerializeConfig<TOwner, float>) config).PrintingConfig;
            ((IPrintingConfig<TOwner>) conf).AddCustomCulture(typeof(float), culture);
            return conf;
        }
        public static PrintingConfig<TOwner> Using<TOwner>(this SerializeConfig<TOwner, long> config,
            CultureInfo culture)
        {
            var conf = ((ISerializeConfig<TOwner, long>) config).PrintingConfig;
            ((IPrintingConfig<TOwner>) conf).AddCustomCulture(typeof(long), culture);
            return conf;
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this SerializeConfig<TOwner, string> config,
            int index, int length)
        {
            return ((ISerializeConfig<TOwner, string>) config).PrintingConfig;
        }
    }
}