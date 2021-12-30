using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using FullSerializer;

public class DatabaseManager : MonoBehaviour
{
    public string PlayerName;
    public static DatabaseManager Instance;
    public AudioClip AC;
    public string s;
    public List<Signal> Logs;
    public List<Signal> EarthLogs;

    public string Display = "";

    public TMPro.TMP_InputField InputTarget;


    private static fsSerializer serializer = new fsSerializer();

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this);
        }
        /*print(AC.samples * AC.channels);
        float[] samples = new float[AC.samples * AC.channels];
        AC.GetData(samples, 0);
        byte[] byteData = new byte[samples.Length * 4];
        Buffer.BlockCopy(samples, 0, byteData, 0, byteData.Length);
        byte[] CompressedData = Compress(byteData);
        Debug.Log("Compressed byte size " + CompressedData.Length);
        s = Convert.ToBase64String(CompressedData);
        string[] Splitted = StringZipper(s, 36);
        Debug.Log(Splitted.Length);
        //PostToDatabase(Splitted);
        GetFromDatabase();*/
        //Submit(2, new Signal("Marshall Fulcrum", "Bringing Taco Bell to the ship was a bad idea... there's no bathroom."));
        //Submit(0, new Signal("Cetacean", "This is a complete failure."));
        //Submit(1, new Signal("Time Ranger", "It's a long journey. I don't have any resentment now. I accept my destiny."));
        //Submit(3, new Signal("Kayi", "Mom, Dad, Goodbye, I love you:)"));
        RetrieveLogs();

    }

    public void AddPlayerLog(int i)
    {
        Display += "\n<color=#FF5342>" + Logs[i].PlayerName + ": " + Logs[i].Message + "</color>";
    }
    

    public void AddEarthLog(int i)
    {
        Display += "\n<color=#6CFF42>" + EarthLogs[i].PlayerName + ": " + EarthLogs[i].Message + "</color>";
    }

    public void Submit(int index, string s)
    {
        RestClient.Put("https://bvwround4-default-rtdb.firebaseio.com/logs/" + index + ".json", new Signal(PlayerName, s));
    }

    public void Submit(int index, Signal s)
    {
        RestClient.Put("https://bvwround4-default-rtdb.firebaseio.com/logs/" + index + ".json", s);
    }

    public delegate void GetLogsCallback(List<Signal> response);

    public void RetrieveLogs()
    {
        RetrieveAllLogs(response =>
        {
            Logs = response;
            //Debug.Log(LeaderBoards.Count);
            //Debug.Log(LeaderBoards[0].players.Count);
        });
    }

    private void RetrieveAllLogs(GetLogsCallback callback)
    {
        RestClient.Get("https://bvwround4-default-rtdb.firebaseio.com/logs.json").Then(response =>
        {
            //Debug.Log(response.Text);
            var responseJson = response.Text;
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(List<Signal>), ref deserialized);
            var logs = deserialized as List<Signal>;
            Debug.Log(logs.Count);
            callback(logs);
        });
    }

    /*public static byte[] Compress(byte[] data)
    {
        MemoryStream output = new MemoryStream();
        using (DeflateStream dstream = new DeflateStream(output, System.IO.Compression.CompressionLevel.Optimal))
        {
            dstream.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    public static byte[] Decompress(byte[] data)
    {
        MemoryStream input = new MemoryStream(data);
        MemoryStream output = new MemoryStream();
        using (DeflateStream dstream = new DeflateStream(input, System.IO.Compression.CompressionMode.Decompress))
        {
            dstream.CopyTo(output);
        }
        return output.ToArray();
    }


    public byte[] CompressBytes(byte[] data)
    {
        using (MemoryStream compressStream = new MemoryStream())
        {
            using (var zipStream = new GZipStream(compressStream, CompressionMode.Compress))
                zipStream.Write(data, 0, data.Length);
            return compressStream.ToArray();
        }
    }

    public byte[] DecompressBytes(byte[] data)
    {
        using (var compressStream = new MemoryStream(data))
        {
            using (GZipStream zipStream = new GZipStream(compressStream, CompressionMode.Decompress))
            {
                byte[] nb = { data[data.Length - 4], data[data.Length - 3], data[data.Length - 2], data[data.Length - 1] };
                int count = BitConverter.ToInt32(nb, 0);

                byte[] resultStream = new byte[count];
                zipStream.Read(resultStream, 0, count);

                return resultStream;
            }
        }
    }

    private string[] StringZipper(string s, int limit = 36)
    {
        string[] result = new string[s.Length / limit];
        for (int i = 0; i < s.Length / 36; i++)
        {
            result[i] = string.Copy(s.Substring(i * limit, limit));
        }
        return result;
    }

    private string StringUnipper(string[] s, int limit = 36)
    {
        string result = "";
        for (int i = 0; i < s.Length; i++)
        {
            result += s[i];
        }
        return result;
    }

    public void PostToDatabase(string[] SoundFile)
    {
        RestClient.Post("https://bvwround4-default-rtdb.firebaseio.com/recordings/" + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + ".json", JsonHelper.ArrayToJsonString(SoundFile));
    }

    public void GetFromDatabase()
    {
        RestClient.GetArray<string>("https://bvwround4-default-rtdb.firebaseio.com/recordings/5141/-MmqiaRT5w32Ct91kVLD/Items.json").Then(response =>
        {
            Debug.Log(response.Length);
            string unzipped = StringUnipper(response);
            byte[] BacktoBytes = Convert.FromBase64String(unzipped);
            Debug.Log("before decompressed to be " + BacktoBytes.Length);
            byte[] decompressed = Decompress(BacktoBytes);
            Debug.Log("decompressed to be " + decompressed.Length);
            var floatArray = new float[decompressed.Length / 4];
            Debug.Log("float size " + floatArray.Length);
            Buffer.BlockCopy(decompressed, 0, floatArray, 0, decompressed.Length);
            AudioClip NAC = AudioClip.Create("Haha", floatArray.Length, 1, 44100, false);
            NAC.SetData(floatArray, 0);
            GetComponent<AudioSource>().clip = NAC;
        });
    }*/
}
