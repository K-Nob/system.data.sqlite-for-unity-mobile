using UnityEngine;
using System.Data.SQLite;


public class Sample : MonoBehaviour
{

    //
    // Constants
    //

    public static readonly string SampleDatabaseName    = "sample.sqlite3";
    public static readonly string SampleTableName       = "member";




    //
    // Static Methods
    //



    static bool ConfirmSampleTableExists (SQLiteCommand cmd)
    {
        Debug.Log("[ConfirmSampleTableExists]");

        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";

        using (SQLiteDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                if (reader.GetString(0) == SampleTableName)
                {
                    return true;
                }
            }
        }

        return false;
    }



    static void CreateSampleTable (SQLiteCommand cmd)
    {
        Debug.Log("[CreateSampleTable]");

        cmd.CommandText = $"CREATE TABLE {SampleTableName}(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT)";

        cmd.ExecuteNonQuery();
    }


    static void InsertRecord (SQLiteCommand cmd)
    {
        Debug.Log("[InsertRecord]");

        cmd.CommandText = $"INSERT INTO {SampleTableName}(name) values (@name)";

        cmd.Parameters.Add(new SQLiteParameter("@name", "Unknown"));

        cmd.ExecuteNonQuery();
    }


    static int RetrieveLatestRecordId (SQLiteCommand cmd)
    {
        Debug.Log("[RetrieveLatestRecordId]");

        cmd.CommandText = $"SELECT MAX(id) FROM {SampleTableName}";

        using (SQLiteDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                return reader.GetInt32(0);
            }
        }

        return 0;
    }




    //
    // Instance Fields
    //

    int _latestRecordId;




    //
    // Instance Methods
    //

    void Start()
    {
        string databaseFilePath = System.IO.Path.Combine (Application.persistentDataPath, SampleDatabaseName);

        if (! System.IO.File.Exists (databaseFilePath))
        {
            Debug.Log("SQLiteConnection.CreateFile");
            SQLiteConnection.CreateFile (databaseFilePath);
        }

        using (var conn = new SQLiteConnection ($"Data Source={databaseFilePath}"))
        {
            Debug.Log("SQLiteConnection.Open");
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                if (! ConfirmSampleTableExists (cmd))
                {
                    CreateSampleTable (cmd);
                }

                InsertRecord (cmd);
                _latestRecordId = RetrieveLatestRecordId (cmd);
            }
        }
    }


    void OnGUI()
    {
        GUILayout.Space (100F);

        GUIStyle style = new GUIStyle ();
        style.normal.textColor = Color.white;
        style.fontSize = 27;

        GUILayout.Label (new GUIContent ($"The latest record id = {_latestRecordId}"), style);
    }

}
