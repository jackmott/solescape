using System;
using UnityEngine;

public class Graph 
{
    

    public static void graph(float[] points, Color c, Rect box)
    {
        float min = 9999;
        float max = -9999;
        

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] < min) min = points[i];
            if (points[i] > max) max = points[i];
        }

        float xMult = box.width / points.Length;
        float yMult = box.height;
        if (max-min > 1) yMult = box.height / (max-min);

        GameObject obj = new GameObject("Line");        
        obj.transform.parent = Camera.main.transform;
        obj.transform.localPosition = new Vector3(0, 0, Camera.main.transform.localPosition.x );        
        LineRenderer line = obj.AddComponent<LineRenderer>();
        line.SetColors(c, c);
        line.SetVertexCount(points.Length);
        line.useWorldSpace = true;
        line.SetWidth(.01f, .01f);
        line.material = (Material)Resources.Load("materials/GraphMat");
        
        
        for (int i = 0; i < points.Length; i++)
        {
            float x = (float)i  * xMult;
            float y = (points[i] - min) * yMult;
            x = x + (float)box.xMin;
            y = (float)y + box.yMin;
            line.SetPosition(i, Camera.main.ScreenToWorldPoint(new Vector3(x, y, 2)));
        }        

    }

  
    //****************************************************************************************************
    //  static function DrawLine(rect : Rect) : void
    //  static function DrawLine(rect : Rect, color : Color) : void
    //  static function DrawLine(rect : Rect, width : float) : void
    //  static function DrawLine(rect : Rect, color : Color, width : float) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, width : float) : void
    //  static function DrawLine(Vector2 pointA, Vector2 pointB, color : Color, width : float) : void
    //  
    //  Draws a GUI line on the screen.
    //  
    //  DrawLine makes up for the severe lack of 2D line rendering in the Unity runtime GUI system.
    //  This function works by drawing a 1x1 texture filled with a color, which is then scaled
    //   and rotated by altering the GUI matrix.  The matrix is restored afterwards.
    //****************************************************************************************************

    
}