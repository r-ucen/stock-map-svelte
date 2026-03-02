using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Services.TreeMap.Layout;

public static class RectangleSplitter
{
    public static (RectangleDto, RectangleDto) SplitRectangle(RectangleDto rectangleToSplit, double ratio)
    {
        RectangleDto current;
        RectangleDto remaining;
        
        if (rectangleToSplit.Width > rectangleToSplit.Height)
        {
            var split = rectangleToSplit.Width * ratio;
            current = new RectangleDto
            {
                X = rectangleToSplit.X,
                Y = rectangleToSplit.Y,
                Width = split,
                Height = rectangleToSplit.Height
            };
            
            remaining = new RectangleDto
            {
                X = rectangleToSplit.X + split,
                Y = rectangleToSplit.Y,
                Width = rectangleToSplit.Width - split,
                Height = rectangleToSplit.Height
            };
        }
        else
        {
            var split = rectangleToSplit.Height * ratio;
            current = new RectangleDto
            {
                X = rectangleToSplit.X,
                Y = rectangleToSplit.Y,
                Width = rectangleToSplit.Width,
                Height = split
            };
            
            remaining = new RectangleDto
            {
                X = rectangleToSplit.X,
                Y = rectangleToSplit.Y + split,
                Width = rectangleToSplit.Width,
                Height = rectangleToSplit.Height - split
            };
        }
        
        return (current, remaining);
    }
}