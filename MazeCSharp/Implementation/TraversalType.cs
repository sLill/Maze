﻿using Common;

namespace Implementation
{
    public abstract class TraversalType
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public Map Map { get; private set; }
        #endregion Properties..

        #region Constructors..
        public TraversalType(Map map)
        {
            Map = map;
        }
        #endregion Constructors..

        #region Methods..
        public bool Solve(bool multithreadingEnabled)
        {
            return multithreadingEnabled ? SolveMultiThreaded() : SolveSingleThreaded();
        }

        protected virtual bool SolveSingleThreaded() { return true; }

        protected virtual bool SolveMultiThreaded() { return true; }
        #endregion Methods..
    }
}
