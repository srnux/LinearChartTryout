using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartingTest
{
  public partial class LinearChart : Form
  {
    private Thread cpuThread;
    private Thread cpuThread2;
    private Thread cpuThread3;
    private Thread cpuThread4;
    private Thread cpuThread5;

    private double[] cpuArray=new double[60];
    private ConcurrentDictionary<string, double[]> counters= new ConcurrentDictionary<string, double[]>();
    public LinearChart()
    {
      randomColor = new Random();
      InitializeComponent();
    }

    private void GetPerformanceCounters(string categoryName, string counterName, string instanceName, string seriesName)
    {
      var cpuPerfCounter = new PerformanceCounter(categoryName, counterName, instanceName);

      while (true)
      {
        if (!counters.Any(p => p.Key == seriesName))
        {
          var cpuArray = new double[60];
          counters[seriesName] = cpuArray;
          //counters.Add(seriesName, cpuArray);
        }

        var localDoubles = counters[seriesName];
        localDoubles[localDoubles.Length - 1] = Math.Round(cpuPerfCounter.NextValue(), 0);

        Array.Copy(localDoubles, 1, localDoubles, 0, localDoubles.Length - 1);
        
        //cpuArray[cpuArray.Length - 1] = Math.Round(cpuPerfCounter.NextValue(), 0);

        //Array.Copy(cpuArray, 1, cpuArray, 0, cpuArray.Length - 1);

        counters[seriesName]=localDoubles;
        

        if (chart1.IsHandleCreated)
        {
          this.Invoke(new MethodInvoker(() => this.UpdateCpuChart(seriesName)));
        }
        else
        {
          //......
        }

        Thread.Sleep(1000);
      }
    }

    private void UpdateCpuChart(string seriesName)
    {
      if (chart1.Series.FindByName(seriesName) ==null )
      {

        var series1 = new Series
        {
          Name = seriesName,
          Color = GetRandomColor(),
          IsVisibleInLegend = false,
          IsXValueIndexed = true,
          ChartType = SeriesChartType.Line
        };
        chart1.Series.Add(series1);
      }

      chart1.Series[seriesName].Points.Clear();

      if (counters.Any(p => p.Key == seriesName))
      {
        var localDoubles = counters[seriesName];
        for (int i = 0; i < localDoubles.Length - 1; ++i)
        {
          chart1.Series[seriesName].Points.AddY(localDoubles[i]);
        }
      }

    }

    private Random randomColor;
    private Color GetRandomColor()
    {
      return Color.FromArgb(randomColor.Next(0, 255), randomColor.Next(0, 255), randomColor.Next(0, 255));
    }


    private void Form1_Load(object sender, EventArgs e)
    {
      //chart1.Series.Clear();
      //var series1 = new Series
      //{
      //  Name = "Series1",
      //  Color = System.Drawing.Color.Green,
      //  IsVisibleInLegend = false,
      //  IsXValueIndexed = true,
      //  ChartType = SeriesChartType.Line
      //};

      //chart1.Series.Add(series1);

      //for (var i = 0; i < 100; i++)
      //{
      //  series1.Points.AddXY(i, f(i));
      //}
      //chart1.Invalidate();
      cpuThread = new Thread( ()=> GetPerformanceCounters("Processor Information", "% Processor Time", "_Total","Processor Time"))
      {
        IsBackground = true
      };
      cpuThread.Start();

      //cpuThread2 = new Thread(() => GetPerformanceCounters("Processor Information", "% C0 Time", "_Total", "Core 1"))
      //{
      //  IsBackground = true
      //};
      //cpuThread2.Start();

      cpuThread3 = new Thread(() => GetPerformanceCounters("Processor Information", "% C1 Time", "_Total", "Core 2"))
      {
        IsBackground = true
      };
      cpuThread3.Start();

      cpuThread4 = new Thread(() => GetPerformanceCounters("Processor Information", "% C2 Time", "_Total", "Core 3"))
      {
        IsBackground = true
      };
      cpuThread4.Start();

      cpuThread5 = new Thread(() => GetPerformanceCounters("Processor Information", "% C3 Time", "_Total", "Core 4"))
      {
        IsBackground = true
      };
      cpuThread5.Start();
    }
  }


}


