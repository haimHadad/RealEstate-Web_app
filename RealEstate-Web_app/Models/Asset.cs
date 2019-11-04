using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate_Web_app.Models
{
    public class Asset : IComparable
    {
        private String assetOwner { get; set; }
        private uint  assetID {get; }
        private String assetStreet { get; set; }
        private uint assetNo { get; set; }
        private uint assetCity { get; set; }
        private uint assetRooms { get; set; }
        private uint assetAreaIn { get; set; }
        private uint assetAreaOut { get; set; }
        private uint assetPrice { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Asset otherAsset = obj as Asset;

            if (this.assetID == otherAsset.assetID)
                return 0;

            return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            Asset otherAsset = obj as Asset;

            if (this.assetID == otherAsset.assetID)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.assetID.GetHashCode();
        }
    }
    

   
}
