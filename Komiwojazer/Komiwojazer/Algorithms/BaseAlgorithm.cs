using System;
using System.Collections.Generic;
using System.Text;

namespace Komiwojazer.Algorithms
{
    public class BaseAlgorithm
    {
        protected readonly int N;
        public IList<IList<int>> G;
        protected IList<int> _usedPoints;
        public Version Version;
        protected readonly int STARTING_POINT;

        public BaseAlgorithm(Version version, IList<IList<int>> g)
        {
            N = g.Count;
            G = g;
            _usedPoints = new List<int>();
            Version = version;
            switch (Version)
            {
                case Version.Demo:
                    STARTING_POINT = 26;
                    break;
                case Version.Full:
                    STARTING_POINT = 132;
                    break;
            }
        }

        public bool AllPointsUsed()
        {
            for (int i = STARTING_POINT + 1; i < N; i++)
            {
                if (!_usedPoints.Contains(i))
                    return false;
            }

            return true;
        }
    }
}
