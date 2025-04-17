using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace generator {
  class CharGenerator {
    private string syms = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя";
    private char[] data;
    private int size;
    private Random random = new Random();
    public CharGenerator() {
      size = syms.Length;
      data = syms.ToCharArray();
    }
    public char getSym() {
      return data[random.Next(0, size)];
    }
  }

  class BigramGenerator {
    private List<string> data = new List<string>();
    private int size;
    private Random random = new Random();
    public BigramGenerator(string filePath) {
      List<string> lines = File.ReadAllLines(filePath).ToList();

      foreach (string line in lines) {
        var listLine = line.Split(" ");
        for (int i = 0; i < int.Parse(listLine[2]); i++) {
          data.Add(listLine[1]);
        }
      }
      size = data.Count;
    }
    public string getSym() {
      return data[random.Next(0, size)];
    }
  }
  class WordGenerator {
    private List<string> data = new List<string>();
    private int size;
    private Random random = new Random();
    public WordGenerator(string filePath) {
      List<string> lines = File.ReadAllLines(filePath).ToList();

      int summ = 0;
      foreach (string line in lines) {
        var listLine = line.Split(" ");
        int n = (int)(float.Parse(listLine[4]) * 10);
        summ += n;
        for (int i = 0; i < n; i++) {
          data.Add(listLine[1]);
        }
      }
      size = data.Count;
    }
    public string getSym() {
      return data[random.Next(0, size)];
    }
  }
  class Program {
    static void charText() {
      CharGenerator gen = new CharGenerator();
      SortedDictionary<char, int> stat = new SortedDictionary<char, int>();
      for (int i = 0; i < 1000; i++) {
        char ch = gen.getSym();
        if (stat.ContainsKey(ch))
          stat[ch]++;
        else
          stat.Add(ch, 1);
        Console.Write(ch);
      }
      Console.Write('\n');
      foreach (KeyValuePair<char, int> entry in stat) {
        Console.WriteLine("{0} - {1}", entry.Key, entry.Value / 1000.0);
      }
    }
    static void bigramText() {
      string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
      string bigramsFile = Path.Combine(projectDirectory, "bigrams.txt");
      string gen1File = Path.Combine(projectDirectory, "..", "Results", "gen-1.txt");

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
      Console.Write('\n');
      foreach (KeyValuePair<string, int> entry in bigramStat) {
        Console.WriteLine("{0} - {1}", entry.Key, entry.Value / 1000.0);
      }
    }
    static void wordText() {
      string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
      string wordsFile = Path.Combine(projectDirectory, "words.txt");
      string gen2File = Path.Combine(projectDirectory, "..", "Results", "gen-2.txt");

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
      Console.Write('\n');
      foreach (KeyValuePair<string, int> entry in wordStat) {
        Console.WriteLine("{0} - {1}", entry.Key, entry.Value / 1000.0);
      }
    }
    static void Main(string[] args) {
      charText();

      bigramText();

      wordText();
    }
  }
}

