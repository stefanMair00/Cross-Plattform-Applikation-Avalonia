using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CrossPlatformApplication.ViewModels;

public class GraphControl : Control
{
    private Point[]? _points;

    public void LoadData()
    {
        Random random = new Random();
        _points = new Point[500];
        for (int i = 0; i < _points.Length; i++)
        {
            _points[i] = new Point(10 + (400 - 20.0) / (_points.Length - 1) * i,
                10 + random.Next(200 - 20));
            PerformIntensiveCalculations();
        }

        InvalidateVisual();
    }

    public async Task? LoadDataAsync()
    {
        _points = new Point[200];
        Random random = new Random();

        for (int i = 0; i < 200; i++)
        {
            _points[i] = new Point(10 + (400 - 20.0) / (200 - 1) * i, 10 + random.Next(200 - 20));
            InvalidateVisual();
            await Task.Delay(1);
        }
    }

    public void ClearGraph()
    {
        _points = null;
        InvalidateVisual();
    }


    private void PerformIntensiveCalculations()
    {
        double sum = 0;

        // sum is not used, but it's needed to save calculations
        for (int i = 0; i < 1000000; i++)
            sum += Math.Sqrt(i);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.DrawLine(new Pen(Brushes.Black), new Point(10, 10), new Point(10, 200 - 10));
        context.DrawLine(new Pen(Brushes.Black), new Point(10, 200 - 10),
            new Point(400 - 10, 200 - 10));

        if (_points == null)
            return;

        for (int i = 0; i < _points.Length - 1; i++)
            context.DrawLine(new Pen(Brushes.Red, 2), _points[i], _points[i + 1]);
        foreach (Point point in _points) 
            context.DrawEllipse(Brushes.Black, null, point, 1, 1);
    }
}