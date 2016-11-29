using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rref
{
    abstract class Expression : IEquatable<Expression>
    {
        /*public static HashSet<Constant> _constants = new HashSet<Constant>();
        public static HashSet<Variable> _variables = new HashSet<Variable>();
        public static HashSet<Power> _powers = new HashSet<Power>();
        public static HashSet<Product> _products = new HashSet<Product>();
        public static HashSet<Sum> _sums = new HashSet<Sum>();
        public static HashSet<Ratio> _ratios = new HashSet<Ratio>();*/

        protected string _str = null;
        protected int? _hash = null;

        public static implicit operator Expression(double v)
        {
            return (Constant)v;
        }
        
        static Power AsPower(Expression x)
        {
            if (x is Variable)
            {
                return (Variable)x;
            }
            return x as Power;
        }

        static Product AsProduct(Expression x)
        {
            if (x is Constant)
            {
                return (Constant)x;
            }
            if (x is Variable || x is Power)
            {
                return AsPower(x);
            }
            return x as Product;
        }

        static Sum AsSum(Expression x)
        {
            if (x is Constant || x is Variable || x is Power || x is Product)
            {
                return AsProduct(x);
            }
            return x as Sum;
        }

        static Ratio AsRatio(Expression x)
        {
            if (x is Constant || x is Variable || x is Power || x is Product || x is Sum)
            {
                return AsSum(x);
            }
            return x as Ratio;
        }

        public static Expression operator +(Expression a, Expression b)
        {
            if (a == 0.0)
                return b;
            if (b == 0.0)
                return a;
            if (a is Ratio || b is Ratio)
            {
                return AsRatio(a) + AsRatio(b);
            }
            if (!(a is Constant && b is Constant))
            {
                return AsSum(a) + AsSum(b);
            }
            return (Constant)a + (Constant)b;
        }

        public static Expression operator -(Expression x)
        {
            return x * -1;
        }

        public static Expression operator -(Expression a, Expression b)
        {
            if (a == b)
                return 0.0;
            return a + (-b);
        }

        public static Expression operator *(Expression a, Expression b)
        {
            if (a == 0.0 || b == 0.0)
                return 0.0;
            if (a == 1.0)
                return b;
            if (b == 1.0)
                return a;
            if (a is Ratio || b is Ratio)
            {
                return AsRatio(a) * AsRatio(b);
            }
            if (a is Sum || b is Sum)
            {
                return AsSum(a) * AsSum(b);
            }
            if (!(a is Constant && b is Constant))
            {
                return AsProduct(a) * AsProduct(b);
            }
            return (Constant)a * (Constant)b;
        }

        public static Expression operator /(Expression a, Expression b)
        {
            if (a == 0.0)
                return 0.0;
            if (b == 1.0)
                return a;
            if (b == -1.0)
                return -a;
            if (a == b)
                return 1.0;
            if (a is Ratio || a is Sum || b is Ratio || b is Sum)
            {
                return AsRatio(a) / AsRatio(b);
            }
            if (!(a is Constant && b is Constant))
            {
                return AsProduct(a) / AsProduct(b);
            }
            return (Constant)a / (Constant)b;
        }

        public static Expression operator ^(Expression a, Constant b)
        {
            if (b == 0.0)
                return 1.0;
            if (b == 1.0)
                return a;
            if (a is Product)
            {
                return AsProduct(a) ^ b;
            }
            if (!(a is Constant))
            {
                return AsPower(a) ^ b;
            }
            return (Constant)a ^ b;
        }

        public static bool operator ==(Expression a, Expression b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Expression a, Expression b)
        {
            return !(a == b);
        }

        public bool Equals(Expression other)
        {
            if ((object)other == null)
                return false;

            if (this is Ratio || other is Ratio)
                return AsRatio(this).Equals(AsRatio(other));
            if (this is Sum || other is Sum)
                return AsSum(this).Equals(AsSum(other));
            if (this is Product || other is Product)
                return AsProduct(this).Equals(AsProduct(other));
            if (this is Power || other is Power)
                return AsPower(this).Equals(AsPower(other));
            if (this is Variable && other is Variable)
                return ((Variable)this).Equals((Variable)other);
            if (this is Constant && other is Constant)
                return ((Constant)this).Equals((Constant)other);
            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Expression);
        }

        public abstract int ComputeHashCode();
        
        public override int GetHashCode()
        {
            return _hash.Value;
        }

        public abstract string ComputeString();

        public override string ToString()
        {
            return _str;
        }

#if false
        static List<Expression> AllExpressions = new List<Expression>();
        static Dictionary<Operator, string> OperatorStrings = new Dictionary<Operator, string>() {
            {Operator.Addition, "+"},
            {Operator.Multiplication, "*"},
            {Operator.Exponentiation, "^"},
            {Operator.Division, "/"}
        };
        public static Expression NegOne = Expression.Value(-1.0);
        public static Expression Zero = Expression.Value(0.0);
        public static Expression One = Expression.Value(1.0);
        public static Expression Two = Expression.Value(2.0);

        string _identity;
        double? _value;
        bool _negation = false;
        HashSet<Expression> _terms;
        Expression _numerator;
        Expression _denominator;
        Expression _neg;
        Operator _op = Operator.None;

        public bool IsValue
        {
            get { return (_value != null); }
        }

        public bool IsNegation
        {
            get { return _negation; }
        }

        public static Expression Value(double val)
        {
            var any = AllExpressions.Where(x => x.IsValue && x._value.Value == val);
            if (any.Any())
            {
                return any.First();
            }
            return new Expression(val);
        }

        public static Expression Var(string identity)
        {
            var any = AllExpressions.Where(x => x._identity == identity);
            if (any.Any())
            {
                return any.First();
            }
            return new Expression(identity);
        }
        
        private Expression(double val)
        {
            _value = val;
            _identity = val.ToString();
            AllExpressions.Add(this);
        }
        
        private Expression(string identity)
        {
            _identity = identity;
            AllExpressions.Add(this);
        }

        private Expression(Expression pos)
        {
            _negation = true;
            _terms = new[] { pos };
            _identity = "-" + pos._identity;
            AllExpressions.Add(this);
        }

        public enum Operator
        {
            None,
            Addition,
            Multiplication,
            Division,
            Exponentiation
        }

        private Expression(IEnumerable<Expression> terms, Operator o)
        {
            _terms = new HashSet<Expression>(terms);
            _op = o;
            _identity = "(" + string.Join(OperatorStrings[o], _terms.Select(t => t._identity)) + ")";
            AllExpressions.Add(this);
        }

        public static implicit operator Expression(double val)
        {
            return Expression.Value(val);
        }

        public static Expression operator +(Expression a, Expression b)
        {
            if (a == Expression.Zero)
            {
                return b;
            }
            if (b == Expression.Zero)
            {
                return a;
            }
            if (a.IsValue && b.IsValue)
            {
                return Expression.Value(a._value.Value + b._value.Value);
            }
            if (a == b)
            {
                return Expression.Two * a;
            }
            if (a.IsNegation && !b.IsNegation && a._neg == b)
            {
                return Expression.Zero;
            }
            if (b.IsNegation && !a.IsNegation && b._neg == a)
            {
                return Expression.Zero;
            }
            var stuff = new HashSet<Expression>();
            if (a._op == Operator.Addition)
            {
                stuff.UnionWith(a._terms);
            }
            else
            {
                stuff.Add(a);
            }
            if (b._op == Operator.Addition)
            {
                stuff.UnionWith(b._terms);
            }
            else
            {
                stuff.Add(b);
            }
            var any = AllExpressions
                .Where(x => x._op == Operator.Addition)
                .Where(x => x._terms.SetEquals(stuff));
            if (any.Any())
            {
                return any.First();
            }
            return new Expression(stuff, Operator.Addition);
        }

        public static Expression operator -(Expression x)
        {
            if (x.IsValue)
            {
                return Expression.Value(-x._value.Value);
            }
            if (x.IsNegation)
            {
                return x._neg;
            }
            return new Expression(x);
        }

        public static Expression operator -(Expression a, Expression b)
        {
            if (a == b)
            {
                return Expression.Zero;
            }
            return a + -b;
        }

        public static Expression operator *(Expression a, Expression b)
        {
            if (a == Expression.Zero || b == Expression.Zero)
            {
                return Expression.Zero;
            }
            if (a.IsNegation && b.IsNegation)
            {
                return a._left * b._left;
            }
            if (a.IsNegation)
            {
                return -(a._left * b);
            }
            if (b.IsNegation)
            {
                return -(a * b._left);
            }
            if (a == Expression.NegOne)
            {
                return -b;
            }
            if (b == Expression.NegOne)
            {
                return -a;
            }
            if (a == Expression.One)
            {
                return b;
            }
            if (b == Expression.One)
            {
                return a;
            }
            /*if (a == b)
            {
                return a ^ Expression.Two;
            }*/
            
            var any = AllExpressions
                .Where(x => x._left == a || x._right == a)
                .Where(x => x._left == b || x._right == b)
                .Where(x => x._op == Operator.Multiplication);
            if (any.Any())
            {
                return any.First();
            }
            return new Expression(a, b, Operator.Multiplication);
        }

        public static Expression operator /(Expression a, Expression b)
        {
            if (a == Expression.Zero)
            {
                return Expression.Zero;
            }
            if (a == b)
            {
                return Expression.One;
            }
            if (a.IsValue && b.IsValue)
            {
                return Expression.Value(a._value.Value / b._value.Value);
            }
            if (b == Expression.One)
            {
                return a;
            }
            if (a.IsNegation && b.IsNegation)
            {
                return a._left / b._left;
            }
            if (a.IsNegation)
            {
                return -(a._left / b);
            }
            if (b.IsNegation)
            {
                return -(a / b._left);
            }

            var any = AllExpressions
                .Where(x => x._left == a && x._right == b)
                .Where(x => x._op == Operator.Division);
            if (any.Any())
            {
                return any.First();
            }
            return new Expression(a, b, Operator.Division);
        }

        /*public static Expression operator ^(Expression a, Expression b)
        {
            if (a == Expression.Zero)
            {
                return Expression.Zero;
            }
            if (a == Expression.One)
            {
                return Expression.One;
            }
            if (!b.IsValue)
            {
                throw new NotImplementedException();
            }
            if (b == Expression.Zero)
            {
                return Expression.One;
            }
            if (a.IsValue)
            {
                return Expression.Value(Math.Pow(a._value.Value, b._value.Value));
            }
            if (b == Expression.NegOne)
            {
                if (a._op == Operator.Division)
                {
                    return a._right / a._left;
                }
                return Expression.One / a;
            }

            var any = AllExpressions
                .Where(x => x._left == a && x._right == b)
                .Where(x => x._op == Operator.Exponentiation);
            if (any.Any())
            {
                return any.First();
            }
            return new Expression(a, b, Operator.Exponentiation);
        }*/

        public override string ToString()
        {
            return _identity;
        }
#endif
    }
}
