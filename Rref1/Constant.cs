using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rref
{
    class Constant : Expression, IEquatable<Constant>
    {
        static Dictionary<double, Constant> _values = new Dictionary<double, Constant>();

        internal double _val;

        public Constant(double val)
        {
            _val = val;
            ComputeHashCode();
            ComputeString();
        }

        public static implicit operator Constant(double val)
        {
            if (_values.ContainsKey(val))
                return _values[val];
            return (_values[val] = new Constant(val));
        }

        public bool Equals(Constant other)
        {
            if ((object)other == null)
                return false;
            return _val == other._val;
        }

        public override int ComputeHashCode()
        {
            return _val.GetHashCode();
        }

        public override string ComputeString()
        {
            return _val.ToString();
        }

        public static Constant operator +(Constant a, Constant b)
        {
            return (Constant)(a._val + b._val);
        }

        public static Constant operator *(Constant a, Constant b)
        {
            return (Constant)(a._val * b._val);
        }

        public static Constant operator /(Constant a, Constant b)
        {
            return (Constant)(a._val / b._val);
        }

        public static Constant operator ^(Constant a, Constant b)
        {
            return (Constant)(Math.Pow(a._val, b._val));
        }
    }
}
