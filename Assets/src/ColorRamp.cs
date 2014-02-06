using UnityEngine;
using System.Collections;

public class ColorRamp
{


    public static int RAMP_SIZE = 256;
    public Color[] colors = new Color[RAMP_SIZE];
    


    public static ColorRamp frozen()
    {
        Color[] colors = new Color[4];
        float[] ranges = new float[3];

        colors[0] = new Color(.0f, .6f, .6f);
        ranges[0] = .45f;
        colors[1] = new Color(0f, .9f, .9f);
        ranges[1] = .05f;
        colors[2] = new Color(.5f, .5f, .5f);
        ranges[2] = .5f;
        colors[3] = new Color(.8f, .8f, .8f);


        return new ColorRamp(colors, ranges);
    }

    public static ColorRamp hellish()
    {
        Color[] colors = new Color[5];
        float[] ranges = new float[4];

        colors[0] = new Color(.2f, 0, 0);
        ranges[0] = .45f;
        colors[1] = new Color(.8f, 0, 0);
        ranges[1] = .05f;
        colors[2] = new Color(.8f, .4f, .0f);
        ranges[2] = .05f;
        colors[3] = new Color(.1f, .1f, .1f);
        ranges[3] = .45f;
        colors[4] = new Color(.4f, .4f, .4f);

        return new ColorRamp(colors, ranges);
    }

    public static ColorRamp earthy()
    {
        Color[] colors = new Color[6];
        float[] ranges = new float[5];

        colors[0] = new Color(0, 0, .1f, 0f);
        ranges[0] = .45f;
        colors[1] = new Color(0, 0, .9f, 1f);
        ranges[1] = .05f;
        colors[2] = new Color(.9f, .8f, .4f);
        ranges[2] = .05f;
        colors[3] = new Color(0, .3f, 0);
        ranges[3] = .35f;
        colors[4] = new Color(0, .8f, 0);
        ranges[4] = .1f;
        colors[5] = new Color(.8f, .8f, .8f);

        return new ColorRamp(colors, ranges);
    }

    public ColorRamp(Color[] inColors, float[] ranges)
    {

        int colorIndex = 0;
        colors[RAMP_SIZE - 1] = Color.red;
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
                    colors[colorIndex] = new Color(r, g, b, a);
                }
                else
                {
                    float r = Mathf.Lerp(start.r, end.r, (float)j / (float)indexSpan);
                    float g = Mathf.Lerp(start.g, end.g, (float)j / (float)indexSpan);
                    float b = Mathf.Lerp(start.b, end.b, (float)j / (float)indexSpan);
                    float a = Mathf.Lerp(start.a, end.a, (float)j / (float)indexSpan);
                    colors[colorIndex] = new Color(r, g, b, a);
                }

                colorIndex++;
            }

        }

        while (colorIndex < RAMP_SIZE)
        {
            colors[colorIndex] = inColors[inColors.Length - 1];
            colorIndex++;
        }

    }




}