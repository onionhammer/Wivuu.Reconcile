using System;
using System.Collections.Generic;
using System.Linq;

namespace Wivuu.Reconcile
{
    public class Reconciliation<TSource, TDestination>
    {
        #region Delegates

        public delegate void WithMatchingDelegate(TSource source, TDestination destination);

        public delegate void WithNotInSourceDelegate(TDestination destination);

        public delegate void WithNotInDestinationDelegate(TSource source);

        #endregion

        #region Fields

        readonly IEnumerable<TDestination> Destination;
        readonly IEnumerable<TSource> Source;
        readonly Func<TSource, TDestination, bool> IsMatch;

        private WithMatchingDelegate WithMatchingCallback                     = null;
        private WithNotInSourceDelegate WithItemNotInSourceCallback           = null;
        private WithNotInDestinationDelegate WithItemNotInDestinationCallback = null;

        #endregion

        #region Constructor

        internal Reconciliation(
            IEnumerable<TDestination> destination,
            IEnumerable<TSource> source,
            Func<TSource, TDestination, bool> isMatch)
        {
            this.Destination = destination;
            this.Source      = source;
            this.IsMatch     = isMatch;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Item exists in both source and destination sequences.
        /// Update destination item
        /// </summary>
        public Reconciliation<TSource, TDestination> WithMatching(WithMatchingDelegate callback)
        {
            this.WithMatchingCallback = callback;
            return this;
        }

        /// <summary>
        /// Item only exists in destination sequence.
        /// Delete destination item
        /// </summary>
        public Reconciliation<TSource, TDestination> WithItemNotInSource(WithNotInSourceDelegate callback)
        {
            this.WithItemNotInSourceCallback = callback;
            return this;
        }

        /// <summary>
        /// Item only exists in source sequence.
        /// Insert source item
        /// </summary>
        public Reconciliation<TSource, TDestination> WithItemNotInDestination(WithNotInDestinationDelegate callback)
        {
            this.WithItemNotInDestinationCallback = callback;
            return this;
        }

        /// <summary>
        /// Perform updates to destination
        /// </summary>
        public void UpdateDestination()
        {
            var destination = Destination.ToList();

            foreach (var src in Source)
            {
                var index = destination.FindIndex(dest => IsMatch(src, dest));

                if (index >= 0)
                {
                    // Item exists in both sequences
                    WithMatchingCallback?.Invoke(src, destination[index]);

                    destination.RemoveAt(index);
                }
                else
                    // Item exists in source but not destination
                    WithItemNotInDestinationCallback?.Invoke(src);
            }

            if (WithItemNotInSourceCallback != null)
                foreach (var dest in destination)
                    // Item exists in destination but not in source
                    WithItemNotInSourceCallback(dest);
        }

        /// <summary>
        /// Return zipped results
        /// </summary>
        public IEnumerable<(TSource source, TDestination destination)> Zip()
        {
            var destination = Destination.ToList();

            foreach (var src in Source)
            {
                var index = destination.FindIndex(dest => IsMatch(src, dest));

                if (index >= 0)
                {
                    // Item exists in both sequences
                    yield return (src, destination[index]);

                    destination.RemoveAt(index);
                }
                else
                    yield return (src, default);
            }

            foreach (var dest in destination)
                yield return (default, dest);
        }

        #endregion
    }
}