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

    #endregion
}