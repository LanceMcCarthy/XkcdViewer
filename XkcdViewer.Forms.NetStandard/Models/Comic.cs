﻿using System.Runtime.Serialization;

namespace XkcdViewer.Forms.NetStandard.Models
{
    [DataContract]
    public class Comic
    {
        [DataMember (Name = "month")]
        public string Month { get; set; }

        [DataMember(Name = "num")]
        public int Num { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "year")]
        public string Year { get; set; }

        [DataMember(Name = "news")]
        public string News { get; set; }

        [DataMember(Name = "safe_title")]
        public string SafeTitle { get; set; }

        [DataMember(Name = "transcript")]
        public string Transcript { get; set; }

        [DataMember(Name = "alt")]
        public string Alt { get; set; }

        [DataMember(Name = "img")]
        public string Img { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "day")]
        public string Day { get; set; }
    }

}