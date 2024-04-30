using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Comparatizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly char[] sep = { ' ' };

        readonly Logger logger = new Logger();

        private List<string> QAText;
        private List<string> ScribeText;
        Dictionary<int, bool> QAMatches;
        Dictionary<int, bool> ScribeMatches;

        SettingsWindow SettingsWindow;
        DebugWindow DebugWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            SettingsWindow = new SettingsWindow();
            DebugWindow = new DebugWindow(logger);
        }

        private bool GetOuterMatches()
        {
            logger.WriteLine($"Finding Outer Matches");
            logger.IncrementTab();

            int qi = 0;
            int FirstScribeIndex = -1;
            int LastScribeIndex = -1;

            bool foundMatch = false;

            //Find first word in QAText that appears in ScribeText
            while (qi < QAText.Count) 
            {
                logger.WriteLine($"Searching for {QAText[qi]} @ {qi}", 1);
                if (ScribeText.Contains(QAText[qi]))
                {
                    FirstScribeIndex = ScribeText.IndexOf(QAText[qi]);
                    foundMatch = true;
                    break;
                }
                qi++;
                QAMatches[qi] = true;
            }

            //If no match by the time we iterate the whole text once, return false
            if (!foundMatch) { return foundMatch; }

            //Find last word in QAText that appears in ScribeText
            qi = QAText.Count-1;
            while (qi >= 0) 
            {
                logger.WriteLine($"Searching for {QAText[qi]} @ {qi}", 1);
                if (!ScribeText.Contains(QAText[qi]))
                {
                    qi--;
                    QAMatches[qi] = true;
                    continue;
                }
                LastScribeIndex = ScribeText.LastIndexOf(QAText[qi]);
                break;
            }

            //Mark words at beginning and end of scribe text as different
            for (int i = 0; i < FirstScribeIndex; i++) { ScribeMatches[i] = true; }
            for (int i = LastScribeIndex; i < ScribeText.Count; i++) { ScribeMatches[i] = true; }

            logger.DecrementTab();
            return foundMatch;
        }

        private bool FindLongestMatch ()
        {
            logger.WriteLine("Finding Longest Common Match");
            logger.IncrementTab();

            List<int> LongestQAMatches = new List<int>();
            List<int> LongestScribeRange = new List<int>();
            bool foundMatch = false;

            int qi = 0;
            int longest = 0;

            while (qi < QAText.Count)
            {
                if (QAMatches[qi]) { qi++; continue; }
                logger.WriteLine($"QA Index is {qi} : {QAText[qi]}", 1);
                
                if (!ScribeText.Contains(QAText[qi])) { qi++; continue; }
                logger.WriteLine($"ScribeText contains {QAText[qi]}", 1);

                int si = 0;
                while (si < ScribeText.Count)
                {
                    if (ScribeMatches[si]) { si++; continue; }
                    logger.WriteLine($"Scribe Index is {si} : {ScribeText[si]}", 1);
                    logger.IncrementTab();

                    int length = 0;
                    List<int> tempQAMatches = new List<int>();
                    List<int> tempScribeMatches = new List<int>();

                    while (si + length < ScribeText.Count & qi + length < QAText.Count)
                    {
                        logger.WriteLine($"Comparing {QAText[qi + length]} to {ScribeText[si + length]}", 1);

                        if (QAText[qi+length] != ScribeText[si+length]) { break; }

                        tempQAMatches.Add(qi+length);
                        tempScribeMatches.Add(si+length);
                        logger.WriteLine($"Adding {qi+length} to tempQAMatches. Adding {si+length} to tempScribeMatches.", 1);

                        length++;
                        foundMatch = true;

                        logger.WriteLine($"Current length is {length}", 1);

                        if (length > longest)
                        {
                            logger.WriteLine($"Current Longest is {length}", 1);
                            longest = length;

                            LongestQAMatches = tempQAMatches;
                            LongestScribeRange = tempScribeMatches;

                            logger.WriteLine($"Setting temp range to longest", 1);
                        }
                    }
                    logger.DecrementTab();
                    si++;
                }
                qi++;
            }

            logger.StartMessage("Found and removing Longest QA Range @ ");
            foreach (int d in LongestQAMatches.OrderByDescending(x => x)) 
            { 
                logger.AppendMessage($" {d}");
                QAMatches[d] = true;
            }

            logger.PushMessage(1);
            logger.StartMessage("Found and removing Longest Scribe Range @");
            foreach (int d in LongestScribeRange.OrderByDescending(x => x))
            {
                logger.AppendMessage($" {d}");
                ScribeMatches[d] = true;
            }

            logger.PushMessage(1);
            logger.DecrementTab();

            return foundMatch;
        }

        private void FindAllMatches ()
        {
            logger.WriteLine("Finding All Common Matches");
            logger.IncrementTab();

            while (FindLongestMatch())
            {
                logger.WriteLine("New Text:", 1);
                logger.IncrementTab();

                logger.StartMessage("QA: ");
                for (int i = 0; i < QAText.Count; i++)
                {
                    if (!QAMatches[i]) logger.AppendMessage($"{QAText[i]} ");
                }

                logger.PushMessage(1);
                logger.StartMessage("Scribe: ");
                for (int i = 0; i < ScribeText.Count; i++)
                {
                    if (!ScribeMatches[i]) logger.AppendMessage($"{ScribeText[i]} ");
                }

                logger.PushMessage(1);
                logger.DecrementTab();

                logger.WriteLine("Indices to remove include:", 1);
                logger.IncrementTab();

                logger.StartMessage("QA: ");
                foreach (var match in QAMatches) 
                { 
                    if (match.Value) logger.AppendMessage($"{match.Key} "); 
                }

                logger.PushMessage(1);
                logger.StartMessage("Scribe: ");
                foreach (var match in ScribeMatches)
                {
                    if (match.Value) logger.AppendMessage($"{match.Key} ");
                }

                logger.PushMessage(1);
                logger.DecrementTab();
            }
            logger.DecrementTab();
        }

        private void PrepareComparison()
        {
            string QAString = null;
            if (rtbQAText.Document.ContentStart != null)
            { QAString = new TextRange(rtbQAText.Document.ContentStart, rtbQAText.Document.ContentEnd).Text.Trim(); }

            string ScribeString = null;
            if (rtbScribeText.Document.ContentStart != null)
            { ScribeString = new TextRange(rtbScribeText.Document.ContentStart, rtbScribeText.Document.ContentEnd).Text.Trim(); }

            QAText = new List<string>(QAString.Split(sep, StringSplitOptions.RemoveEmptyEntries));
            ScribeText = new List<string>(ScribeString.Split(sep, StringSplitOptions.RemoveEmptyEntries));

            //Dict<int, bool> = Index in QATextArray. if (bool) {print in red}
            QAMatches = new Dictionary<int, bool>();
            for (int i = 0; i < QAText.Count; i++) { QAMatches.Add(i, false); }

            ScribeMatches = new Dictionary<int, bool>();
            for (int i = 0; i < ScribeText.Count; i++) { ScribeMatches.Add(i, false); }
        }


        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            logger.WriteLine($"Comparing Text");
            logger.IncrementTab();

            PrepareComparison();

            if (GetOuterMatches()) { FindAllMatches(); }

            rtbQAText.Document.Blocks.Clear();
            rtbScribeText.Document.Blocks.Clear();

            for (int c = 0; c < QAText.Count; c++)
            {
                if (!QAMatches[c]) { rtbQAText.AppendText(QAText[c], "blue"); }
                else { rtbQAText.AppendText(QAText[c], "Black"); }

                rtbQAText.AppendText(" ");
            }

            for (int c = 0; c < ScribeText.Count; c++)
            {
                if (!ScribeMatches[c]) { rtbScribeText.AppendText(ScribeText[c], "red"); }
                else { rtbScribeText.AppendText(ScribeText[c], "Black"); }

                rtbScribeText.AppendText(" ");
            }
            logger.DecrementTab();
            logger.WriteLine("Compared Text");
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow.Show();
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            DebugWindow.Show();
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DebugWindow.Close();
            SettingsWindow.Close();
        }
    }

    public static class Extensions
    {
        public static void AppendText(this RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color));
            }
            catch (FormatException) { }
        }
    }

    public class Logger
    {
        private TextBox output;
        private StringBuilder sb;
        private bool writeToOutput;
        private int verbosity;
        private int tablevel;

        // Verbosity:
        // 0 = Critical Errors
        // 1 = Detailed Code Flow
        // 2 = Shallow Code Flow

        public Logger()
        {
            sb = new StringBuilder();
            writeToOutput = false;
            verbosity = 2;
            tablevel = 0;
        }

        public void Write(string message, int infoLevel = 2)
        {
            if (infoLevel > verbosity) { return; }
            if (!writeToOutput) { return; }

            output.AppendText(message);
        }
        public void WriteLine(string message = "", int infoLevel = 2)
        {
            if (infoLevel > verbosity) { return; }
            if (!writeToOutput) { return; }

            string tabs = "";
            for (int i = 0; i <= tablevel; i++)
            {
                tabs += "   ";
            }

            string currentTime = DateTime.Now.ToString("g");

            message = "[" + currentTime + "]" + tabs + message + "\n";
            output.AppendText(message);
        }

        public void StartMessage(string message) 
        {
            sb.Clear(); 
            sb.Append(message); 
        }
        public void AppendMessage(string message) 
        {
            sb.Append(message); 
        }
        public void AppendLineToMessage(string message)
        {
            string tabs = "";
            for (int i = 0; i <= tablevel; i++)
            {
                tabs += "   ";
            }

            string currentTime = DateTime.Now.ToString("g");

            sb.Append("\n" + "[" + currentTime + "]" + tabs + message);
        }
        public void PushMessage(int infoLevel = 2) 
        { 
            WriteLine(sb.ToString(), infoLevel); 
        }
        public void IncrementTab() { tablevel++; }
        public void DecrementTab() { if (tablevel > 0) { tablevel--; } }

        public void SetOutput(TextBox output) 
        { 
            this.output = output;
        }

        public void SetVerbosity(int v) { verbosity = v; }
        public void SetTabLevel(int tl) { tablevel = tl; }
        public void SetWriteToOutput(bool wto) { writeToOutput = wto; }
    }
}
