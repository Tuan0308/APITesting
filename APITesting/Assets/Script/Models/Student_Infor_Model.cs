using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class Student_Infor_Model
{
    [JsonProperty("Name")]
    public string name { get; set; }

    [JsonProperty("PhoneNumber")]
    public int phoneNumber { get; set; }

    [JsonProperty("Address")]
    public string address { get; set; }

    [JsonProperty("StudentId")]
    public string studentId { get; set; }
}
