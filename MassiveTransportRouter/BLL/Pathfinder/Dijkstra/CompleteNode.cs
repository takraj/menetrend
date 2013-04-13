using MTR.BusinessLogic.Common.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    public class CompleteNode : Node
    {
        public TimeSpan departureTime;
        public CompleteNode viaNode;
        public Stack<Edge> usedEdges;
        public Stack<int> usedRouteIds;

        public CompleteNode() { }

        public CompleteNode(CompleteNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException();
            }

            if (node.departureTime != null)
            {
                departureTime = new TimeSpan(node.departureTime.Ticks);
            }

            if (node.viaNode != null)
            {
                viaNode = new CompleteNode(node.viaNode);
            }

            if (node.usedEdges != null)
            {
                usedEdges = new Stack<Edge>();
                node.usedEdges.Reverse().ToList().ForEach(n => usedEdges.Push(n.Clone()));
            }

            if (node.usedRouteIds != null)
            {
                usedRouteIds = new Stack<int>(node.usedRouteIds.Reverse());
            }

            stopId = node.stopId;
        }
    }
}
