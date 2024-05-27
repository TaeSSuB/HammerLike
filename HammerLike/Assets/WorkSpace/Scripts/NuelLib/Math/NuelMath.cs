using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NuelLib
{
    public class NuelMath
    {
        public IEnumerable<float> RandomNumbers(float a, float b, int count)
        {
            var rnd = new System.Random();
            var range = b - a;
            for (int i = 0; i < count; i++)
            {
                yield return (float)(rnd.NextDouble() * range) + a;
            }
        }

        public float RandomSlice(float a, float b, int slice = 2)
        {
            float sliceSize = (b - a) / slice;
            var sliceValues = Enumerable.Range(0, slice)
                .SelectMany(i => RandomNumbers(a + i * sliceSize, a + (i + 1) * sliceSize, 1));
            return sliceValues.Average();
        }

        /// <summary>
        /// 팩토리얼
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static int Factorial(int n)
        {
            if (n == 0)
                return 1;

            int result = 1;

            for (int i = 1; i <= n; i++)
                result *= i;

            return result;
        }

        /// <summary>
        /// 이항 계수
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        static int BinomialCoefficient(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        /// <summary>
        /// 단순 경로 계산 (트리 구조)
        /// A 는 가지, B 는 레벨(깊이)
        /// 그냥 제곱 출력하는 것과 같음
        /// </summary>
        /// <param name="branches"></param>
        /// <param name="levels"></param>
        /// <returns></returns>
        static long CalculatePaths(int branches, int levels)
        {
            return (long)Math.Pow(branches, levels);
        }

        /// <summary>
        /// 이진 트리 경로 계산
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        static long CalculatePaths2(int levels)
        {
            return CalculatePaths(2, levels);
        }


    }
}