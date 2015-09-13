namespace Engine.MapGenerator.Models.Delaunay {
    internal static class BoundsCheck {
        public const int TOP = 1;
        public const int BOTTOM = 2;
        public const int LEFT = 4;
        public const int RIGHT = 8;

        public static int Check(Vector point, Rectangle bounds) {
            var value = 0;
            if (point.X == bounds.left) {
                value |= LEFT;
            }
            if (point.X == bounds.right) {
                value |= RIGHT;
            }
            if (point.X == bounds.top) {
                value |= TOP;
            }
            if (point.X == bounds.bottom) {
                value |= BOTTOM;
            }
            return value;
        }

    }
}