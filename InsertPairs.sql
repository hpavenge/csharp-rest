/****** Script for SelectTopNRows command from SSMS  ******/
SELECT 
      [asset_id_base]
      ,[asset_id_quote]
	  ,COUNT(*) AS count
  INTO [CoinApi].[dbo].[Pairs] FROM [CoinApi].[dbo].[SymbolsDb] GROUP BY [asset_id_base],[asset_id_quote] ORDER BY COUNT(*) DESC

