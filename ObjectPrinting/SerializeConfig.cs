using System;

namespace ObjectPrinting
{
    public class SerializeConfig<TOwner, TSerializble> : PrintingConfig<TOwner>, ISerializeConfig<TOwner, TSerializble>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly string propertyPath;

        public SerializeConfig(PrintingConfig<TOwner> printingConfig,
           string propertyPath = null)
        {
            this.printingConfig = printingConfig;
            this.propertyPath = propertyPath;
        } 

        public PrintingConfig<TOwner> Using(Func<TSerializble, string> fucc)
        {
            if (propertyPath is null)
                ((IPrintingConfig<TOwner>) printingConfig).AddCustomTypeSerializator(typeof(TSerializble), fucc);
            else
                ((IPrintingConfig<TOwner>) printingConfig).AddCustomPropertySerializator(
                    propertyPath, fucc);
            return printingConfig;
        }

        PrintingConfig<TOwner> ISerializeConfig<TOwner, TSerializble>.PrintingConfig => printingConfig;
    }

    public interface ISerializeConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> PrintingConfig { get; }
    }
}