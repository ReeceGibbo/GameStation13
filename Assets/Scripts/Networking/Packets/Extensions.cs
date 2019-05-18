using System;
using LiteNetLib.Utils;
using UnityEngine;

public static class Extensions {

    /**
     *  VECTOR 3 SERIALIZATION
    **/
    public static void Put(this NetDataWriter writer, Vector3 vector) {
        writer.Put(vector.x);
        writer.Put(vector.y);
        writer.Put(vector.z);
    }

    public static Vector3 GetVector3(this NetDataReader reader) {
        Vector3 v;
        v.x = reader.GetFloat();
        v.y = reader.GetFloat();
        v.z = reader.GetFloat();
        return v;
    }

    public static void Put(this NetDataWriter writer, Color color) {
        writer.Put(color.r);
        writer.Put(color.g);
        writer.Put(color.b);
        writer.Put(color.a);
    }

    /**
     *  COLOR SERIALIZATION
    **/

    public static Color GetColor(this NetDataReader reader) {
        Color color;
        color.r = reader.GetFloat();
        color.g = reader.GetFloat();
        color.b = reader.GetFloat();
        color.a = reader.GetFloat();
        return color;
    }

    /**
     * LIGHT SERVER SERIALIZATION
    **/

    public static void Put(this NetDataWriter writer, LightServer light) {
        writer.Put(light.range);
        writer.Put(light.color);
        writer.Put(light.intensity);

        writer.Put(light.shadowStrength);
        writer.Put(light.shadowBias);
        writer.Put(light.shadowNormalBias);
        writer.Put(light.shadowNearPlane);
    }

    public static LightServer GetLight(this NetDataReader reader) {
        LightServer l;

        l.range = reader.GetFloat();
        l.color = reader.GetColor();
        l.intensity = reader.GetFloat();

        l.shadowStrength = reader.GetFloat();
        l.shadowBias = reader.GetFloat();
        l.shadowNormalBias = reader.GetFloat();
        l.shadowNearPlane = reader.GetFloat();
        return l;
    }

    /**
     * LIGHT SERVER SERIALIZATION
    **/

    public static void Put(this NetDataWriter writer, Guid guid) {
        writer.Put(guid.ToString());
    }

    public static Guid GetGuid(this NetDataReader reader) {
        Guid guid = Guid.Parse(reader.GetString());
        return guid;
    }

    public static T GetRandomElement<T>(this T[] array) {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

}
