using System;
using System.Collections;
using System.Collections.Generic;
using Engine.MapGenerator.Models;
using FortuneVoronoi;
using Vector = Engine.MapGenerator.Models.Vector;

namespace Engine.MapGenerator {
    public class Generator {
        private readonly HashSet<Center> _centers = new HashSet<Center>();
        private readonly HashSet<Corner> _corners = new HashSet<Corner>();
        private readonly HashSet<Edge> _edges = new HashSet<Edge>();
        private readonly HashSet<Vector> _points = new HashSet<Vector>();

        private readonly ArrayList _cornerMap = new ArrayList();
        private readonly int _size;
        private VoronoiGraph _graph; //FIXME
        private VoronoiGraph _voronoiGraph;

        public Generator(int size) {
            _size = size;
            GenerateSquareGrid(size);
            _voronoiGraph = Fortune.ComputeVoronoiGraph(_points);
            BuildGraph();
            ImproveCorners();
        }

        private void GenerateSquareGrid(int size) {
            for (var x = 0; x < size; x++) {
                for (var y = 0; y < size; y++) {
                    _points.Add(new Vector(x, y));
                }
            }
        }

        private void BuildGraph() {
            var libedges = _voronoiGraph.Edges;
            var centerLookup = new Dictionary<Vector, Center>();
            foreach (var point in _points) {
                var center = new Center {
                    index = _centers.Count,
                    location = point,
                    neighbors = new HashSet<Center>(),
                    borders = new HashSet<Edge>(),
                    corners = new HashSet<Corner>()
                };
                _centers.Add(center);
                centerLookup[point] = center;
            }
            foreach (Models.Delaunay.Edge libedge in libedges) {
                var delaunayEdge = libedge.edge // TODO: Make voronai edge castable to Delaunay
            }
        }

        private Corner MakeCorner(Vector point) {
            if (point == null) {
                return null;
            }
            int bucket;
            for (bucket = (int) point.X - 1; bucket <= (int) point.X + 1; bucket++) {
                foreach (Corner possibleCorner in (ArrayList) _cornerMap[bucket]) {
                    var dx = point.X - possibleCorner.location.X;
                    var dy = point.Y - possibleCorner.location.Y;
                    if (dx * dx + dy * dy < 1E-6) {
                        return possibleCorner;
                    }
                }
            }
            bucket = (int) point.X;
            if (_cornerMap[bucket] == null) {
                _cornerMap[bucket] = new ArrayList();
            }
            var corner = new Corner {
                index = _corners.Count,
                location = point,
                mapBorder =
                    (Math.Abs(point.X) < 1E-6 || Math.Abs(point.X - _size) < 1E-6 || Math.Abs(point.Y) < 1E-6 ||
                     Math.Abs(point.Y - _size) < 1E-6),
                touches = new HashSet<Center>(),
                protrudes = new HashSet<Edge>(),
                adjacent = new HashSet<Corner>()
            };
            _corners.Add(corner);
            _cornerMap[bucket] = corner;
            return corner;
        }

        private void AddToCornerList(Corner corner, HashSet<Corner> corners) {
            if (corner != null && !corners.Contains(corner)) {
                corners.Add(corner);
            }
        }

        private void AddToCenterList(Center center, HashSet<Center> centers) {
            if (center != null && !centers.Contains(center)) {
                centers.Add(center);
            }
        }
    }
}