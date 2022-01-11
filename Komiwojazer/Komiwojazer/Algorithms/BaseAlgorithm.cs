using System;
using System.Collections.Generic;
using System.Text;

namespace Komiwojazer.Algorithms
{
    public class BaseAlgorithm
    {
        protected readonly int N;
        protected readonly IList<IList<int>> G;
        protected IList<int> _usedPoints;

        public BaseAlgorithm(IList<IList<int>> g)
        {
            N = g.Count;
            G = g;
            _usedPoints = new List<int>();
        }

        public bool AllPointsUsed()
        {
            for (int i = 27; i < N; i++)
            {
                if (!_usedPoints.Contains(i))
                    return false;
            }

            return true;
        }
    }
}
