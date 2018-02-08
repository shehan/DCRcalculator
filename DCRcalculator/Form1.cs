using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DCRcalculator
{
    public partial class Form1 : Form
    {
        List<GitCommit> gitCommits = new List<GitCommit>();
        List<AuthorDCR> authorDCRList;
        AuthorDCR authorDCR;
        GitCommit gitCommit;
        Database db;
        string inputFile;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxInput.Text))
            {
                MessageBox.Show("Input File - Required!");
                return;
            }

            inputFile = textBoxInput.Text;
            button1.Enabled = false;
            textBoxInput.Enabled = false;
            
            Thread t = new Thread(doWork);
            t.Start();
        }

        
        private void doWork()
        {
            UpdateStatus("Process Started");
            db = new Database();
            UpdateStatus("Started Reading Input");
            ReadInputFile();
            UpdateStatus("Started Calculating DCR");
            CalculateDCR();
            UpdateStatus("Done");
            MessageBox.Show("Done!");
        }


        private void CalculateDCR()
        {
            var apps = gitCommits.Select(x => x.App).Distinct().ToList();
            foreach (var app in apps)
            {                
                authorDCRList = new List<AuthorDCR>();
                var users = gitCommits.Where(x => x.App.Equals(app)).Select(u => u.AuthorEmail).Distinct().ToList();
                var appCommits = gitCommits.Where(x => x.App.Equals(app)).OrderBy(d => d.Date);

                UpdateStatus($"Processing app: {app}; Total Authors: {users.Count}; Total Commits: {appCommits.Count()};");
                foreach (var user in users)
                {
                    var userCommits = appCommits.Where(x => x.AuthorEmail.Equals(user));
                    foreach (var userCommit in userCommits)
                    {
                        var priorAppCommits = appCommits.Where(d => d.Date <= userCommit.Date).Count();
                        var priorUserCommits = userCommits.Where(d => d.Date <= userCommit.Date).Count();
                        var totalPriorAppFiles = appCommits.Where(d => d.Date <= userCommit.Date).Sum(f => f.CommitFiles);
                        var totalPriorUserFiles = userCommits.Where(d => d.Date <= userCommit.Date).Sum(f => f.CommitFiles);

                        authorDCR = new AuthorDCR();
                        authorDCR.App = app;
                        authorDCR.CommitSHA = userCommit.CommitSHA;
                        authorDCR.AuthorEmail = user;
                        authorDCR.AppCommits = priorAppCommits;
                        authorDCR.AuthorCommits = priorUserCommits;
                        authorDCR.AuthorFiles = totalPriorUserFiles;
                        authorDCR.AppFiles = totalPriorAppFiles;
                        authorDCR.Dcr = Convert.ToDouble(priorUserCommits) / Convert.ToDouble(priorAppCommits);

                        authorDCRList.Add(authorDCR);
                    }
                }

                db.BatchInsert(authorDCRList);
            }
        }



        private void ReadInputFile()
        {
            string[] fields;

            using (TextFieldParser parser = new TextFieldParser(inputFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                while (!parser.EndOfData)
                {
                    fields = parser.ReadFields();
                    gitCommit = new GitCommit();
                    gitCommit.App = fields[0].ToString();
                    gitCommit.CommitSHA = fields[1].ToString();
                    gitCommit.AuthorEmail = fields[2].ToString();
                    gitCommit.Date = new DateTime(long.Parse(fields[3].ToString()));
                    gitCommit.CommitFiles = int.Parse(fields[4].ToString());

                    gitCommits.Add(gitCommit);
                }
            }
        }

        private void UpdateStatus(string text)
        {
            if (this.textBoxLog.InvokeRequired)
            {
                UpdateStatusCallback callback = new UpdateStatusCallback(UpdateStatus);
                this.Invoke(callback, new object[] { text });
            }
            else
            {
                textBoxLog.AppendText($"{Environment.NewLine}{DateTime.Now.ToString()} - {text}");
            }
        }

        delegate void UpdateStatusCallback(string text);
    }
}
