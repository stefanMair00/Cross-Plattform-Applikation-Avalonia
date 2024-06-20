using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CrossPlatformApplication.ViewModels;

namespace CrossPlatformApplication;

public partial class MainWindow : Window
{
    private const string SecondsUnit = "s";
    private const string MicroSecondsUnit = "μs";
    private GraphControl? _leftGraphControl;
    private GraphControl? _rightGraphControl;

    public MainWindow()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Setup();
        StartUpLoading();

        stopwatch.Stop();

        Dispatcher.UIThread.Invoke(() =>
        {
            TextBlock? startUpTimeTextBlock = this.FindControl<TextBlock>("StartUpTimeTextBlock");
            if (startUpTimeTextBlock == null)
                return;

            startUpTimeTextBlock.Text = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 2)} {SecondsUnit}";
        });
    }

    private void Setup()
    {
        InitializeComponent();
        DataContext = this;
        _leftGraphControl = this.FindControl<GraphControl>("LeftGraphCanvas");
        _rightGraphControl = this.FindControl<GraphControl>("RightGraphCanvas");
        Icon = new WindowIcon("Resources/icon.png");
    }

    private void StartUpLoading()
    {
        _leftGraphControl?.LoadData();
        _rightGraphControl?.LoadData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void LoadData(object sender, RoutedEventArgs e)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            TextBlock? loadTimeTextBlock = this.FindControl<TextBlock>("LoadTimeTextBlock");
            if (loadTimeTextBlock == null)
                return;

            loadTimeTextBlock.Text = $"0 {SecondsUnit}";

            Grid? loadingGridVisible = this.FindControl<Grid>("LoadingGrid");
            if (loadingGridVisible == null)
                return;

            loadingGridVisible.IsVisible = true;
        });

        await Task.Delay(10);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        _leftGraphControl?.LoadData();
        _rightGraphControl?.LoadData();

        stopwatch.Stop();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            TextBlock? loadTimeTextBlock = this.FindControl<TextBlock>("LoadTimeTextBlock");
            if (loadTimeTextBlock == null)
                return;

            loadTimeTextBlock.Text = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 2)} {SecondsUnit}";

            Grid? loadingGridVisible = this.FindControl<Grid>("LoadingGrid");
            if (loadingGridVisible == null)
                return;

            loadingGridVisible.IsVisible = false;
        });
    }

    private async void StartMultiThreadingAsync(object sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            TextBlock? multiThreadingTimeTextBlock = this.FindControl<TextBlock>("MultiThreadingTimeTextBlock");
            if (multiThreadingTimeTextBlock == null)
                return;

            multiThreadingTimeTextBlock.Text = $"0 {SecondsUnit}";

            Button? isDeleteGraphEnabled = this.FindControl<Button>("DeleteGraphButton");
            if (isDeleteGraphEnabled == null)
                return;

            isDeleteGraphEnabled.IsEnabled = false;

            Button? isLoadDataEnabled = this.FindControl<Button>("LoadDataButton");
            if (isLoadDataEnabled == null)
                return;

            isLoadDataEnabled.IsEnabled = false;

            Button? isMultiThreadingEnabled = this.FindControl<Button>("MultiThreadingButton");
            if (isMultiThreadingEnabled == null)
                return;

            isMultiThreadingEnabled.IsEnabled = false;
        });

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            await Task.WhenAll(_leftGraphControl?.LoadDataAsync() ?? throw new InvalidOperationException(), _rightGraphControl?.LoadDataAsync() ?? throw new InvalidOperationException());
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }

        stopwatch.Stop();

        Dispatcher.UIThread.Invoke(() =>
        {
            TextBlock? multiThreadingTimeTextBlock = this.FindControl<TextBlock>("MultiThreadingTimeTextBlock");
            if (multiThreadingTimeTextBlock == null)
                return;

            multiThreadingTimeTextBlock.Text = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 2)} {SecondsUnit}";

            Button? isDeleteGraphEnabled = this.FindControl<Button>("DeleteGraphButton");
            if (isDeleteGraphEnabled == null)
                return;

            isDeleteGraphEnabled.IsEnabled = true;

            Button? isLoadDataEnabled = this.FindControl<Button>("LoadDataButton");
            if (isLoadDataEnabled == null)
                return;

            isLoadDataEnabled.IsEnabled = true;

            Button? isMultiThreadingEnabled = this.FindControl<Button>("MultiThreadingButton");
            if (isMultiThreadingEnabled == null)
                return;

            isMultiThreadingEnabled.IsEnabled = true;
        });
    }

    private void ClearGraph(object sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            TextBlock? deleteTimeTextBlock = this.FindControl<TextBlock>("DeleteTimeTextBlock");
            if (deleteTimeTextBlock == null)
                return;

            deleteTimeTextBlock.Text = $"0 {MicroSecondsUnit}";
        });

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        _leftGraphControl?.ClearGraph();
        Task.Delay(1);
        _rightGraphControl?.ClearGraph();

        stopwatch.Stop();

        Dispatcher.UIThread.Invoke(() =>
        {
            TextBlock? deleteTimeTextBlock = this.FindControl<TextBlock>("DeleteTimeTextBlock");
            if (deleteTimeTextBlock == null)
                return;

            deleteTimeTextBlock.Text = $"{stopwatch.Elapsed.Microseconds} {MicroSecondsUnit}";
        });
    }
}