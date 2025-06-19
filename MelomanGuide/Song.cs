using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelomanGuide 
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }

        public override string ToString()
        {
            return $"{Title} - {Artist} ({Album})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Song other)
            {
                return Title == other.Title && Artist == other.Artist && Album == other.Album;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Title?.GetHashCode() ?? 0);
                hash = hash * 23 + (Artist?.GetHashCode() ?? 0);
                hash = hash * 23 + (Album?.GetHashCode() ?? 0);
                return hash;
            }
        }

    }

}
