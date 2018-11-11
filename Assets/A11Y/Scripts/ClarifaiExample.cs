using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Clarifai.API;
using Clarifai.API.Responses;
using Clarifai.DTOs.Inputs;
using UnityEditor;
using System.Threading.Tasks;

public class ClarifaiExample : MonoBehaviour {

    ClarifaiClient client;

	// Use this for initialization
	async void Start () {
        Debug.Log("Trying API");
        client = new ClarifaiClient("1f0b34a90530d579b006a1e3071b4846a508d362");
        /*
        var res = await client.PublicModels.GeneralModel
                .Predict(new ClarifaiURLImage("https://samples.clarifai.com/metro-north.jpg"))
                .ExecuteAsync();
        */
        /*
        Texture2D t = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/A11Y/Textures/metro-north.jpg", typeof(Texture2D));
        var arr = t.EncodeToJPG(100);
        var res = await client.PublicModels.GeneralModel
            .Predict(new ClarifaiFileImage(arr))
            .ExecuteAsync();
        
        Debug.Log(res);
        */

        // res.


    }
	
	// Update is called once per frame
	void Update () {
	   	
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


}
