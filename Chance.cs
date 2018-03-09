using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_rest
{
    public class Chance
    {
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public string ExchangeToBuy { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal BuyVolume { get; set; }
        public string ExchangeToSell { get; set; }
        public decimal SellPrice { get; set; }
        public decimal SellVolume { get; set; }
        public decimal DifferencePercentage { get; set; }
        //        Console.WriteLine("Chance = " + pair.asset_id_base + "/" + pair.asset_id_quote + " Buy at:" +
        //        exchangeToBuy + " for:" + defBuyPrice + "Volume: " + volumeBuy + "Sell at: " + exchangeToSell +
        //        "for:" + defSellPrice + "Volume: " + volumeSell + "Difference: %" + percentage);
        public string ToConsoleString()
        {
            string chance = "Chance = " + BaseCurrency + "/" + QuoteCurrency + " Buy at:" +
                            ExchangeToBuy + " for:" + BuyPrice + "Volume: " + BuyPrice + "Sell at: " + ExchangeToSell +
                            "for:" + SellPrice + "Volume: " + SellVolume + "Difference: %" + DifferencePercentage;
            return chance;
        }
    }
}
