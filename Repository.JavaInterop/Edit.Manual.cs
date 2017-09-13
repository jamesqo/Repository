using System;

namespace Repository.JavaInterop
{
    public partial class Edit : IEquatable<Edit>
    {
        public override bool Equals(object obj)
            => obj is Edit other && Equals(other);

        public override int GetHashCode() => throw new NotSupportedException();

        public override string ToString() => base.ToString();
    }
}