using UnityEngine;
using System.Collections;

public class ColorRamp
{


    public static int RAMP_SIZE = 256;
    public Color[] gradient = new Color[RAMP_SIZE];
    private Color[] colors;


    
	public ColorRamp(Color[] inColors, float[] ranges)
    {
        this.colors = inColors;
        int colorIndex = 0;
        gradient[RAMP_SIZE - 1] = Color.red;
        for (int i = 0; i < inColors.Length - 1; i++)
        {
            Color start = inColors[i];  //start of gradient
            Color end = inColors[i + 1]; //end of gradient
            int indexSpan = (int)(RAMP_SIZE * ranges[i]); // number of indices in colors to fill with this interpolation;

            for (int j = 0; j < indexSpan; j++)
            {
                if (i == 0)
                {
                    float r = start.r;
                    float g = start.g;
                    float b = start.b;
                    float a = start.a;
                    gradient[colorIndex] = new Color(r, g, b, a);
                }
                else
                {
                    float r = Mathf.Lerp(start.r, end.r, (float)j / (float)indexSpan);
                    float g = Mathf.Lerp(start.g, end.g, (float)j / (float)indexSpan);
                    float b = Mathf.Lerp(start.b, end.b, (float)j / (float)indexSpan);
                    float a = Mathf.Lerp(start.a, end.a, (float)j / (float)indexSpan);
                    gradient[colorIndex] = new Color(r, g, b, a);
                }

                colorIndex++;
            }

        }

        while (colorIndex < RAMP_SIZE)
        {
            gradient[colorIndex] = inColors[inColors.Length - 1];
            colorIndex++;
        }

    }

    public override string ToString()
    {
        string result = "";
        for (int i = 0; i < colors.Length; i++)
        {
            result += colors[i];
            if (i < colors.Length - 1)
            {
                result += "|";
            }
        }
		return result;
    }


}