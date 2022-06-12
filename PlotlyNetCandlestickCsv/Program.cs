using System;
using Plotly.NET;
using Plotly.NET.TraceObjects;
using Microsoft.FSharp.Core;
using Plotly.NET.LayoutObjects;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;

namespace PlotlyNetCandlestickCsv
{
    public class CsvRow
    {
        [CsvHelper.Configuration.Attributes.Index(0)] public long Time { get; set; }
        [CsvHelper.Configuration.Attributes.Index(1)] public decimal Open { get; set; }
        [CsvHelper.Configuration.Attributes.Index(2)] public decimal High { get; set; }
        [CsvHelper.Configuration.Attributes.Index(3)] public decimal Low { get; set; }
        [CsvHelper.Configuration.Attributes.Index(4)] public decimal Close { get; set; }
    }

    public class Candle
    {
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // From TradingView CSV export

            var csv_items =
                new CsvReader(
                    new StreamReader(@"..\..\..\SP_SPX, 1D.csv"),
                    new CsvConfiguration(CultureInfo.InvariantCulture))
                .GetRecords<CsvRow>()
                .ToList();

            var items = csv_items.Select(item =>
                new Candle()
                {
                    DateTime = DateTimeOffset.FromUnixTimeSeconds(item.Time).UtcDateTime.Date,
                    Open = item.Open,
                    High = item.High,
                    Low = item.Low,
                    Close = item.Close
                });

            var seq = items.Select(elt =>
                Tuple.Create(
                    elt.DateTime,
                    StockData.Create((double)elt.Open, (double)elt.High, (double)elt.Low, (double)elt.Close)
                ));

            Chart2D.Chart.Candlestick<string>(seq)

                .WithYAxis(LinearAxis.init<IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible>(
                    FixedRange: false))

                //.WithYAxis(LinearAxis.init(FixedRange: false))

                .WithConfig(Config.init(Responsive: true))
                .WithSize(1800, 900)

                //.WithConfig(Config.init(Responsive: true, Autosizable: true))

                .WithTitle("SPX")

                .Show();
        }
    }
}
