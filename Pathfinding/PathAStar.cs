using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public static class PathAStar
    {

        public static TilePath CreatePath(Tile[,] tiles, PathTileGraph<Tile> pathTileGraph, Tile startTile, Tile targetTile, bool adjacent)
        {
            Dictionary<Tile, Node<Tile>> nodes = pathTileGraph.Nodes;

            if (!nodes.ContainsKey(startTile) || !nodes.ContainsKey(targetTile))
            {
                Console.WriteLine("Node hasn't start or end tile");
                return null;
            }

            Node<Tile> startNode = nodes[startTile];

            Node<Tile> targetNode = nodes[targetTile];

            Heap<Node<Tile>> openSet = new Heap<Node<Tile>>(nodes.Count);
            HashSet<Node<Tile>> closedSet = new HashSet<Node<Tile>>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node<Tile> currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode.Equals(targetNode))
                {
                    return new TilePath(RetracePath(startNode, currentNode), targetTile, adjacent);
                }

                foreach (Tile n in currentNode.Data.AllNeighbours)
                {
                    Node<Tile> neighbourNode = nodes[n];

                    if (IsClippingCorner(currentNode.Data, n, tiles))
                        continue;

                    if (adjacent)
                    {
                        if (neighbourNode.Equals(targetNode))
                        {
                            return new TilePath(RetracePath(startNode, currentNode), targetTile, adjacent);
                        }
                    }

                    if (!n.IsWalkable)
                        continue;

                    if (closedSet.Contains(neighbourNode))
                        continue;

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbourNode);
                    if (newMovementCostToNeighbour < neighbourNode.GCost || !openSet.Contains(neighbourNode))
                    {
                        neighbourNode.GCost = newMovementCostToNeighbour;
                        neighbourNode.HCost = GetDistance(neighbourNode, targetNode);
                        neighbourNode.Parent = currentNode;

                        if (!openSet.Contains(neighbourNode))
                            openSet.Add(neighbourNode);
                        else
                            openSet.UpdateItem(neighbourNode);
                    }
                }
            }

            return null;
        }

        private static List<Tile> RetracePath(Node<Tile> startNode, Node<Tile> endNode)
        {
            List<Tile> path = new List<Tile>();
            Node<Tile> currentNode = endNode;

            while (!currentNode.Equals(startNode))
            {
                path.Add(currentNode.Data);
                currentNode = currentNode.Parent;
            }

            return path;
        }

        private static int GetDistance(Node<Tile> nodeA, Node<Tile> nodeB)
        {
            int dstX = Math.Abs(nodeA.Data.X - nodeB.Data.X);
            int dstY = Math.Abs(nodeA.Data.Y - nodeB.Data.Y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);

            return 14 * dstX + 10 * (dstY - dstX);
        }

        private static bool IsClippingCorner(Tile curr, Tile neigh, Tile[,] tiles)
        {
            int dX = curr.X - neigh.X;
            int dY = curr.Y - neigh.Y;

            if (Math.Abs(dX) + Math.Abs(dY) == 2)
            {

                if (!tiles[curr.X - dX, curr.Y].IsWalkable)
                    return true;

                if (!tiles[curr.X, curr.Y - dY].IsWalkable)
                    return true;

            }

            return false;
        }

    }
}
