using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetBus
{
    public class BusTopic
    {

        public BusTopic(string topicName)
        {
            if (!Regex.IsMatch(topicName, "^[a-z0-9_]+$", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException(nameof(topicName));
            }
            Name = topicName;
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            if (obj is BusTopic topic)
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
        
        public static bool operator ==(BusTopic btLeft, BusTopic btRight)
        {
            if (btLeft is null)
            {
                return btRight is null;
            }
            return btLeft.Equals(btRight);
        }
        public static bool operator !=(BusTopic btLeft, BusTopic btRight)
        {
            return !(btLeft == btRight);
        }


        public static bool TryParse(string topicName, out BusTopic busTopic)
        {
            try
            {
                busTopic = new BusTopic(topicName);
                return true;
            }
            catch
            {
                busTopic = null;
                return false;
            }
        }
        

    }
}
