using System;

namespace AccidentalNoise
{
    public sealed class ImplicitBias : ImplicitModuleBase
    {
        public ImplicitBias(ImplicitModuleBase source, Double bias)
        {
            this.Source = source;
            this.Bias = new ImplicitConstant(bias);
        }

        public ImplicitBias(ImplicitModuleBase source, ImplicitModuleBase bias)
        {
            this.Source = source;
            this.Bias = bias;
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase Bias { get; set; }

        public override Double Get(Double x, Double y)
        {
			return MyMathHelper.Bias(this.Bias.Get(x, y), this.Source.Get(x, y));
        }

        public override Double Get(Double x, Double y, Double z)
        {
			return MyMathHelper.Bias(this.Bias.Get(x, y, z), this.Source.Get(x, y, z));
        }

        public override Double Get(Double x, Double y, Double z, Double w)
        {
			return MyMathHelper.Bias(this.Bias.Get(x, y, z, w), this.Source.Get(x, y, z, w));
        }

        public override Double Get(Double x, Double y, Double z, Double w, Double u, Double v)
        {
			return MyMathHelper.Bias(this.Bias.Get(x, y, z, w, u, v), this.Source.Get(x, y, z, w, u, v));
        }
    }
}