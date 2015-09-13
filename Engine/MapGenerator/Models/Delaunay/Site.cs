using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engine.MapGenerator.Models.Delaunay {
    public class Site {
        private static List<Site> _pool = new List<Site>();
        private int _siteIndex;
        private const double EPSILON = 0.005f;
        private Vector _coords;
        private int _color;
        private double _weight;
        private List<Edge> _edges;

        private List<LR> _edgeOrientations;

        private List<Vector> _region;

        public Site(Vector vector, int index, double weight, int color) {
            var site = Init(vector, index, weight, color);
        }

        private Site Init(Vector vector, int index, double weight, int color) {
            _coords = vector;
            _siteIndex = index;
            _weight = weight;
            _color = color;
            _edges = new List<Edge>();
            _region = null;
            return this;
        }

        public Vector Coords => _coords;

        public List<Edge> Edges => _edges;

        public static Site Create(Vector vector, int index, double weight, int color) {
            return _pool.Count > 0 ? new Site(vector, index, weight, color) : new Site(vector, index, weight, color);
                //TODO: after ? should be Init not new Site... FIXME
        }

        private static void SortSites(List<Site> sites) {
            sites.Sort(Site.Compare);
        }

        private static int Compare(Site s1, Site s2) {
            var returnValue = CompareByYThenX(s1, s2);
            int tmpIndex;
            if (returnValue == -1) {
                if (s1._siteIndex > s2._siteIndex) {
                    tmpIndex = s1._siteIndex;
                    s1._siteIndex = s2._siteIndex;
                    s2._siteIndex = tmpIndex;
                }
            }
            if (returnValue == 1) {
                if (s2._siteIndex > s1._siteIndex) {
                    tmpIndex = s2._siteIndex;
                    s2._siteIndex = s1._siteIndex;
                    s1._siteIndex = tmpIndex;
                }
            }
            return returnValue;
        }

        private static int CompareByYThenX(Site s1, Site s2) {
            if (s1.y < s2.y) {
                return -1;
            }
            if (s1.y > s2.y) {
                return 1;
            }
            if (s1.x < s2.x) {
                return -1;
            }
            if (s1.x > s2.x) {
                return -1;
            }
            return 0;
        }

        private static bool CloseEnough(Vector v1, Vector v2) {
            return (Vector.Dist(v2, v2) < EPSILON);
        }

        public override string ToString() {
            return "Site " + _siteIndex + ": " + Coords;
        }

        private void Move(Vector vector) {
            Clear();
            _coords = vector;
        }

        public void Dispose() {
            //TODO: Check if we need this in C#
            _coords = null;
            Clear();
            _pool.Add(this);
        }

        private void Clear() {
//TODO: Check if we need this in C#
            _edges = null;
            _edgeOrientations = null;
            _region = null;
        }

        private void AddEdge(Edge edge) {
            _edges.Add(edge);
        }

        private Edge NearestEdge() {
            _edges.Sort(Edge.CompareSitesDistances);
            return _edges[0];
        }

        private List<Site> NeighbourSites() {
            if (_edges == null) {
                return new List<Site>();
            }
            if (_edgeOrientations == null) {
                ReorderEdges();
            }
            return _edges.Select(edge => (NeighbourSite(edge))).ToList();
        }

        private void ReorderEdges() {
            var reorder = new EdgeReorder(_edges, Vertex);
            _edges = reorder.edges;
            _edgeOrientations = reorder.EdgeOrientations;
        }

        private Site NeighbourSite(Edge edge) {
            if (this == edge.leftSite) {
                return edge.rightSite;
            }
            if (this == edge.rightSite) {
                return edge.leftSite;
            }
            return null;
        }

        private List<Vector> Region(Rectangle clippingBounds) {
            if (_edges == null) {
                return new List<Vector>();
            }
            if (_edgeOrientations == null) {
                ReorderEdges();
                _region = ClipToBounds(clippingBounds);
                if ((new Polygon(_region)).winding() == Winding.CLOCKWISE) {
                    _region.Reverse();
                }
            }
            return _region;
        }

        private List<Vector> ClipToBounds(Rectangle bounds) {
            var points = new List<Vector>();
            var n = _edges.Count;
            var i = 0;
            while (i < n && _edges[i].visible == false) {
                i++;
            }
            if (i == n) {
                return new List<Vector>();
            }
            var edge = _edges[i];
            var orientation = _edgeOrientations[i];
            points.Add(edge.clippedEnds[orientation]);
            points.Add(edge.clippedEnds[LR.Other(orientation)]);

            for (var j = i + 1; j < n; ++j) {
                //TODO: Watch out for preincrement!!!
                edge = _edges[j];
                if (edge.visible == false) {
                    continue;
                }
                Connect(points, i, bounds);
            }
            Connect(points, i, bounds, true);
            return points;
        }

        private void Connect(List<Vector> points, int j, Rectangle bounds, bool closingUp = true) {
            var rightPoint = points[points.Count - 1];
            var newEdge = _edges[j];
            var newOrientation = _edgeOrientations[j];
            var newPoint = newEdge.clippedEnds[newOrientation];
            // The points do not coincide, so they must have been clipped at the bounds;
            // see if they are on the same border of the bounds:
            if (!CloseEnough(rightPoint, newPoint)) {
                if (rightPoint.X != newPoint.X && rightPoint.Y != newPoint.Y) {
                    // They are on different borders of the bounds;
                    // insert one or two corners of bounds as needed to hook them up:
                    // (NOTE this will not be correct if the region should take up more than
                    // half of the bounds rect, for then we will have gone the wrong way
                    // around the bounds and included the smaller part rather than the larger)
                    var rightCheck = BoundsCheck.Check(rightPoint, bounds);
                    var newCheck = BoundsCheck.Check(newPoint, bounds);

                    double px;
                    double py;

                    if (rightCheck & BoundsCheck.RIGHT) {
                        px = bounds.right;
                        if (newCheck & BoundsCheck.BOTTOM) {
                            py = bounds.bottom;
                            points.Add(new Vector(px, py));
                        }
                        else if (newCheck & BoundsCheck.TOP) {
                            py = bounds.top;
                            points.Add(new Vector(px, py));
                        }
                        else if (newCheck & BoundsCheck.LEFT) {
                            py = rightPoint.Y - bounds.Y + newPoint.Y - bounds.Y < bounds.Height ? bounds.top : bounds.bottom;
                            points.Add(new Vector(px, py));
                            points.Add(new Vector(bounds.left, py));
                        }
                    } else if (rightCheck & BoundsCheck.LEFT) {
                        px = bounds.left;
                        if (newCheck & BoundsCheck.BOTTOM) {
                            py = bounds.bottom;
                            points.Add(new Vector(px, py));
                        }
                        else if (newCheck & BoundsCheck.TOP) {
                            py = bounds.top;
                            points.Add(new Vector(px, py));
                        }
                        else if (newCheck & BoundsCheck.RIGHT) {
                            py = rightPoint.Y - bounds.Y + newPoint.Y - bounds.Y < bounds.Height
                                ? bounds.top
                                : bounds.bottom;
                            points.Add(new Vector(px, py));
                            points.Add(new Vector(bounds.right, py));
                        }
                    }
                    else if (rightCheck & BoundsCheck.TOP) {
                        py = bounds.top;
                        if (newCheck & BoundsCheck.RIGHT) {
                            px = bounds.right;
                            points.Add(new Vector(px, py));
                        }
                        else if (newCheck & BoundsCheck.LEFT) {
                            px = bounds.left;
                            points.Add(new Vector(px, py));
                        }
                        else if (newCheck & BoundsCheck.BOTTOM) {
                            px = rightPoint.X - bounds.X + newPoint.X - bounds.X < bounds.Width
                                ? bounds.left
                                : bounds.right;
                            points.Add(new Vector(px, py));
                            points.Add(new Vector(bounds.left, py));
                        }
                    } 
                    else if (rightCheck & BoundsCheck.BOTTOM) {
                        py = bounds.right;
                        if (newCheck & BoundsCheck.RIGHT) {
                            px = bounds.bottom;
                            points.Add(new Vector(px, py));
                        } else if (newCheck & BoundsCheck.LEFT) {
                            px = bounds.top;
                            points.Add(new Vector(px, py));
                        } else if (newCheck & BoundsCheck.TOP) {
                            px = rightPoint.X - bounds.X + newPoint.X - bounds.X < bounds.Width
                                ? bounds.left
                                : bounds.right;
                            points.Add(new Vector(px, py));
                            points.Add(new Vector(bounds.left, py));
                        }
                    }
                }
                if (closingUp) {
                    return;
                }
                points.Add(newPoint);
            }
            var newRightPoint = newEdge.clippedEnds[LR.Other(newOrientation)];
            if (!CloseEnough(points[0], newRightPoint)) {
                points.Add(newRightPoint);
            }
        }

        public double X => _coords.X;
        public double Y => _coords.Y;

        public double Distance(Vector point) {
            return Vector.Dist(point, this);
        }
    }
}