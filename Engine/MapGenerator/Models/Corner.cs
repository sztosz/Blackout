using System.Collections.Generic;

namespace Engine.MapGenerator.Models {
    public class Corner {
        public int index { get; set; }
        public Vector location { get; set; }

        public bool water { get; set; }
        public bool ocean { get; set; }
        public bool coast { get; set; }
        public bool mapBorder { get; set; }
        public double elevation { get; set; }
        public double moisture { get; set; }

        public HashSet<Center> touches { get; set; }
        public HashSet<Edge> protrudes { get; set; }
        public HashSet<Corner> adjacent { get; set; }

        public int riverSize { get; set; }
        public Corner LowestCorner { get; set; }
        public Corner watershed { get; set; }
        public int watershedSize { get; set; }
    }
}