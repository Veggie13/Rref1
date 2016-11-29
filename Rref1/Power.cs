using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rref
{
    class Power : Expression, IEquatable<Power>
    {
        static Dictionary<Variable, Power> _conversions = new Dictionary<Variable, Power>();

        internal Variable _var;
        internal double _exponent;

        public Power(Variable var, Constant exponent)
        {
            _var = var;
            _exponent = exponent._val;
            ComputeHashCode();
            ComputeString();
        }

        public static implicit operator Power(Variable v)
        {
            if (_conversions.ContainsKey(v))
                return _conversions[v];
            return (_conversions[v] = new Power(v, 1.0));
        }

        public bool Equals(Power other)
        {
            if ((object)other == null)
                return false;
            return (_var.Equals(other._var) && _exponent == other._exponent);
        }

        public override int ComputeHashCode()
        {
            return _var.GetHashCode() + _exponent.GetHashCode();
        }

        public override string ComputeString()
        {
            if (_exponent == 0.0)
                return "1";
            if (_exponent == 1.0)
                return _var.ToString();
            return _var.ToString() + "^" + _exponent.ToString();
        }

        public static Power operator ^(Power a, Constant b)
        {
            return new Power(a._var, a._exponent * b._val);
        }
    }
}
