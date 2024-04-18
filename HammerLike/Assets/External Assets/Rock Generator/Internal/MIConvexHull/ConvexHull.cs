/******************************************************************************
 *
 * The MIT License (MIT)
 *
 * MIRockGenConvexHull, Copyright (c) 2015 David Sehnal, Matthew Campbell
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *  
 *****************************************************************************/

namespace MIRockGenConvexHull
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Factory class for computing convex hulls.
    /// </summary>
    public static class RockGenConvexHull
    {
        /// <summary>
        /// Creates a convex hull of the input data.
        /// </summary>
        /// <typeparam name="TVertex"></typeparam>
        /// <typeparam name="TFace"></typeparam>
        /// <param name="data"></param>
        /// <param name="config">If null, default RockGenConvexHullComputationConfig is used.</param>
        /// <returns></returns>
        public static RockGenConvexHull<TVertex, TFace> Create<TVertex, TFace>(IList<TVertex> data, RockGenConvexHullComputationConfig config = null)
            where TVertex : IVertex
            where TFace : ConvexFace<TVertex, TFace>, new()
        {
            return RockGenConvexHull<TVertex, TFace>.Create(data, config);
        }

        /// <summary>
        /// Creates a convex hull of the input data.
        /// </summary>
        /// <typeparam name="TVertex"></typeparam>
        /// <param name="data"></param>
        /// <param name="config">If null, default RockGenConvexHullComputationConfig is used.</param>
        /// <returns></returns>
        public static RockGenConvexHull<TVertex, DefaultConvexFace<TVertex>> Create<TVertex>(IList<TVertex> data, RockGenConvexHullComputationConfig config = null)
            where TVertex : IVertex
        {
            return RockGenConvexHull<TVertex, DefaultConvexFace<TVertex>>.Create(data, config);
        }

        /// <summary>
        /// Creates a convex hull of the input data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="config">If null, default RockGenConvexHullComputationConfig is used.</param>
        /// <returns></returns>
        public static RockGenConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> Create(IList<double[]> data, RockGenConvexHullComputationConfig config = null)
        {
            var points = data.Select(p => new DefaultVertex { Position = p.ToArray() }).ToList();
            return RockGenConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>>.Create(points, config);
        }
    }

    /// <summary>
    /// Representation of a convex hull.
    /// </summary>
    /// <typeparam name="TVertex"></typeparam>
    /// <typeparam name="TFace"></typeparam>
    public class RockGenConvexHull<TVertex, TFace>
        where TVertex : IVertex
        where TFace : ConvexFace<TVertex, TFace>, new()
    {
        /// <summary>
        /// Points of the convex hull.
        /// </summary>
        public IEnumerable<TVertex> Points { get; internal set; }

        /// <summary>
        /// Faces of the convex hull.
        /// </summary>
        public IEnumerable<TFace> Faces { get; internal set; }

        /// <summary>
        /// Creates the convex hull.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="config">If null, default RockGenConvexHullComputationConfig is used.</param>
        /// <returns></returns>
        public static RockGenConvexHull<TVertex, TFace> Create(IList<TVertex> data, RockGenConvexHullComputationConfig config)
        {
            if (data == null) throw new ArgumentNullException("data");
            return RockGenConvexHullInternal.GetRockGenConvexHull<TVertex, TFace>((IList<TVertex>)data, config);
        }

        /// <summary>
        /// Can only be created using a factory method.
        /// </summary>
        internal RockGenConvexHull()
        {

        }
    }
}
