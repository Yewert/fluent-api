using System;
using System.Linq.Expressions;

namespace ObjectPrinting
{
    public class SerializeConfig<TOwner, TSerializble> : PrintingConfig<TOwner>, ISerializeConfig<TOwner, TSerializble>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly Expression<Func<TOwner, TSerializble>> propertySelector;

        public SerializeConfig(PrintingConfig<TOwner> printingConfig,
            Expression<Func<TOwner, TSerializble>> propertySelector = null)
        {
            this.printingConfig = printingConfig;
            this.propertySelector = propertySelector;
        } 

        public PrintingConfig<TOwner> Using(Func<TSerializble, string> fucc)
        {
            if (propertySelector is null)
                ((IPrintingConfig<TOwner>) printingConfig).AddCustomTypeSerializator(typeof(TSerializble), fucc);
            else
                ((IPrintingConfig<TOwner>) printingConfig).AddCustomPropertySerializator(
                    propertySelector.Body.ToString(), fucc);
            return printingConfig;
        }

        PrintingConfig<TOwner> ISerializeConfig<TOwner, TSerializble>.PrintingConfig => printingConfig;
    }

    public interface ISerializeConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> PrintingConfig { get; }
    }
}