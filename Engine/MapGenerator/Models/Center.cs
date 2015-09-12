using System.Collections.Generic;

namespace Engine.MapGenerator.Models {
    public class Center {
        public int index { get; set; }
        public Vector location { get; set; }
        public bool water { get; set; }
        public bool ocean { get; set; }
        public bool coast { get; set; }
        public bool mapBorder { get; set; }
        public Biomes biome { get; set; }
        public double elevation { get; set; }
        public double moisture { get; set; }

        public HashSet<Center> neighbors { get; set; }
        public HashSet<Edge> borders { get; set; }
        public HashSet<Corner> corners { get; set; }
    }
}