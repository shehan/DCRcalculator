using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCRcalculator
{
    class AuthorDCR
    {
        string authorEmail, app, commitSHA;
        int authorCommits, appCommits, authorFiles, appFiles;
        double dcr_v1, dcr_v2;

        public string AuthorEmail { get => authorEmail; set => authorEmail = value; }
        public string App { get => app; set => app = value; }
        public string CommitSHA { get => commitSHA; set => commitSHA = value; }
        public int AuthorCommits { get => authorCommits; set => authorCommits = value; }
        public int AppCommits { get => appCommits; set => appCommits = value; }
        public int AuthorFiles { get => authorFiles; set => authorFiles = value; }
        public int AppFiles { get => appFiles; set => appFiles = value; }
        public double Dcr_v1 { get => dcr_v1; set => dcr_v1 = value; }
        public double Dcr_v2 { get => dcr_v2; set => dcr_v2 = value; }
    }
}
