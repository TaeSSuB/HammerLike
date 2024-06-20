using System;
using System.IO;
using UnityEngine;

public class BMPNoiseGenerator : MonoBehaviour
{
    private const int WIDTH = 1024;
    private const int HEIGHT = 1024;

    public static void GenerateNoiseBMP(string filePath = "Assets/", string fileName = "BMPNoiseResult")
    {
        if(string.IsNullOrEmpty(filePath))
        {
            filePath = "Assets/";
        }
        if(string.IsNullOrEmpty(fileName))
        {
            fileName = "BMPNoiseResult";
        }

        var fullPath = Path.Combine(filePath, fileName + ".bmp");

        Texture2D texture = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGB24, false);

        // 랜덤 노이즈 데이터 채우기
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                Color color = new Color(0, 0, UnityEngine.Random.Range(0f, 1f));
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();

        // BMP 파일로 저장
        byte[] bmpBytes = TextureToBMP(texture);
        File.WriteAllBytes(fullPath, bmpBytes);
    }

    static byte[] TextureToBMP(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        int rowBytes = width * 3;
        int paddingBytes = (4 - (rowBytes % 4)) % 4;
        int totalBytes = (rowBytes + paddingBytes) * height;
        int fileSize = 54 + totalBytes;

        byte[] bmp = new byte[fileSize];

        // BMP 헤더 작성
        using (MemoryStream ms = new MemoryStream(bmp))
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            // BMP 파일 헤더 (14 bytes)
            bw.Write((ushort)0x4D42);                // Signature 'BM'
            bw.Write(fileSize);                      // File size
            bw.Write((uint)0);                       // Reserved
            bw.Write((uint)54);                      // Data offset

            // DIB 헤더 (40 bytes)
            bw.Write((uint)40);                      // DIB header size
            bw.Write(width);                         // Width
            bw.Write(height);                        // Height
            bw.Write((ushort)1);                     // Planes
            bw.Write((ushort)24);                    // Bits per pixel
            bw.Write((uint)0);                       // Compression
            bw.Write(totalBytes);                    // Image size
            bw.Write((int)0);                        // X pixels per meter
            bw.Write((int)0);                        // Y pixels per meter
            bw.Write((uint)0);                       // Total colors
            bw.Write((uint)0);                       // Important colors

            // BMP 데이터 작성
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = texture.GetPixel(x, y);
                    bw.Write((byte)(pixel.b * 255)); // Blue
                    bw.Write((byte)(pixel.g * 255)); // Green
                    bw.Write((byte)(pixel.r * 255)); // Red
                }
                // 패딩 바이트 작성
                for (int p = 0; p < paddingBytes; p++)
                {
                    bw.Write((byte)0);
                }
            }
        }

        return bmp;
    }
}
