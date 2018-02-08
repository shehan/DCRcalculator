using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCRcalculator
{
    class GitCommit
    {
        string app, commitSHA, authorEmail;
        DateTime date;
        int commitFiles;

        public string App { get => app; set => app = value; }
        public string CommitSHA { get => commitSHA; set => commitSHA = value; }
        public string AuthorEmail { get => authorEmail; set => authorEmail = value; }
        public DateTime Date { get => date; set => date = value; }
        public int CommitFiles { get => commitFiles; set => commitFiles = value; }
    }
}
