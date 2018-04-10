using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_rest
{
    public class DatabaseHelper
    {
        CoinApiEntities coinApiEntities = new CoinApiEntities();

        internal void InsertSymbols(IEnumerable<Symbol> symbols)
        {
            List<SymbolsDb> symbolsDb = new List<SymbolsDb>();
            foreach (var symbol in symbols)
            {
                SymbolsDb symbolDb = new SymbolsDb();
                symbolDb.symbol_id = symbol.symbol_id;
                symbolDb.exchange_id = symbol.exchange_id;
                symbolDb.symbol_type = symbol.symbol_type;
                symbolDb.asset_id_base = symbol.asset_id_base;
                symbolDb.asset_id_quote = symbol.asset_id_quote;
                symbolsDb.Add(symbolDb);
            }

            coinApiEntities.SymbolsDbs.AddRange(symbolsDb);
            coinApiEntities.SaveChanges();
        }

        public DbSet<Pair> GetAllPairs()
        {
            return coinApiEntities.Pairs;
        }

        public IQueryable<SymbolsDb> GetExchanges(Pair pair)
        {
            List<string> ltcExchanges = getLtcExchanges();
            List<string> xlmExchanges = getXlmExchanges();
            List<string> neoExchanges = getNeoExchanges();
            List<string> xrpExchanges = getXrpExchanges();
            List<string> AlCombined = new List<string>();
            AlCombined.AddRange(ltcExchanges);
            AlCombined.AddRange(xlmExchanges);
            AlCombined.AddRange(neoExchanges);
            AlCombined.AddRange(xrpExchanges);
            AlCombined.RemoveAll(x => x == "BITFINEX");
            List<string> GoldenExchanges = AlCombined.GroupBy(x => x).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

            return coinApiEntities.SymbolsDbs.Where(x =>
                x.asset_id_base == pair.asset_id_base && x.asset_id_quote == pair.asset_id_quote && GoldenExchanges.Contains(x.exchange_id));
        }

        public List<string> getXrpExchanges()
        {
            return coinApiEntities.SymbolsDbs.Where(x => x.asset_id_base == "XRP").Select(x => x.exchange_id).Distinct().ToList();
        }

        public List<string> getNeoExchanges()
        {
            return coinApiEntities.SymbolsDbs.Where(x => x.asset_id_base == "NEO").Select(x => x.exchange_id).Distinct().ToList();
        }

        public List<string> getXlmExchanges()
        {
            return coinApiEntities.SymbolsDbs.Where(x => x.asset_id_base == "XLM").Select(x => x.exchange_id).Distinct().ToList();
        }

        public List<string> getLtcExchanges()
        {
            return coinApiEntities.SymbolsDbs.Where(x => x.asset_id_base == "LTC").Select(x => x.exchange_id).Distinct().ToList();
        }
    }
}
