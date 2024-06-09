using UnityEngine;
using System.Diagnostics;

public class TimeComplexityCalculator : MonoBehaviour
{
    void Start()
    {
        UnityEngine.Debug.Log("Time Complexity Calculator");
        // 연산 메서드 호출
        //MeasureExecutionTime(SampleMethod);
    }

    // 예제 메서드
    void SampleMethod()
    {
        int sum = 0;
        for (int i = 0; i < 1000000000; i++)
        {
            sum += i;
        }
    }

    // 실행 시간 측정 메서드
    void MeasureExecutionTime(System.Action method)
    {
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        method();
        stopwatch.Stop();

        UnityEngine.Debug.Log("Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");
    }
}
