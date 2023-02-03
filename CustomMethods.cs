using Color = UnityEngine.Color;

namespace CrabGameUtils;

// ReSharper disable UnusedMember.Global
public static class CustomMethods
{
    public static uint RandomColor() => (uint)new Random().Next(0x0, 0xFFFFFF);
    
    public static float FixValue(float value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
    
    public static Vector3 FillRandom(this Vector3 vector, int min, int max)
    {
        vector.x = UnityEngine.Random.Range(min, max);
        vector.y = UnityEngine.Random.Range(min, max);
        vector.z = UnityEngine.Random.Range(min, max);
        return vector;
    }
    
    public static Color ToColor(this Vector3 vector)
        => new(vector.x, vector.y, vector.z);

    public static void RandomColor(ref this Color color)
    {
        color = Color.HSVToRGB(UnityEngine.Random.Range(0, 255), 255, 255);
    }
    
    public static Texture MakeGrayscale (Texture2D tex , int skip) 
    {
        var texColors = tex.GetPixels();
        for (int i = 0; i < texColors.Length; i++) 
        {
            if (i % skip == 0)
            {
                var grayValue = texColors[i].grayscale;
                texColors[i] = new Color(grayValue, grayValue, grayValue, texColors[i].a);
            }
            
        }
        tex.SetPixels(texColors);
        tex.Apply();
        return tex;
    }
}