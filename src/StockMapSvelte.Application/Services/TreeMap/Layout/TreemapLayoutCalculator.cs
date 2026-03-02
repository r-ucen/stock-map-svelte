using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Services.TreeMap.Layout;

public class TreemapLayoutCalculator
{
    
    public TreemapDataDto CalculateTreemapRectangles(TreemapDataDto treemapData, double containerWidth, double containerHeight)
    {
        return SliceAndDice(treemapData, containerWidth, containerHeight, 0, 0);
    }

    private TreemapDataDto SliceAndDice(TreemapDataDto treemapData, double containerWidth,
        double containerHeight, double containerX, double containerY)
    {
        var sortedSectors = treemapData.Sectors.OrderByDescending(s => s.TotalMarketCap).ToList();

        double totalMarketCap = treemapData.TotalMarketCap;

        RectangleDto rect = new RectangleDto
        {
            X = containerX,
            Y = containerY,
            Width = containerWidth,
            Height = containerHeight
        };

        foreach (var sector in sortedSectors)
        {
            var currentSectorMarketCap = sector.TotalMarketCap;
            var sectorRatio = sector.TotalMarketCap / totalMarketCap;
            totalMarketCap -= currentSectorMarketCap;
            
            var (currentSectorRect, remainingSectorRect) = RectangleSplitter.SplitRectangle(rect, sectorRatio);
            rect = remainingSectorRect;
            
            sector.Rectangle = currentSectorRect;
            
            var workingSectorRect = currentSectorRect;
            
            SliceAndDiceAddStockTilesForSector(sector, currentSectorMarketCap, workingSectorRect);
        }

        return treemapData;
    }

    private void SliceAndDiceAddStockTilesForSector(TreemapSectorDto pSector, double pCurrentSectorMarketCap,
        RectangleDto pWorkingSectorRect)
    {
        var sortedStocks = pSector.Stocks.OrderByDescending(s => s.MarketCap).ToList();
        
        foreach (var stock in sortedStocks)
        {
            var ratio = stock.MarketCap / pCurrentSectorMarketCap;
            pCurrentSectorMarketCap -= stock.MarketCap;
            
            var (currentStockRect, remainingStockRect) = RectangleSplitter.SplitRectangle(pWorkingSectorRect, ratio);
            
            stock.Rectangle = currentStockRect;
            
            pWorkingSectorRect = remainingStockRect;
        }
    }
}