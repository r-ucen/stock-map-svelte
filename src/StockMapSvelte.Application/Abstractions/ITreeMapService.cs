using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Application.Abstractions;

public interface ITreeMapService
{
    public TreemapDataDto CalculateTreemapRectangles(TreemapDataDto treemapData, double containerWidth, double containerHeight);
    public string GetCellColor(TreemapNodeDto stock, MapMetric mapMetric);
    public string GetCellDescription(TreemapNodeDto stock, MapMetric mapMetric);
}