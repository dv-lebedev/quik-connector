using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Data.Channels
{
    public class Security
    {
        public string Instrument { get; set; }
        public string Stock { get; set; }
        public string SecCode { get; set; }
        public string Class { get; set; }
        public string ClassCode { get; set; }
        public string Status { get; set; }
        public decimal Supply { get; set; }
        public int SupplyVolume { get; set; }
        public decimal Demand { get; set; }
        public int DemandVolume { get; set; }
        public decimal PriceOfLastDeal { get; set; }
        public int Lot { get; set; }
        public decimal ExchangeFee { get; set; }

        public event EventHandler<Security> Updated;

        public void SetValue(int position, double value)
        {
            switch (position)
            {
                case 5:
                    Supply = (decimal) value;
                    break;
                case 6:
                    SupplyVolume = (int)value;
                    break;
                case 7:
                    Demand = (decimal)value;
                    break;
                case 8:
                    DemandVolume = (int)value;
                    break;
                case 9:
                    PriceOfLastDeal = (decimal)value;
                    break;
                case 10:
                    Lot = (int)value;
                    break;
                case 11:
                    ExchangeFee = (decimal)value;
                    break;
                default:
                    break;
            }
        }
        public void SetValue(int position, string value)
        {
            switch (position)
            {
                case 0:
                    Instrument = value;
                    break;
                case 1:
                    SecCode = value;
                    break;
                case 2:
                    Class = value;
                    break;
                case 3:
                    ClassCode = value;
                    break;
                case 4:
                    Status = value;
                    break;
                default: break;
            }
        }

        public void OnUpdated(object sender, Security item)
        {
            if (Updated != null) Updated(sender, item);
        }

        public void Update(Security mtr)
        {
            Instrument = mtr.Instrument;
            SecCode = mtr.SecCode;
            Class = mtr.Class;
            ClassCode = mtr.ClassCode;
            Status = mtr.Status;
            Supply = mtr.Supply;
            SupplyVolume = mtr.SupplyVolume;
            Demand = mtr.Demand;
            DemandVolume = mtr.DemandVolume;
            PriceOfLastDeal = mtr.PriceOfLastDeal;
            Lot = mtr.Lot;
            ExchangeFee = mtr.ExchangeFee;

            OnUpdated(this, this);
        }
    }
}
