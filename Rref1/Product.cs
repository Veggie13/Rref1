using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rref
{
    class Product : Expression, IEquatable<Product>
    {
        static Dictionary<Constant, Product> _constConversions = new Dictionary<Constant, Product>();
        static Dictionary<Power, Product> _conversions = new Dictionary<Power, Product>();

        internal double _coeff;
        internal HashSet<Power> _factors;
        internal int? _factorsHash = null;

        public Product(Constant coeff, IEnumerable<Power> factors)
        {
            _coeff = coeff._val;
            _factors = new HashSet<Power>(factors.Where(f => f._exponent != 0.0));
            ComputeHashCode();
            ComputeString();
        }

        public static implicit operator Product(Constant c)
        {
            if (_constConversions.ContainsKey(c))
                return _constConversions[c];
            return (_constConversions[c] = new Product(c, new Power[0]));
        }
        
        public static implicit operator Product(Power p)
        {
            if (_conversions.ContainsKey(p))
                return _conversions[p];
            return (_conversions[p] = new Product(1.0, new[] { p }));
        }

        public bool Equals(Product other)
        {
            if ((object)other == null)
                return false;
            if (GetHashCode() != other.GetHashCode())
                return false;
            return _coeff == other._coeff && _factors.SetEquals(other._factors);
        }

        public int ComputeFactorsHashCode()
        {
            if (_factorsHash == null)
                _factorsHash = _factors.Aggregate(0, (x, y) => unchecked(x + y.GetHashCode()));
            return _factorsHash.Value;
        }

        public override int ComputeHashCode()
        {
            unchecked
            {
                return _coeff.GetHashCode() + ComputeFactorsHashCode();
            }
        }

        public override string ComputeString()
        {
            if (!_factors.Any() || _coeff == 0.0)
                return _coeff.ToString();
            return (_coeff == 1.0 ? "" : (_coeff == -1.0 ? "-" : _coeff.ToString())) + "(" + string.Join(")(", _factors.Select(p => p.ToString())) + ")";
        }

        class PowerComparer : IEqualityComparer<Power>
        {
            public bool Equals(Power x, Power y)
            {
                return x._var.Equals(y._var);
            }

            public int GetHashCode(Power obj)
            {
                return obj._var.GetHashCode();
            }
        }

        static PowerComparer comp = new PowerComparer();

        public static Product operator *(Product a, Product b)
        {
            var sharedLeft = a._factors.Intersect(b._factors, comp);
            var sharedRight = b._factors.Intersect(a._factors, comp);
            var outside = a._factors.Union(b._factors, comp).Except(sharedLeft, comp);
            var combined = sharedLeft.Join(sharedRight, p => p._var, p => p._var, (l, r) => new Power(l._var, l._exponent + r._exponent));
            return new Product(a._coeff * b._coeff, outside.Union(combined));
        }

        public static Product operator /(Product a, Product b)
        {
            return a * (b ^ -1.0);
        }

        public static Product operator ^(Product a, Constant b)
        {
            return new Product(Math.Pow(a._coeff, b._val), a._factors.Select(p => new Power(p._var, p._exponent * b._val)));
        }
    }
}
