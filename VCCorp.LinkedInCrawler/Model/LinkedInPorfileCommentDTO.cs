using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCCorp.LinkedInCrawler.Model
{
    public class LinkedInPorfileCommentDTO
    {
        public string author_name { get; set; }
        public string profile_pic { get; set; }
        public int followers { get; set; }
        public string country { get; set; }
        public string experiences { get;set; }
        public List<Experiences> list_exp { get; set; }
        public List<Skills> list_skills { get; set; }

    }
    public class Experiences
    {
        
        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }
        public string company { get; set; }
        public string description { get; set; }
        public string skills { get; set; }
    }

    public class Skills
    {
        
        public string skill_name { get; set; }
    }
}
