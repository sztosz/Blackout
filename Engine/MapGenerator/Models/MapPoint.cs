namespace Engine.MapGenerator.Structs {
    public struct MapPoint {
        public MapPoint(double x, double y) {
            X = x;
            Y = y;
        }

        public double X { get; private set; }

        public double Y { get; private set; }
    }
}