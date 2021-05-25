using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Circuit
{
    public class Connection : IEquatable<Connection>, IEnumerable<Terminal>
    {
        private readonly (Terminal, Terminal) _connection;

        public Connection(Terminal a, Terminal b)
        {
            _connection = (a, b);
        }

        public Terminal this[int t] 
        {
            get 
            {
                if(t < 0 || t > 1) throw new ArgumentOutOfRangeException($"Connection can only contain two terminals. Trying to access {t} terminal, which does not exist.");
                return t == 0 ? _connection.Item1 : _connection.Item2;
            }
        }

        public static bool operator ==(Connection a, Connection b) => a.Equals(b);
        public static bool operator !=(Connection a, Connection b) => !a.Equals(b);
        public bool Equals(Connection other)
        {
            return (this[0] == other[0] && this[1] == other[1]) || (this[0] == other[1] && this[1] == other[0]);
        }
        public override bool Equals(object obj)
        {
            if(obj is Connection b) return this == b;
            return false;
        }
        public override int GetHashCode()
        {
            int h1 = this[0].GetHashCode();
            int h2 = this[1].GetHashCode();
            return h1 ^ h2;
        }
        public override string ToString()
        {
            return $"Connection between Terminal:{this[0].Id} & Terminal:{this[1].Id}";
        }

        public IEnumerator<Terminal> GetEnumerator()
        {
            yield return this[0];
            yield return this[1];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return this[0];
            yield return this[1];
        }
    }
}
