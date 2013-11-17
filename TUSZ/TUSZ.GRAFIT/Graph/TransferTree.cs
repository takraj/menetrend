using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;

namespace TUSZ.GRAFIT.Graph
{
    public class TransferTree
    {
        private Dictionary<Route, TransferTree> _tree;

        public TransferTree()
        {
            _tree = new Dictionary<Route, TransferTree>();
        }

        public TransferTree CreateOrGetTree(Route key)
        {
            if (!_tree.ContainsKey(key))
            {
                _tree[key] = new TransferTree();
            }

            return _tree[key];
        }

        public TransferTree GetTree(Route key)
        {
            return _tree[key];
        }

        public bool IsRouteAllowed(Route subject)
        {
            return _tree.ContainsKey(subject);
        }

        public bool IsLeaf
        {
            get
            {
                return _tree.Keys.Count == 0;
            }
        }
    }
}
