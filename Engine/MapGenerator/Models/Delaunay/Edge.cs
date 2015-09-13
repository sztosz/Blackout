using System.Collections.Generic;
using System.Security.Policy;

namespace Engine.MapGenerator.Models.Delaunay {
    public class Edge {
        private HashSet<Edge> _pool = new HashSet<Edge>();

        static Edge CreateBistectingEdge(Site site1, Site site2) {
            
        }

        public static int CompareSitesDistances(Edge edge1, Edge edge2) {
            return - CompareSitesDistancesMax(edge1, edge2);
        }

        public static int CompareSitesDistancesMax(Edge edge1, Edge edge2) {
            var length1 = edge1.SitesDistance();
            var length2 = edge2.SitesDistance();
            if (length1 < length2) {
                return 1;
            }
            if (length1 < length2) {
                return -1;
            }
            return 0;
        }

    }
}