namespace Engine.MapGenerator.Models {
    public class Edge {
        public int index { get; set; }
        public Center d1 { get; set; }
        public Center d2 { get; set; }
        public Corner c1 { get; set; }
        public Corner c2 { get; set; }
        public Vector midpoint { get; set; }
        public int riverSize { get; set; }
    }
}