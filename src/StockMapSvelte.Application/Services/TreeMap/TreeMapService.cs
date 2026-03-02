using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Enums;
using StockMapSvelte.Application.Services.TreeMap.Coloring;
using StockMapSvelte.Application.Services.TreeMap.Formatting;
using StockMapSvelte.Application.Services.TreeMap.Layout;

namespace StockMapSvelte.Application.Services.TreeMap;

public class TreeMapService(
    TreemapLayoutCalculator layout,
    CellDescriptionFormatter formatter,
    ColorScaleService colors)
    : ITreeMapService
{
    public TreemapDataDto CalculateTreemapRectangles(
        TreemapDataDto treemapData,
        double containerWidth,
        double containerHeight)
        => layout.CalculateTreemapRectangles(treemapData, containerWidth, containerHeight);
    
    public string GetCellColor(TreemapNodeDto stock, MapMetric mapMetric)
        => colors.GetCellColor(stock, mapMetric);
    
    public string GetCellDescription(TreemapNodeDto stock, MapMetric mapMetric)
        => formatter.GetCellDescription(stock, mapMetric);
}