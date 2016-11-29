using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rref
{
    class Variable : Expression, IEquatable<Variable>
    {
        internal char _name;

        public Variable(char name)
        {
            _name = name;
            ComputeHashCode();
            ComputeString();
        }

        public bool Equals(Variable other)
        {
            if ((object)other == null)
                return false;
            return _name == other._name;
        }

        public override int ComputeHashCode()
        {
            return _name.GetHashCode();
        }

        public override string ComputeString()
        {
            return _name.ToString();
        }
    }
}
