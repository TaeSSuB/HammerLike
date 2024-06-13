using System.Collections.Generic;
using UnityEngine;

public class RandomRangeDistribution : MonoBehaviour
{
    private const int SAMPLE_SIZE = 1000000;
    private const int BUCKETS = 256;
    private const int TIME_SLICES = 10;
    private const int RANDOM_SEED = 12345;

    //void Start()
    //{
    //    MeasureRandomRange();
    //}

    public static void MeasureRandomRange(string filePath = "Assets/", string fileName = "RandomRangeDistribution")
    {
        int[] distribution = new int[BUCKETS];

        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            int seed = HashFunction(i);
            Random.InitState(seed);
            int randomValue = Mathf.FloorToInt(Random.Range(0, BUCKETS));
            distribution[randomValue]++;
        }

        // ������׷� �ð�ȭ
        VisualizeDistribution(distribution, filePath, fileName);
    }
    static int HashFunction(int value)
    {
        // ������ �ؽ� �Լ� ����
        return value ^ 0x5f3759df; // ������ XOR ����
    }
    static void VisualizeDistribution(int[] distribution, string filePath = "Assets/", string fileName = "RandomRangeDistribution")
    {
        int maxValue = Mathf.Max(distribution);
        Texture2D texture = new Texture2D(BUCKETS, BUCKETS);
        for (int x = 0; x < BUCKETS; x++)
        {
            for (int y = 0; y < BUCKETS; y++)
            {
                texture.SetPixel(x, y, Color.white);
            }

            int value = distribution[x];
            int height = (int)((float)value / maxValue * BUCKETS);
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();

        byte[] pngBytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(filePath + fileName + ".png", pngBytes);

    }
}
