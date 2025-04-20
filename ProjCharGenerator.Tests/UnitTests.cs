using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using generator;
using System.Collections.Generic;
using System.IO;

namespace ProjCharGenerator.Tests {
  [TestClass]
  public class BigramGeneratorTests {
    string testDir = Directory.GetCurrentDirectory();

    [TestInitialize]
    public void Setup() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      string TestContent = "1 аа 10\n2 аб 20\n3 ба 30\n4 бб 40";
      File.WriteAllText(TestBigramsFile, TestContent);
    }

    [TestCleanup]
    public void Cleanup() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      if (File.Exists(TestBigramsFile)) {
        File.Delete(TestBigramsFile);
      }
    }

    [TestMethod]
    public void BigramGenerator_LoadsDataCorrectly() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      var generator = new BigramGenerator(TestBigramsFile);

      Assert.AreEqual(4, generator.getSymCount());
      Assert.AreEqual(100, generator.getTotalWeight());
    }

    [TestMethod]
    public void BigramGenerator_GeneratesAllPossibleBigrams() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      var generator = new BigramGenerator(TestBigramsFile);
      var generated = new HashSet<string>();
      for (int i = 0; i < 1000; i++) {
        generated.Add(generator.getSym());
      }
      Assert.AreEqual(4, generated.Count);
    }

    [TestMethod]
    public void BigramText_OutputHasNoSpecSym() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      var generator = new BigramGenerator(TestBigramsFile);
      string text = "";
      for (int i = 0; i < 100; i++) {
        text += generator.getSym();
      }
      Assert.IsFalse(text.Contains(" "));
      Assert.IsFalse(text.Contains(","));
      Assert.IsFalse(text.Contains("."));
      Assert.IsFalse(text.Contains("!"));
    }

    [TestMethod]
    public void GetSym_ReturnsValuesAccordingToWeights() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      var generator = new BigramGenerator(TestBigramsFile);
      var results = new Dictionary<string, int>();

      for (int i = 0; i < 10000; i++) {
        string sym = generator.getSym();
        if (results.ContainsKey(sym))
          results[sym]++;
        else
          results.Add(sym, 1);
      }

      Assert.IsTrue(results["аа"] > 800 && results["аа"] < 1200);
      Assert.IsTrue(results["бб"] > 3800 && results["бб"] < 4200);
    }

    [TestMethod]
    public void GetWeight_ReturnsCorrectProbability() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      var generator = new BigramGenerator(TestBigramsFile);

      Assert.AreEqual(0.1f, generator.getWeight("аа"));
      Assert.AreEqual(0.2f, generator.getWeight("аб"));
      Assert.AreEqual(0.3f, generator.getWeight("ба"));
      Assert.AreEqual(0.4f, generator.getWeight("бб"));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetWeight_ThrowsForNonExistentBigram() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      var generator = new BigramGenerator(TestBigramsFile);
      generator.getWeight("xx");
    }
  }

  [TestClass]
  public class WordGeneratorTests {
    string testDir = Directory.GetCurrentDirectory();

    [TestInitialize]
    public void Setup() {
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      string TestContent = "1 слово1 t t 0,1\n2 слово2 t t 0,2\n3 слово3 t t 0,3\n4 слово4 t t 0,4";
      File.WriteAllText(TestWordsFile, TestContent);
    }

    [TestCleanup]
    public void Cleanup() {
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      if (File.Exists(TestWordsFile)) {
        File.Delete(TestWordsFile);
      }
    }

    [TestMethod]
    public void WordGenerator_LoadsDataCorrectly() {
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      var generator = new WordGenerator(TestWordsFile);

      Assert.AreEqual(4, generator.getSymCount());
      Assert.AreEqual(10, generator.getTotalWeight());
    }

    [TestMethod]
    public void WordsGenerator_GeneratesAllPossibleWords() {
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      var generator = new WordGenerator(TestWordsFile);
      var generated = new HashSet<string>();
      for (int i = 0; i < 1000; i++) {
        generated.Add(generator.getSym());
      }
      Assert.AreEqual(4, generated.Count);
    }

    [TestMethod]
    public void WordsText_OutputHasNoSpecSym() {
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      var generator = new WordGenerator(TestWordsFile);
      string text = "";
      for (int i = 0; i < 100; i++) {
        text += generator.getSym();
      }
      Assert.IsFalse(text.Contains(" "));
      Assert.IsFalse(text.Contains(","));
      Assert.IsFalse(text.Contains("."));
      Assert.IsFalse(text.Contains("!"));
    }

    [TestMethod]
    public void GetSym_ReturnsWordsAccordingToWeights() {
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      var generator = new WordGenerator(TestWordsFile);
      var results = new Dictionary<string, int>();

      int n = 10000;
      for (int i = 0; i < n; i++) {
        string word = generator.getSym();
        if (results.ContainsKey(word))
          results[word]++;
        else
          results.Add(word, 1);
      }

      Assert.IsTrue((float)results["слово2"] / n > 0.18 && (float)results["слово2"] / n < 0.22);
      Assert.IsTrue((float)results["слово3"] / n > 0.28 && (float)results["слово3"] / n < 0.32);
    }

    [TestMethod]
    public void GetWeight_ReturnsCorrectProbability() {
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      var generator = new WordGenerator(TestWordsFile);

      Assert.AreEqual(0.1f, generator.getWeight("слово1"));
      Assert.AreEqual(0.2f, generator.getWeight("слово2"));
      Assert.AreEqual(0.3f, generator.getWeight("слово3"));
      Assert.AreEqual(0.4f, generator.getWeight("слово4"));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetWeight_ThrowsForNonExistentBigram() {
      string TestBigramsFile = Path.Combine(testDir, "test_words.txt");
      var generator = new WordGenerator(TestBigramsFile);
      generator.getWeight("словоN");
    }
  }

  [TestClass]
  public class PlotTests {
    string testDir = Directory.GetCurrentDirectory();

    [TestInitialize]
    public void Setup() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      string TestBigramContent = "1 аа 10\n2 аб 20\n3 ба 30\n4 бб 40";
      string TestBigramsStatFile = Path.Combine(testDir, "test_bigrams_stat.txt");
      string TestBigramStatContent = "аа 0,1\nаб 0,2\nба 0,3\nбб 0,4";
      File.WriteAllText(TestBigramsFile, TestBigramContent);
      File.WriteAllText(TestBigramsStatFile, TestBigramStatContent);

      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      string TestWordsContent = "1 слово1 t t 0,1\n2 слово2 t t 0,2\n3 слово3 t t 0,3\n4 слово4 t t 0,4";
      string TestWordsStatFile = Path.Combine(testDir, "test_words_stat.txt");
      string TestWordsStatContent = "слово1 0,15\nслово2 0,17\nслово3 0,27\nслово4 0,3";
      File.WriteAllText(TestWordsFile, TestWordsContent);
      File.WriteAllText(TestWordsStatFile, TestWordsStatContent);
    }

    [TestCleanup]
    public void Cleanup() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      string TestBigramsStatFile = Path.Combine(testDir, "test_bigrams_stat.txt");
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      string TestWordsStatFile = Path.Combine(testDir, "test_words_stat.txt");
      if (File.Exists(TestBigramsFile)) {
        File.Delete(TestBigramsFile);
      }
      if (File.Exists(TestBigramsStatFile)) {
        File.Delete(TestBigramsStatFile);
      }
      if (File.Exists(TestWordsFile)) {
        File.Delete(TestWordsFile);
      }
      if (File.Exists(TestWordsStatFile)) {
        File.Delete(TestWordsStatFile);
      }
    }

    [TestMethod]
    public void PlotBigram_CreatesOutputFile() {
      string TestBigramsFile = Path.Combine(testDir, "test_bigrams.txt");
      string TestBigramsStatFile = Path.Combine(testDir, "test_bigrams_stat.txt");
      string TestBigramsPlotFile = Path.Combine(testDir, "test_bigrams_plot.png");

      var bigramGenerator = new BigramGenerator(TestBigramsFile);
      bigramGenerator.plot(TestBigramsStatFile, TestBigramsPlotFile);

      Assert.IsTrue(File.Exists(TestBigramsPlotFile));
      Assert.IsTrue(new FileInfo(TestBigramsPlotFile).Length > 0);
    }

    [TestMethod]
    public void PlotWords_CreatesOutputFile() {
      string TestWordsFile = Path.Combine(testDir, "test_words.txt");
      string TestWordsStatFile = Path.Combine(testDir, "test_words_stat.txt");
      string TestWordsPlotFile = Path.Combine(testDir, "test_words_plot.png");

      var wordGenerator = new WordGenerator(TestWordsFile);
      wordGenerator.plot(TestWordsStatFile, TestWordsPlotFile);

      Assert.IsTrue(File.Exists(TestWordsPlotFile));
      Assert.IsTrue(new FileInfo(TestWordsPlotFile).Length > 0);
    }
  }
}
