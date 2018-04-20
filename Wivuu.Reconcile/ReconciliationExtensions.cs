using System;
using System.Collections.Generic;

namespace Wivuu.Reconcile
{
    public static class ReconciliationExtensions
    {
        /// <summary>
        /// Reconcile two independent but related sequences of items
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TDestination">The destination type</typeparam>
        /// <param name="destination">The destination sequence</param>
        /// <param name="source">The source sequence</param>
        /// <param name="isMatch">Per item do these two match</param>
        /// <returns>A reconciliation object</returns>
        public static Reconciliation<TSource, TDestination> Reconcile<TSource, TDestination>(
            this IEnumerable<TDestination> destination,
            IEnumerable<TSource> source,
            Func<TSource, TDestination, bool> isMatch) =>
            new Reconciliation<TSource, TDestination>(destination, source, isMatch);
    }
}
