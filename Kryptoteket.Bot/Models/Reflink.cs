﻿using System.Text.Json.Serialization;

namespace Kryptoteket.Bot.Models
{
    public class Reflink
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public bool Approved { get; set; }
    }
}
