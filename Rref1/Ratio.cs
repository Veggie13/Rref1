using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rref
{
    class Ratio : Expression, IEquatable<Ratio>
    {
        static Dictionary<Constant, Ratio> _constConversions = new Dictionary<Constant, Ratio>();
        static Dictionary<Sum, Ratio> _conversions = new Dictionary<Sum, Ratio>();

        internal Sum _numerator;
        internal Sum _denominator;

        public Ratio(Sum numerator, Sum denominator)
        {
            _numerator = numerator;
            _denominator = denominator;
            ComputeHashCode();
            ComputeString();
        }

        public static implicit operator Ratio(Constant c)
        {
            if (_constConversions.ContainsKey(c))
                return _constConversions[c];
            return (_constConversions[c] = new Ratio(c, (Constant)1.0));
        }
        
        public static implicit operator Ratio(Sum s)
        {
            if (_conversions.ContainsKey(s))
                return _conversions[s];
            return (_conversions[s] = new Ratio(s, (Constant)1.0));
        }

        public bool Equals(Ratio other)
        {
            if ((object)other == null)
                return false;
            return (_numerator.Equals(other._numerator) && _denominator.Equals(other._denominator));
        }

        public override int ComputeHashCode()
        {
            return _numerator.GetHashCode() + _denominator.GetHashCode();
        }

        public override string ComputeString()
        {
            return "( " + _numerator.ToString() + " )/( " + _denominator.ToString() + " )";
        }

        public static Ratio operator +(Ratio a, Ratio b)
        {
            if (a._denominator == b._denominator)
                return new Ratio(a._numerator + b._numerator, a._denominator);
            return new Ratio((a._numerator * b._denominator) + (b._numerator * a._denominator), a._denominator * b._denominator);
        }

        public static Ratio operator *(Ratio a, Ratio b)
        {
            if (a._numerator == b._denominator)
                return new Ratio(b._numerator, a._denominator);
            if (a._denominator == b._numerator)
                return new Ratio(a._numerator, b._denominator);
            return new Ratio(a._numerator * b._numerator, a._denominator * b._denominator);
        }

        public static Ratio operator /(Ratio a, Ratio b)
        {
            if (a._denominator == b._denominator)
                return new Ratio(a._numerator, b._numerator);
            if (a._numerator == b._numerator)
                return new Ratio(b._denominator, a._denominator);
            return new Ratio(a._numerator * b._denominator, a._denominator * b._numerator);
        }
    }
}
