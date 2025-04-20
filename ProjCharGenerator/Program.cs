using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScottPlot;

namespace generator {
  class BigramGenerator {
    private List<string> stringData = new List<string>();
    private List<int> weightData = new List<int>();
    private int size = 0;
    private Random random = new Random();
    public BigramGenerator(string filePath) {
      List<string> lines = File.ReadAllLines(filePath).ToList();

      foreach (string line in lines) {
        var listLine = line.Split(" ");
        stringData.Add(listLine[1]);
        weightData.Add(int.Parse(listLine[2]));
        size += int.Parse(listLine[2]);
      }
    }
    public string getSym() {
      int num = random.Next(0, size);
      int current = 0;
      for (int i = 0; i < weightData.Count; i++) {
        current += weightData[i];
        if (num <= current)
          return stringData[i];
      }
      return "";
    }
    public float getWeight(string bigram) {
      return (float)weightData[stringData.IndexOf(bigram)] / size;
    }
  }
  class WordGenerator {
    private List<string> stringData = new List<string>();
    private List<int> weightData = new List<int>();
    private int size;
    private Random random = new Random();
    public WordGenerator(string filePath) {
      List<string> lines = File.ReadAllLines(filePath).ToList();

      foreach (string line in lines) {
        var listLine = line.Split(" ");
        stringData.Add(listLine[1]);
        weightData.Add((int)(double.Parse(listLine[4]) * 10));
        size += (int)(double.Parse(listLine[4]) * 10);
      }
    }
    public string getSym() {
      int num = random.Next(0, size);
      int current = 0;
      for (int i = 0; i < weightData.Count; i++) {
        current += weightData[i];
        if (num <= current)
          return stringData[i];
      }
      return "";
    }
    public float getWeight(string word) {
      return (float)weightData[stringData.IndexOf(word)] / size;
    }
  }
  class Program {
    static void bigramText() {
      string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
      string bigramsFile = Path.Combine(projectDirectory, "bigrams.txt");
      string gen1File = Path.Combine(projectDirectory, "..", "Results", "gen-1.txt");
      string gen1StatFile = Path.Combine(projectDirectory, "..", "Results", "gen-1_stat.txt");

      var bigramGenerator = new BigramGenerator(bigramsFile);
      SortedDictionary<string, int> bigramStat = new SortedDictionary<string, int>();
      string text = "";
      for (int i = 0; i < 1000; i++) {
        string str = bigramGenerator.getSym();
        if (bigramStat.ContainsKey(str))
          bigramStat[str]++;
        else
          bigramStat.Add(str, 1);
        text += str;
      }
      File.WriteAllText(gen1File, text);
      Console.Write(text);

      List<string> stat = new List<string>();
      Console.Write('\n');
      foreach (KeyValuePair<string, int> entry in bigramStat) {
        Console.WriteLine("{0} - {1}", entry.Key, entry.Value / 1000.0);
        stat.Add($"{entry.Key} {(float)entry.Value / 1000}");
      }
      File.WriteAllLines(gen1StatFile, stat);
    }
    static void wordText() {
      string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
      string wordsFile = Path.Combine(projectDirectory, "words.txt");
      string gen2File = Path.Combine(projectDirectory, "..", "Results", "gen-2.txt");
      string gen2StatFile = Path.Combine(projectDirectory, "..", "Results", "gen-2_stat.txt");

      var wordsGenerator = new WordGenerator(wordsFile);
      SortedDictionary<string, int> wordStat = new SortedDictionary<string, int>();
      string text = "";
      for (int i = 0; i < 1000; i++) {
        string str = wordsGenerator.getSym();
        if (wordStat.ContainsKey(str))
          wordStat[str]++;
        else
          wordStat.Add(str, 1);
        text += str + " ";
      }
      File.WriteAllText(gen2File, text);
      Console.Write(text);

      List<string> stat = new List<string>();
      Console.Write('\n');
      foreach (KeyValuePair<string, int> entry in wordStat) {
        Console.WriteLine($"{entry.Key} - {entry.Value / 1000.0} - {wordsGenerator.getWeight(entry.Key)}");
        stat.Add($"{entry.Key} {(float)entry.Value / 1000}");
      }
      File.WriteAllLines(gen2StatFile, stat);
    }

    static void plotBigram(string statFileName, string bigramFileName, string outputFileName) {
      var bigramData = File.ReadAllLines(statFileName).Take(100).ToArray();
      var gen = new BigramGenerator(bigramFileName);

      var bigrams = bigramData
          .Where(line => !string.IsNullOrWhiteSpace(line))
          .Select(line => {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return new {
              Bigram = parts[0],
              ActualFrequency = float.Parse(parts[1].Replace(",", "."),
                    System.Globalization.CultureInfo.InvariantCulture)
            };
          })
          .OrderByDescending(x => x.ActualFrequency)
          .ToList();

      string[] labels = bigrams.Select(x => x.Bigram).ToArray();
      double[] actualValues = bigrams.Select(x => (double)x.ActualFrequency).ToArray();
      double[] expectedValues = bigrams.Select(x => (double)gen.getWeight(x.Bigram)).ToArray();
      double[] positions = Enumerable.Range(0, labels.Length).Select(x => (double)x * 3).ToArray();

      Plot plt = new();

      var actualBars = plt.Add.Bars(positions.Select(x => x - 0.5), actualValues);
      actualBars.LegendText = "Реальная частота";
      actualBars.Color = Colors.Blue;

      var expectedBars = plt.Add.Bars(positions.Select(x => x + 0.5).ToArray(), expectedValues);
      expectedBars.LegendText = "Ожидаемая частота";
      expectedBars.Color = Colors.Red;

      plt.Axes.Bottom.SetTicks(positions.Select(x => x + 0.2).ToArray(), labels);

      plt.Title("Реальная и ожидаемая частота биграмм (Первые 100 значений)", size: 60);
      plt.XLabel(" ", size: 40);
      plt.YLabel("Частота", size: 40);

      plt.Axes.AutoScale();
      plt.Axes.SetLimitsY(0, plt.Axes.GetLimits().YRange.Max);

      plt.ShowLegend(Alignment.MiddleRight);

      plt.SavePng(outputFileName, 2000, 500);
    }
    static void plotWords(string statFileName, string wordsFileName, string outputFileName) {
      var wordData = File.ReadAllLines(statFileName).Take(100).ToArray();
      var gen = new WordGenerator(wordsFileName);

      var words = wordData
          .Where(line => !string.IsNullOrWhiteSpace(line))
          .Select(line => {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return new {
              Word = parts[0],
              ActualFrequency = float.Parse(parts[1].Replace(",", "."),
                    System.Globalization.CultureInfo.InvariantCulture)
            };
          })
          .OrderByDescending(x => x.ActualFrequency)
          .ToList();

      string[] labels = words.Select(x => x.Word).ToArray();
      double[] actualValues = words.Select(x => (double)x.ActualFrequency).ToArray();
      double[] expectedValues = words.Select(x => (double)gen.getWeight(x.Word)).ToArray();
      double[] positions = Enumerable.Range(0, labels.Length).Select(x => (double)x * 3).ToArray();

      Plot plt = new();

      var actualBars = plt.Add.Bars(positions.Select(x => x - 0.5), actualValues);
      actualBars.LegendText = "Реальная частота";
      actualBars.Color = Colors.Blue;

      var expectedBars = plt.Add.Bars(positions.Select(x => x + 0.5).ToArray(), expectedValues);
      expectedBars.LegendText = "Ожидаемая частота";
      expectedBars.Color = Colors.Red;

      plt.Axes.Bottom.SetTicks(positions.Select(x => x + 0.2).ToArray(), labels);

      plt.Axes.Bottom.TickLabelStyle.Rotation = 70;
      plt.Axes.Bottom.TickLabelStyle.OffsetY = 40;
      plt.Title("Реальная и ожидаемая частота слов (Первые 100 значений)", size: 60);
      plt.XLabel(" ", size: 40);
      plt.YLabel("Частота", size: 40);

      plt.Axes.AutoScale();
      plt.Axes.SetLimitsY(0, plt.Axes.GetLimits().YRange.Max);

      plt.ShowLegend(Alignment.MiddleRight);

      plt.SavePng(outputFileName, 2000, 500);
    }
    static void Main(string[] args) {
      bigramText();
      wordText();

      string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
      string bigramsFile = Path.Combine(projectDirectory, "bigrams.txt");
      string gen1StatFile = Path.Combine(projectDirectory, "..", "Results", "gen-1_stat.txt");
      string plot1File = Path.Combine(projectDirectory, "..", "Results", "gen-1.png");
      plotBigram(gen1StatFile, bigramsFile, plot1File);

      string wordsFile = Path.Combine(projectDirectory, "words.txt");
      string gen2StatFile = Path.Combine(projectDirectory, "..", "Results", "gen-2_stat.txt");
      string plot2File = Path.Combine(projectDirectory, "..", "Results", "gen-2.png");
      plotWords(gen2StatFile, wordsFile, plot2File);
    }
  }
}

