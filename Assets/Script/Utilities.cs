using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Utilities : MonoBehaviour
{
    public static bool isCloseEpsilonVec3(Vector3 x, Vector3 y, float epsilon = 0.5f)
    {
        if (Vector3.Distance(x, y) < epsilon)
            return true;
        else
            return false;
    }

    public static bool isCloseEpsilonf(float x, float y, float epsilon = 0.5f)
    {
        if (Mathf.Abs(x-y) < epsilon)
            return true;
        else
            return false;
    }



    public static float crossProductVector2(Vector2 v, Vector2 w)
    {
        return v.x*w.y-v.y*w.x; 
    }

    public static bool haveSameSign(float x, float y)
    {
        return ((x >= 0 && y >= 0) || (x <= 0 && y <= 0));
    }

    public static Vector2 rotate(Vector2 pointA, Vector2 pointB ,float theta, float scale=1)
    {
        
        Vector2 AB = scale *(pointB - pointA);
        //convert degree to radian
        theta *= (Mathf.PI / 180);
        Vector2 afterRotation = new Vector2(Mathf.Cos(theta)*AB.x - Mathf.Sin(theta) * AB.y, Mathf.Sin(theta) * AB.x + Mathf.Cos(theta) * AB.y);

        return afterRotation + pointA;
    }

    public static void instantiateSphereAtPosition(Vector3 position)
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.localScale /= 10;
        Instantiate(Sphere, position, Quaternion.identity);
    }




}


[System.Serializable]
public class Pair<T, U>
{
    public Pair(T first, U second)
    {
        this.first = first;
        this.second = second;
    }

    public T first { get; set; }
    public U second { get; set; }
};