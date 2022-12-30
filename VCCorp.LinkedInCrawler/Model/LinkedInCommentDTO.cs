using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCorp.LinkedInCrawler.Model
{
    public class LinkedInCommentDTO
    {
        public string comment { get; set; }
        public string author_id { get; set; }
        public string author_name { get; set; }
        public string job { get; set; }
        public int status { get; set; }
    }

}
