using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Clarifai.API;
using Clarifai.API.Responses;
using Clarifai.DTOs.Inputs;
using UnityEditor;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
public class DistanceDescriptor : MonoBehaviour
{

    ClarifaiClient client;
    List<string> res_objects = new List<string>();


    // Use this for initialization
    void Start()
    {
        //TO BE USED
        // getAudioSourceByMp3(GetFilenameByDistance(5.6));

        //Debug.Log("Audio: " + src);
        //  src.Play();

        //getAudioSourceByMp3("beyond5");
        Debug.Log("FILENAME :  " + GetFilenameByDistance(2.1));
        Debug.Log("Trying API");
        client = new ClarifaiClient("1723ab89f1ba4d898bc597d720f34bdb");
        /*
        var res = await client.PublicModels.GeneralModel
                .Predict(new ClarifaiURLImage("https://samples.clarifai.com/metro-north.jpg"))
                .ExecuteAsync();
        */
        /*
        Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/A11Y/metro-north.jpg", typeof(Texture2D));
        var arr = t.EncodeToJPG(100);
        var res = await client.PublicModels.GeneralModel
            .Predict(new ClarifaiFileImage(arr))
            .ExecuteAsync();
        Debug.Log("type: " + res);

        Debug.Log("Body: " + res.RawBody);


        int ix = res.RawBody.IndexOf("\"data\":{\"concepts\":");

        if (ix != -1)
        {
            string data = res.RawBody.Substring(ix);
            // do something here
            Debug.Log("STRING " + data + " STRING");

            string first_name = data.Substring(data.IndexOf("name\":\"") + 7, get_length(data));
            data = data.Substring(data.IndexOf(first_name) + 20);
            string second_name = data.Substring(data.IndexOf("name\":\"") + 7, get_length(data));
            data = data.Substring(data.IndexOf(second_name) + 20);
            string third_name = data.Substring(data.IndexOf("name\":\"") + 7, get_length(data));
            Debug.Log("name " + first_name);
            Debug.Log("name " + second_name);
            Debug.Log("name " + third_name);

            res_objects.Add(first_name);
            res_objects.Add(second_name);
            res_objects.Add(third_name);
            playSound(first_name, second_name, third_name);
        }

        */


    }

    int get_length(string data)
    {
        return data.IndexOf("\",\"value\"") - data.IndexOf("name\":\"") - 7;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    public string GetFilenameByDistance(double distance)
    {
        double rounded_distance = Math.Round(distance, 1);

        if (rounded_distance > 5)
            return "beyond5";

        else if (rounded_distance > 4.5)
        {
            if (Math.Abs(rounded_distance - 5) < Math.Abs(rounded_distance - 4.5))
                return "5-0";
            else
                return "4-5";
        }

        else if (rounded_distance > 4.0)
        {
            //            Debug.Log("Distnace " + (Math.Abs(rounded_distance - 4.5) < Math.Abs(rounded_distance - 4.0)));
            if (Math.Abs(rounded_distance - 4.5) < Math.Abs(rounded_distance - 4.0))
                return "4-5";
            else
                return "4-0";
        }

        else if (rounded_distance > 3.5)
        {
            if (Math.Abs(rounded_distance - 4) < Math.Abs(rounded_distance - 3.5))
                return "4-0";
            else
                return "3-5";
        }

        else if (rounded_distance > 3.0)
        {
            if (Math.Abs(rounded_distance - 3.5) < Math.Abs(rounded_distance - 3))
                return "3-5";
            else
                return "3-0";
        }

        else if (rounded_distance > 2.5)
        {
            if (Math.Abs(rounded_distance - 3) < Math.Abs(rounded_distance - 2.5))
                return "3-0";
            else
                return "2-5";
        }

        else if (rounded_distance > 2)
        {
            if (Math.Abs(rounded_distance - 2.5) < Math.Abs(rounded_distance - 2))
                return "2-5";
            else
                return "2-0";
        }

        else if (rounded_distance > 1.5)
        {
            if (Math.Abs(rounded_distance - 2) < Math.Abs(rounded_distance - 1.5))
                return "2-0";
            else
                return "1-5";
        }

        else if (rounded_distance > 1)
        {
            if (Math.Abs(rounded_distance - 1.5) < Math.Abs(rounded_distance - 1))
                return "1-5";
            else
                return "1-0";
        }


        return "1-0";

    }

    public AudioSource getAudioSourceByMp3(string mp3)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = Resources.Load<AudioClip>(mp3);
        audio.Play();
        Debug.Log("MP3 " + mp3);
        return audio;
    }

    public Dictionary<double, AudioSource> getDistanceAudioMap(List<double> distances)
    {
        Dictionary<double, AudioSource> data = new Dictionary<double, AudioSource>();

        foreach (double distance in distances)
        {
            data.Add(distance, getAudioSourceByMp3(GetFilenameByDistance(distance)));
        }
        return data;
    }

    int playSound(string n1, string n2, string n3)
    {
        n1 = n1.ToLower();
        n2 = n2.ToLower();
        n3 = n3.ToLower();

        if (n1.Equals("furniture") || n2.Equals("furniture") || n3.Equals("furniture"))
        {
            getAudioSourceByMp3("furniture");

        }
        else if (n1.Equals("people") || n2.Equals("people") || n3.Equals("people"))
        {
            getAudioSourceByMp3("people");
        }
        else if (n1.Equals("chair") || n2.Equals("chair") || n3.Equals("chair"))
        {
            //play chair.mp3
        }
        else if (n1.Equals("table") || n2.Equals("table") || n3.Equals("table"))
        {
            //play table.mp3
        }
        else if (n1.Equals("wall") || n2.Equals("wall") || n3.Equals("wall"))
        {
            //play wall.mp3
        }
        else if (n1.Equals("room") || n2.Equals("room") || n3.Equals("room"))
        {
            //play room.mp3
        }

        return 0;
    }

}