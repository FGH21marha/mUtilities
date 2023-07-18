// Written by Martin Halldin (https://github.com/FGH21marha/mUtilities)

using System.IO;
using UnityEngine;

public static class MPrefs
{
    //Replace "path" to change where data is stored
    private static string DATAPATH
    {
        get
        {
            string path = Application.persistentDataPath + "/MSettings/";

            if (!Directory.Exists( path ))
                Directory.CreateDirectory( path );

            return path;
        }
    }

    public static bool HasKey( string key )
    {
        if (File.Exists( AsPath( key ) ) )
            return true;

        return false;
    }
    public static void DeleteKey( string key )
    {
        if ( HasKey( key ) )
            File.Delete( AsPath( key ) );
    }
    public static void DeleteAll()
    {
        if (Directory.Exists( DATAPATH ))
            Directory.Delete( DATAPATH , true );

        Directory.CreateDirectory( DATAPATH );
    }

    #region Utility
    private static string AsPath( string key )
    {
        return DATAPATH + key + ".txt";
    }
    private static void TryCreateFile( string key )
    {
        if ( !HasKey( key ) )
            File.Create( AsPath( key ) ).Close();
    }
    private static string Parsekey( string key , string type )
    {
        return File.ReadAllText( AsPath( key ) ).Replace( type , "" );
    }
    #endregion

    #region Variables

    #region float
    public static void SetFloat(string key, float value )
    {
        TryCreateFile( key );
        File.WriteAllText( AsPath(key) , "FLOAT:" + value.ToString() );
    }
    public static float GetFloat( string key )
    {
        if ( !HasKey( key ) )
            return 0f;

        if ( float.TryParse( Parsekey (key , "FLOAT:" ) , out float value ) )
            return value;

        return 0f;
    }
    #endregion

    #region int
    public static void SetInt( string key , int value )
    {
        TryCreateFile( key );
        File.WriteAllText( AsPath( key ) , "INT:" + value.ToString() );
    }
    public static int GetInt( string key )
    {
        if ( !HasKey( key ) )
            return 0;

        if ( int.TryParse( Parsekey( key , "INT:" ) , out int value ) )
            return value;

        return 0;
    }
    #endregion

    #region bool
    public static void SetBool( string key , bool value )
    {
        TryCreateFile( key );
        File.WriteAllText( AsPath( key ) , "BOOL:" + value.ToString() );
    }
    public static bool GetBool( string key )
    {
        if ( !HasKey( key ) )
            return false;

        if ( bool.TryParse( Parsekey( key , "BOOL:" ) , out bool value ) )
            return value;

        return false;
    }
    #endregion

    #region string
    public static void SetString( string key , string value )
    {
        TryCreateFile( key );
        File.WriteAllText( AsPath( key ) , "STRING:" + value );
    }
    public static string GetString( string key )
    {
        if ( !HasKey( key ) )
            return "";

        return Parsekey( key , "STRING:" );
    }
    #endregion

    #region Vector2
    public static void SetVector2( string key , Vector2 value )
    {
        TryCreateFile( key );

        var jSon = JsonUtility.ToJson( value );

        File.WriteAllText( AsPath( key ) , jSon );
    }
    public static Vector2 GetVector2( string key )
    {
        if (!HasKey( key ))
            return Vector2.zero;

        var data = File.ReadAllText( AsPath( key ) );

        return JsonUtility.FromJson<Vector2>( data );
    }
    #endregion

    #region Vector3
    public static void SetVector3( string key , Vector3 value )
    {
        TryCreateFile( key );

        var jSon = JsonUtility.ToJson( value );

        File.WriteAllText( AsPath( key ) , jSon );
    }
    public static Vector3 GetVector3( string key )
    {
        if (!HasKey( key ))
            return Vector3.zero;

        var data = File.ReadAllText( AsPath( key ) );

        return JsonUtility.FromJson<Vector3>( data );
    }
    #endregion

    #region Vector4
    public static void SetVector4( string key , Vector4 value )
    {
        TryCreateFile( key );

        var jSon = JsonUtility.ToJson( value );

        File.WriteAllText( AsPath( key ) , jSon );
    }
    public static Vector4 GetVector4( string key )
    {
        if (!HasKey( key ))
            return Vector4.zero;

        var data = File.ReadAllText( AsPath( key ) );

        return JsonUtility.FromJson<Vector4>( data );
    }
    #endregion

    #region Color
    public static void SetColor( string key , Color value )
    {
        TryCreateFile( key );

        var jSon = JsonUtility.ToJson( value );

        File.WriteAllText( AsPath( key ) , jSon );
    }
    public static Color GetColor( string key )
    {
        if (!HasKey( key ))
            return Color.white;

        var data = File.ReadAllText( AsPath( key ) );

        return JsonUtility.FromJson<Color>( data );
    }
    #endregion

    #region Quaternion
    public static void SetQuaternion( string key , Quaternion value )
    {
        TryCreateFile( key );

        var jSon = JsonUtility.ToJson( value );

        File.WriteAllText( AsPath( key ) , jSon );
    }
    public static Quaternion GetQuaternion( string key )
    {
        if (!HasKey( key ))
            return Quaternion.identity;

        var data = File.ReadAllText( AsPath( key ) );

        return JsonUtility.FromJson<Quaternion>( data );
    }
    #endregion

    #region Transform
    [System.Serializable] private class MPrefsTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
    public static void SetTransform( string key , Transform value )
    {
        TryCreateFile( key );

        MPrefsTransform t = new MPrefsTransform
        {
            position = value.position ,
            rotation = value.rotation ,
            scale = value.localScale
        };

        var jSon = JsonUtility.ToJson( t );

        File.WriteAllText( AsPath( key ) , jSon );
    }
    public static Transform GetTransform(this Transform transform , string key )
    {
        if (!HasKey( key ))
            return null;

        var data = File.ReadAllText( AsPath( key ) );
        var t = JsonUtility.FromJson<MPrefsTransform>( data );

        transform.position = t.position;
        transform.rotation = t.rotation;
        transform.localScale = t.scale;

        return transform;
    }
    #endregion

    #region Object
    public static void SetObject( string key , object value )
    {
        TryCreateFile( key );

        var jSon = JsonUtility.ToJson( value );

        File.WriteAllText( AsPath( key ) , jSon );
    }
    public static T GetObject<T>( string key )
    {
        if (!HasKey( key ))
            return default(T);

        var data = File.ReadAllText( AsPath( key ) );

        return JsonUtility.FromJson<T>( data );
    }
    #endregion

    #endregion
}
