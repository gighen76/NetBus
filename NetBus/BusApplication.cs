using System;
using System.Text.RegularExpressions;

namespace NetBus
{
    public class BusApplication
    {

        public BusApplication(string applicationName)
        {
            if (!Regex.IsMatch(applicationName, "^[a-z0-9_]+$", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException(nameof(applicationName));
            }
            Name = applicationName;
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            if (obj is BusApplication topic)
            {
                return Name.Equals(topic.Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
        
        public static bool operator ==(BusApplication btLeft, BusApplication btRight)
        {
            return btLeft.Equals(btRight);
        }
        public static bool operator !=(BusApplication btLeft, BusApplication btRight)
        {
            return !btLeft.Equals(btRight);
        }
        

    }
}
