using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_rest
{
    class Program
    {
        static void Main(string[] args)
        {
            var coinApi = new CoinApi("EF475D08-FDC5-4845-A193-7ACC0952FCFE");
            DatabaseHelper database = new DatabaseHelper();
            int countChances = 0;
            List<Chance> chances = new List<Chance>();

            #region get exchange info and trading pairs
            //IEnumerable<Symbol> symbols = getExchanges(coinApi);
            //database.InsertSymbols(symbols);
            //run insertpairs.sql
            #endregion

            #region calculate price differences and return chances
            // get pairs that are on multiple exchanges
            IQueryable<Pair> pairs = database.GetAllPairs().Where(x => x.count > 2);
            // get all orderbooks from exchanges (sells and buys)
            var orderbooks_current_data = coinApi.Orderbooks_current_data_all();
            //filter out korean coin
            pairs = pairs.Where(x => x.asset_id_quote != "KRW");


            countChances = SimpleChances(database, countChances, chances, pairs, orderbooks_current_data);

            #endregion

            Console.ReadLine();

        }

        private static int SimpleChances(DatabaseHelper database, int countChances, List<Chance> chances, IQueryable<Pair> pairs, List<Orderbook> orderbooks_current_data)
        {
            foreach (Pair pair in pairs)
            {
                IQueryable<SymbolsDb> exchanges = database.GetExchanges(pair);
                int TotalExchanges = exchanges.Count();
                decimal defSellPrice = 0;
                decimal defBuyPrice = 0;
                string exchangeToSell = "";
                string exchangeToBuy = "";
                int firstLoopdone = 0;

                decimal volumeSell = 0;
                decimal volumeBuy = 0;

                // filter exchanges
                exchanges = exchanges.Where(x => x.exchange_id != "YOBIT" && x.exchange_id != "CCEX" && x.exchange_id != "HITBTC" && x.exchange_id != "BITFINEX" && x.exchange_id != "OKCOIN_USD");
                //TODO filter exchanges of 2 goedkope transfercoins traden

                foreach (var exchange in exchanges)
                {
                    Orderbook orderbook =
                        orderbooks_current_data.FirstOrDefault(x => x.symbol_id == exchange.symbol_id);


                    if (orderbook != null && orderbook.bids.Length > 1 && orderbook.asks.Length > 1)
                    {
                        Bid[] bids = orderbook.bids;
                        Ask[] asks = orderbook.asks;
                        volumeBuy = bids.Sum(x => x.size);
                        volumeSell = asks.Sum(x => x.size);
                        decimal ExchangeSellPrice = bids.Max(y => y.price); // wat bieden mensen ervoor ? -> tosell
                        decimal ExchangeBuyPrice = asks.Min(y => y.price); //  waar verkopen mensen het voor? -> tobuy

                        if (firstLoopdone == 0)
                        {
                            defSellPrice = ExchangeSellPrice;
                            exchangeToSell = exchange.exchange_id;
                            defBuyPrice = ExchangeBuyPrice;
                            exchangeToBuy = exchange.exchange_id;
                        }
                        else
                        {
                            if (ExchangeSellPrice >= defSellPrice)
                            {
                                defSellPrice = ExchangeSellPrice;
                                exchangeToSell = exchange.exchange_id;
                            }

                            if (ExchangeBuyPrice <= defBuyPrice)
                            {
                                defBuyPrice = ExchangeBuyPrice;
                                exchangeToBuy = exchange.exchange_id;
                            }

                        }

                        firstLoopdone = 1;
                    }
                }

                if (defSellPrice > defBuyPrice)
                {
                    decimal volumeBuyPrice = volumeBuy * defBuyPrice;
                    decimal volumeSellPrice = volumeSell * defSellPrice;
                    decimal profit = defSellPrice - defBuyPrice;
                    decimal percentage = (profit / defBuyPrice) * 100;
                    // remove death coins 
                    if (volumeBuyPrice > 100 && volumeSellPrice > 100 && percentage > 2)
                    {
                        //TODO CALC FEES
                        Chance chance = new Chance()
                        {
                            BaseCurrency = pair.asset_id_base,
                            QuoteCurrency = pair.asset_id_quote,
                            ExchangeToBuy = exchangeToBuy,
                            ExchangeToSell = exchangeToSell,
                            BuyPrice = defBuyPrice,
                            BuyVolume = volumeBuy,
                            SellPrice = defSellPrice,
                            SellVolume = volumeSell,
                            DifferencePercentage = percentage
                        };
                        chances.Add(chance);
                        //                            Console.WriteLine("Chance = " + pair.asset_id_base + "/" + pair.asset_id_quote + " Buy at:" +
                        //                                              exchangeToBuy + " for:" + defBuyPrice + "Volume: " + volumeBuy + "Sell at: " + exchangeToSell +
                        //                                              "for:" + defSellPrice + "Volume: " + volumeSell + "Difference: %" + percentage);
                        countChances++;
                    }



                }
            }


            foreach (var chance in chances.OrderBy(x => x.DifferencePercentage).ThenBy(x => x.BuyVolume))
            {
                Console.WriteLine(chance.ToConsoleString());
            }

            return countChances;
        }

        private static IEnumerable<Symbol> getExchanges(CoinApi coinApi)
        {
            Console.Write("Exchange:");
            Console.Write(Environment.NewLine);
            var exchange = coinApi.Metadata_list_exchanges();
            foreach (var item in exchange)
            {
                Console.Write("exchange_id:" + item.exchange_id);
                Console.Write(Environment.NewLine);
                Console.Write("website:" + item.website);
                Console.Write(Environment.NewLine);
                Console.Write("name:" + item.name);
                Console.Write(Environment.NewLine);
                Console.Write("--------------------------------------------------------------------------------------------------------");
                Console.Write(Environment.NewLine);
            }

            Console.Write("symbols:");
            Console.Write(Environment.NewLine);
            var symbols = coinApi.Metadata_list_symbols().Where(x => x.symbol_type == "SPOT");
            foreach (var item in symbols)
            {
                Console.Write("symbol_id:" + item.symbol_id);
                Console.Write(Environment.NewLine);
                Console.Write("exchange_id:" + item.exchange_id);
                Console.Write(Environment.NewLine);
                Console.Write("symbol_type:" + item.symbol_type);
                Console.Write(Environment.NewLine);
                Console.Write("asset_id_base:" + item.asset_id_base);
                Console.Write(Environment.NewLine);
                Console.Write("asset_id_quote:" + item.asset_id_quote);
                Console.Write(Environment.NewLine);
                Console.Write("--------------------------------------------------------------------------------------------------------");
                Console.Write(Environment.NewLine);
            }

            return symbols;
        }
    }
        }

