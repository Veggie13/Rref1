using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rref
{
    class Sum : Expression, IEquatable<Sum>
    {
        static Dictionary<Constant, Sum> _constConversions = new Dictionary<Constant, Sum>();
        static Dictionary<Product, Sum> _conversions = new Dictionary<Product, Sum>();

        internal HashSet<Product> _terms;
        internal double _constTerm;
        internal int? _termsHash = null;

        public Sum(IEnumerable<Product> terms, Constant constTerm)
        {
            _terms = new HashSet<Product>(terms.Where(t => t._coeff != 0.0));
            _constTerm = constTerm._val;
            ComputeHashCode();
            ComputeString();
        }

        public static implicit operator Sum(Constant c)
        {
            if (_constConversions.ContainsKey(c))
                return _constConversions[c];
            return (_constConversions[c] = new Sum(new Product[0], c));
        }

        public static implicit operator Sum(Product p)
        {
            if (_conversions.ContainsKey(p))
                return _conversions[p];
            return (_conversions[p] = new Sum(new[] { p }, 0.0));
        }

        public bool Equals(Sum other)
        {
            if ((object)other == null)
                return false;
            if (GetHashCode() != other.GetHashCode())
                return false;
            return _constTerm.Equals(other._constTerm) && _terms.SetEquals(other._terms);
        }

        public int ComputeTermsHashCode()
        {
            if (_termsHash == null)
                _termsHash = _terms.Aggregate(0, (x, y) => unchecked(x + y.GetHashCode()));
            return _termsHash.Value;
        }

        public override int ComputeHashCode()
        {
            unchecked
            {
                return _constTerm.GetHashCode() + ComputeTermsHashCode();
            }
        }

        public override string ComputeString()
        {
            if (!_terms.Any())
                return _constTerm.ToString();
            return string.Join(" + ", _terms.Select(t => t.ToString())) + (_constTerm == 0.0 ? "" : (" + " + _constTerm.ToString()));
        }

        class ProductComparer : IEqualityComparer<Product>
        {
            public bool Equals(Product x, Product y)
            {
                if (GetHashCode(x) != GetHashCode(y))
                    return false;
                return x._factors.SetEquals(y._factors);
            }

            public int GetHashCode(Product obj)
            {
                return obj.ComputeFactorsHashCode();
            }
        }
        static ProductComparer comp = new ProductComparer();

        public static Sum operator +(Sum a, Sum b)
        {
            var sharedLeft = a._terms.Intersect(b._terms, comp);
            var sharedRight = b._terms.Intersect(a._terms, comp);
            var outside = a._terms.Union(b._terms, comp).Except(sharedLeft, comp);
            var combined = sharedLeft.Join(sharedRight, p => p, p => p, (l, r) => new Product(l._coeff + r._coeff, l._factors), comp);
            return new Sum(outside.Union(combined), a._constTerm + b._constTerm);
        }

        public static Sum operator *(Sum a, Sum b)
        {
            var acxb = b._terms.Select(t => t * (Constant)a._constTerm);
            var result = a._terms
                .Select(t => new Sum(b._terms.Select(bt => bt * t).Union(new[] { t * (Constant)b._constTerm }), 0.0))
                .Aggregate(new Sum(acxb, a._constTerm * b._constTerm), (x, y) => x + y);
            return result;
        }
    }
}
