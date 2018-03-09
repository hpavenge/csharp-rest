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
            return coinApiEntities.SymbolsDbs.Where(x =>
                x.asset_id_base == pair.asset_id_base && x.asset_id_quote == pair.asset_id_quote);
        }
    }
}
