using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    public class BinaryHeapPriorityQueue : PriorityQueue
    {
        private List<CompleteNode> _storage = new List<CompleteNode>();
        private Dictionary<int, int> _idMap = new Dictionary<int, int>();       // StopId --> Index

        private int GetLeftChildIndex(int i)
        {
            return 2 * i + 1;
        }

        private int GetRightChildIndex(int i)
        {
            return 2 * i + 2;
        }

        private int? GetParentIndex(int i)
        {
            if (i > 0)
            {
                return (int)Math.Floor((i - 1) / 2.0);
            }
            else
            {
                return null;
            }
        }

        private void BubbleUp(int i)
        {
            var node = _storage[i];
            bool valid = false;
            while (!valid)
            {
                var parentIndex = GetParentIndex(_idMap[node.stopId]);

                if (parentIndex == null)
                {
                    valid = true;
                }
                else if (_storage[(int)parentIndex].departureTime > node.departureTime)
                {
                    Swap(_idMap[node.stopId], (int)parentIndex);
                }
                else {
                    valid = true;
                }
            }
        }

        private void PushDown(int i)
        {
            var node = _storage[i];
            bool valid = false;
            while (!valid)
            {
                int rightChildIndex = GetRightChildIndex(_idMap[node.stopId]);
                int leftChildIndex = GetLeftChildIndex(_idMap[node.stopId]);

                bool rightChildExists = (_storage.Count > rightChildIndex);
                bool leftChildExists = (_storage.Count > leftChildIndex);

                if (!(rightChildExists || leftChildExists))
                {
                    valid = true;
                }
                else
                {
                    int directionIndex;

                    if (rightChildExists && leftChildExists)
                    {
                        if (_storage[rightChildIndex].departureTime < _storage[leftChildIndex].departureTime)
                        {
                            directionIndex = rightChildIndex;
                        }
                        else
                        {
                            directionIndex = leftChildIndex;
                        }
                    }
                    else if (rightChildExists)
                    {
                        directionIndex = rightChildIndex;
                    }
                    else
                    {
                        directionIndex = leftChildIndex;
                    }

                    if (_storage[directionIndex].departureTime < node.departureTime)
                    {
                        Swap(_idMap[node.stopId], directionIndex);
                    }
                    else
                    {
                        valid = true;
                    }
                }
            }
        }

        private void Swap(int i1, int i2)
        {
            var temp = _storage[i1];
            _storage[i1] = _storage[i2];
            _storage[i2] = temp;

            _idMap[_storage[i1].stopId] = i1;
            _idMap[_storage[i2].stopId] = i2;
        }

        public CompleteNode Pop()
        {
            if (_storage.Count == 0)
            {
                return null;
            }

            Swap(0, _storage.Count - 1);

            var result = _storage.Last();
            _storage.Remove(result);
            _idMap.Remove(result.stopId);

            if (_storage.Count > 0)
            {
                PushDown(0);
            }

            return result;
        }

        public CompleteNode Peek()
        {
            if (_storage.Count == 0)
            {
                return null;
            }

            return new CompleteNode(_storage.First());
        }

        public bool Contains(int stopId)
        {
            return _idMap.ContainsKey(stopId);
        }

        public CompleteNode GetNode(int stopId)
        {
            if (!_idMap.ContainsKey(stopId))
            {
                return null;
            }

            return new CompleteNode(_storage[_idMap[stopId]]);
        }

        public void Push(CompleteNode node)
        {
            if (_idMap.ContainsKey(node.stopId))
            {
                UpdateNode(node);
                return;
            }

            var insertedIndex = _storage.Count;
            _storage.Add(node);
            _idMap.Add(node.stopId, insertedIndex);
            BubbleUp(insertedIndex);
        }

        public void UpdateNode(CompleteNode node)
        {
            if (!_idMap.ContainsKey(node.stopId))
            {
                Push(node);
                return;
            }

            int i = _idMap[node.stopId];
            var originalNode = _storage[i];
            _storage[i] = new CompleteNode(node);

            if (originalNode.departureTime > node.departureTime)
            {
                BubbleUp(i);
            }
            else
            {
                PushDown(i);
            }
        }
    }
}
