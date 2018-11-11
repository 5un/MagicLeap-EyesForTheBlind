using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public class Oscillator : MonoBehaviour
{

    public double frequency = 440.0; //tone of the wave in Hertz // distance mapped to frequency
    public double min_frequency = 440.0; //tone of the wave in Hertz // distance mapped to frequency

    public double max_frequency = 1500.0; //tone of the wave in Hertz // distance mapped to frequency
    public double min_distance = 10.0; //cm
    public double max_distance = 1000.0; //cm
    public double min_tempo = 10; //sec 
    public double max_tempo = 1; //sec

    private double increment;
    private double phase; //location on the wave
    private double sampling_frequency = 480000.0; //freq at which unity works

    public float gain; //power/volume - this could be useful for our logic
    public float volume = 2f; //values greater than 1f can hurt your ears and speakersss!!!

    public float[] frequencies;
    public int thisFreq;
    public string soundType; // state: distance, currentState: fly_cont, not_fly_cont 
    public double tempo = 2; //distance 

    public float curr_time;
    public float prev_time;

    private float timeDelta;
    private int flag = 0;
    private bool isHit;

    public float distance = 20;


    // to be passed 
    void UpdateSoundByEnvironment(float distance, bool is_flying)
    {
        UpdateTempoByDistance(distance);
        UpdateFrequencyByDistance(distance);
        UpdateSoundTypeByStateAndDistance(is_flying, distance);
    }

    void Start()
    {
        Debug.Log("Start");
        //Debug.Log("dfddf", POST());
        //StartCoroutine(Post());

        frequencies = new float[8];
        frequencies[0] = 440;
        frequencies[1] = 494;
        frequencies[2] = 554;
        frequencies[3] = 587;
        frequencies[4] = 659;
        frequencies[5] = 740;
        frequencies[6] = 831;
        frequencies[7] = 880;
    }
    void Update()
    {
        UpdateSoundByEnvironment(distance, true);
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Gain on key down" + gain + thisFreq);
            //gain = volume;
            //frequency = frequencies[thisFreq];
            //thisFreq += 1;
            //thisFreq = thisFreq % frequencies.Length;
            isHit = true;
        }

        if (Input.GetKeyUp("space"))
        {
            //Debug.Log("Gain on key up" + gain + thisFreq);
            gain = 0;
            isHit = false;
        }


        if (isHit == false)
        {
            // start new timer
            flag = 1;
            timeDelta = 0;
        }

        if (isHit == true)
        {
            if (flag == 1)
            {
                // first time after press
                curr_time = System.DateTime.Now.Second;
                flag = 0;
            }

            else
            {
                // start counting delta
                timeDelta = System.DateTime.Now.Second - curr_time;
                //Debug.Log("Time delta!" + timeDelta + " tempo: " + tempo + " ishit " + isHit);

            }

        }

        while (isHit == true && timeDelta > tempo)
        {
            //Debug.Log("Should beep here!");
            Beep();
            curr_time = System.DateTime.Now.Second;
        }


    }

    void Beep()
    {

        Debug.Log("Gainnn " + gain);
        if (gain == 0)
        {
            gain = volume;
        }
        else
        {
            gain = 0;
        }

    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0 * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i += channels)
        {
            //Debug.Log("Incrementing phase");
            phase += increment;

            data[i] = GetSoundByType();

            //copy data to the second channel so that sound can be heard on both earphones etc

            if (channels == 2)
            {
                //Debug.Log("Two channels detected");
                data[i + 1] = data[i];
            }

            if (phase > (Mathf.PI * 2))
            {
                phase = 0.0; // reset after a full loop
                //Debug.Log("Phase reset");
            }
        } // increment phase as many times as number of channels
    }

    float GetSoundByType()
    {
        if (soundType == "square")
        {
            if ((float)(gain * Mathf.Sin((float)phase)) >= 0 * gain)
            {
                return (float)gain * 0.6f;
            }
            else
            {
                return (-(float)gain) * 0.6f;
            }
        }

        if (soundType == "triangle")
        {
            return (float)(gain * (double)Mathf.PingPong((float)phase, 1.0f));
        }

        return (float)(gain * Mathf.Sin((float)phase)); //default to sin
    }

    void UpdateFrequencyByDistance(float distance)
    {
        // assuming baseline as 440 Hz for 10m (1000cm) and 1500 for 10cm
        if (distance < min_distance || distance > max_distance)
        {
            frequency = 10; // inaudible
        }

        else
        {
            frequency = 440.0 + ((max_frequency - min_frequency) / (max_distance - min_distance)) * distance;
        }

    }

    void UpdateTempoByDistance(float distance)
    {
        // assuming baseline as 1sec to 5sec for 10m (1000cm) and 1500 for 10cm
        if (distance < min_distance || distance > max_distance)
        {
            tempo = 100; // stop or extremely slow?
        }

        else
        {
            tempo = 2 - ((max_tempo - min_tempo) / (max_distance - min_distance)) * distance;
        }

    }

    void UpdateSoundTypeByStateAndDistance(bool is_flying, float distance)
    {


    }

    //IEnumerator Post()
    //{
    //    //Building b = new Building { text = "<speak>Hello I am Lee</speak>", voiceId = "Joanna" };
    //    //string postData = JsonUtility.ToJson({ "text":"<speak>Hello, I'm Lee. Thanks for taking your time talking with me. I'll need to ask you a couple of questions and you don't have to answer it all. Feel free to skip any questions if you want. Just say, Skip, or press the skip button. If you want me to repeat the question, just say, repeat.</speak>","voiceId":"Joanna"});
    //    //byte[] bytes = System.Text.Encoding.UTF8.GetBytes("\"text\" : \"<speak> Hello</speak>\" , \"voiceId\":\"Joanna\"}");
    //    byte[] bytes = System.Text.Encoding.UTF8.GetBytes("{\"name\": \"paul rudd\",\"movies\": [\"I Love You Man\", \"Role Models\"]}");
    //    using (UnityWebRequest www = UnityWebRequest.Post("https://reqres.in/api/users", bytes))
    //    {
    //        www.SetRequestHeader("Content-Type", "application/json");

    //        yield return www.Send();

    //        if (www.isError)
    //        {
    //            Debug.Log(www.error);
    //        }
    //        else
    //        {
    //            Debug.Log(www.downloadHandler.text);
    //                    StringBuilder sb = new StringBuilder();
    //                    foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders())
    //                    {
    //                        sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
    //                    }

    //                    // Print Headers
    //                    Debug.Log(sb.ToString());

    //                    // Print Body
    //                    Debug.Log(www.downloadHandler.text);
    //        }
    //    }
    //}


    //IEnumerator Upload()
    //{
    //    WWWForm form = new WWWForm();
    //    form.AddField("text", "<speak>Hello I am Lee</speak>");
    //    form.AddField("voiceId", "Joanna");


    //    UnityWebRequest www = UnityWebRequest.Post("https://msufxupxpg.execute-api.us-west-1.amazonaws.com/dev/speak", form);
    //    yield return www.SendWebRequest();

    //    if (www.isNetworkError || www.isHttpError)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        Debug.Log("Form upload complete!");

    //        Debug.Log("POST successful!");
    //        StringBuilder sb = new StringBuilder();
    //        foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders())
    //        {
    //            sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
    //        }

    //        // Print Headers
    //        Debug.Log(sb.ToString());

    //        // Print Body
    //        Debug.Log(www.downloadHandler.text);
    //    }

    //}

    //public WWW POST()
    //{
    //    WWW www;
    //    Hashtable postHeader = new Hashtable();
    //    postHeader.Add("Content-Type", "application/json");

    //    // convert json string to byte
    //    var formData = System.Text.Encoding.UTF8.GetBytes("{\"text\":\" < speak > Hello, I'm Lee. Thanks for taking your time talking with me. I'll need to ask you a couple of questions and you don't have to answer it all. Feel free to skip any questions if you want. Just say, Skip, or press the skip button. If you want me to repeat the question, just say, repeat.</speak>\",\"voiceId\":\"Joanna\"}");

    //    www = new WWW("https://msufxupxpg.execute-api.us-west-1.amazonaws.com/dev/speak", formData, postHeader);
    //    StartCoroutine(WaitForRequest(www));
    //    return www;
    //}

    //IEnumerator WaitForRequest(WWW www)
    //{
    //    yield return www;

    //    // check for errors
    //    if (www.error == null)
    //    {
    //        Debug.Log("WWW Ok!: " + www.text);
    //    }
    //    else
    //    {
    //        Debug.Log("WWW Error: " + www.error);
    //    }
    //}
}