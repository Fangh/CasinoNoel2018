using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class WeatherAPI : MonoBehaviour
{
    public List<WeatherObject> myList = new List<WeatherObject>();

    // Use this for initialization
    void Start ()
    {
        AddWeatherInListAsync();		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ContextMenu("Add")]
    public async void AddWeatherInListAsync()
    {
        WeatherObject w = await GetWeather();
        myList.Add(w);
    }

    [Serializable]
    public class WeatherObject
    {
        public int id;
        public string name;
        public List<Weather> weather;
    }


    [Serializable]
    public class Weather
    {
        public int id;
        public string main;
        public string description;
        public string icon;

    }

    public async Task<WeatherObject> GetWeather()
    {
        WebRequest request = WebRequest.Create(string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}", "Nantes,FR", "b865e644ced70de1ed3b9cb52f3a1f8a"));
        WebResponse response = await request.GetResponseAsync();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        WeatherObject info = JsonUtility.FromJson<WeatherObject>(jsonResponse);
        return info;
    }
}
