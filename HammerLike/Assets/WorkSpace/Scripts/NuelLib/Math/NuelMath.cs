using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NuelLib.Mathmetics
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
        /// ���丮��
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static int Factorial(int n)
        {
            if (n == 0)
                return 1;

            int result = 1;

            for (int i = 1; i <= n; i++)
            {
                result *= i;
                Debug.Log(result);
            }

            return result;
        }

        /// <summary>
        /// ���� ���
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        static int BinomialCoefficient(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0 || k == n) return 1;

            Debug.Log(Factorial(k));
            Debug.Log(Factorial(n));
            Debug.Log(Factorial(n-k));

            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        /// <summary>
        /// ���� �÷ο� ������ ���� ������ ���� ����
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private static int CalculateBinomialCoefficient(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0 || k == n) return 1;

            int result = 1;
            for (int i = 1; i <= k; i++)
            {
                result *= (n - (k - i));
                result /= i;
            }
            return result;
        }

        /// <summary>
        /// ���� ����� �α� �� ���
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static double LogBinomialCoefficient(int n, int k)
        {
            if (k > n) return Double.NegativeInfinity;
            if (k == 0 || k == n) return 0.0;

            if (k > n - k) // C(n, k) == C(n, n - k)
                k = n - k;

            double logResult = 0.0;
            for (int i = 1; i <= k; i++)
            {
                logResult += Math.Log(n - (k - i) + 1) - Math.Log(i);
            }

            return logResult;
        }

        /// <summary>
        /// Ƚ�� n, ���� Ȯ�� p �� �� k �� ������ Ȯ�� ���
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double CalculateProbability(int n, int k, double p)
        {
            //int binomialCoefficient = CalculateBinomialCoefficient(n, k);
            //double probability = binomialCoefficient * Math.Pow(p, k) * Math.Pow(1 - p, n - k);
            //return probability;
            double logBinomialCoefficient = LogBinomialCoefficient(n, k);
            double logProbability = logBinomialCoefficient + k * Math.Log(p) + (n - k) * Math.Log(1 - p);
            return Math.Exp(logProbability);
        }

        /// <summary>
        /// �ܼ� ��� ��� (Ʈ�� ����)
        /// A �� ����, B �� ����(����)
        /// �׳� ���� ����ϴ� �Ͱ� ����
        /// </summary>
        /// <param name="branches"></param>
        /// <param name="levels"></param>
        /// <returns></returns>
        static long CalculatePaths(int branches, int levels)
        {
            return (long)Math.Pow(branches, levels);
        }

        /// <summary>
        /// ���� Ʈ�� ��� ���
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        static long CalculatePaths2(int levels)
        {
            return CalculatePaths(2, levels);
        }


    }
}