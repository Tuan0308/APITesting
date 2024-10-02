using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;
using UnityEngine.Purchasing.MiniJSON;
using System.Linq;
using Newtonsoft.Json;
public class GetDataAPI : MonoBehaviour
{
    private GameObject prefab;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private GameObject dataField;
    [SerializeField] private GetDataField getDataField;
    [SerializeField] private List<Button> buttons = new List<Button>();
    private Dictionary<string, Action> data;

    private void Awake()
    {
        //   prefab.SetActive(false);
        InitializeDataDictionary();
        prefab = Resources.Load<GameObject>("Student"); // Load prefab từ thư mục Resources
    }

    private void OnEnable()
    {
        AddListenerBtn(); // Thêm sự kiện cho các button
    }

    private void OnDisable()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners(); // Xóa sự kiện của các button
        }

    }

    private void Start()
    {
        StartCoroutine(GetDataFromAPI()); // Gọi hàm lấy dữ liệu từ API
    }

    private void Clear_Object_Infor()
    {
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject); // Xóa các object con trong parentTransform
        }
    }
    private void InitializeDataDictionary()
    {
        data = new Dictionary<string, Action>(){
            {"0", GetData},
            {"1", PostData},
            {"2", PutData},
            {"3", DeleteData},
            {"4", PatchData}
        };
    }
    private void AddListenerBtn()
    {
        foreach (var button in buttons)
        {
            int index = buttons.IndexOf(button);
            button.onClick.AddListener(() =>
            {
                if (data.TryGetValue(index.ToString(), out Action action))
                {
                    action.Invoke();
                }
            });
            // Debug.Log($"Button {index} is clicked");
        }
    }

    private void GetData()
    {
        Clear_Object_Infor();
        GlobalVariable.command = "Get"; // Gán command = "Get" để biết là phải get data
        StartCoroutine(GetDataFromAPI()); // Gọi hàm lấy dữ liệu từ API
        Debug.Log("Get Data");
    }

    private void PostData()
    {
        dataField.SetActive(true);
        GlobalVariable.command = "Post"; // Gán command = "Post" để biết là phải post data
        getDataField.CheckCommand(GlobalVariable.command);
    }

    private void PutData()
    {
        dataField.SetActive(true);
        GlobalVariable.command = "Put"; // Gán command = "Put" để biết là phải put data
        getDataField.CheckCommand(GlobalVariable.command);
    }

    private void DeleteData()
    {
        dataField.SetActive(true);
        GlobalVariable.command = "Delete"; // Gán command = "Delete" để biết là phải delete data
        getDataField.CheckCommand(GlobalVariable.command);
    }

    private void PatchData()
    {
        dataField.SetActive(true);
        GlobalVariable.command = "Patch"; // Gán command = "Patch" để biết là phải patch data
        getDataField.CheckCommand(GlobalVariable.command);
    }

    public IEnumerator GetDataFromAPI()
    {
        GlobalVariable.send_Request_Status = false;

        UnityWebRequest response = UnityWebRequest.Get(GlobalVariable.url);
        yield return response.SendWebRequest();

        if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error While Sending: " + response.error);
        }
        else
        {
            try
            {
                string jsonResponse = response.downloadHandler.text;
                if (jsonResponse != null)
                {
                    Debug.Log("Get Success");
                    List<Student_Infor_Model> dataList = JsonConvert.DeserializeObject<List<Student_Infor_Model>>(jsonResponse); // Chuyển json thành list
                    GlobalVariable.studentList = dataList; // Gán list vào biến global
                    if (GlobalVariable.command == "Get")
                    {
                        PopulateScrollView(dataList);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);
            }
        }
        response.Dispose();
    }

    public IEnumerator PostDataToAPI(Student_Infor_Model student)
    {
        GlobalVariable.send_Request_Status = false;

        string url = GlobalVariable.url;
        UnityWebRequest response = null;

        try
        {
            string jsonData = JsonConvert.SerializeObject(student);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            response = new UnityWebRequest(url, "POST");
            response.uploadHandler = new UploadHandlerRaw(bodyRaw);
            // response.downloadHandler = new DownloadHandlerBuffer();
            response.SetRequestHeader("Content-Type", "application/json");
            //  response.SendWebRequest();
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
        if (response != null)
        {
            yield return response.SendWebRequest();
            if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error While Sending: {response.error}");
            }
            if (response.result == UnityWebRequest.Result.Success)
            {
                GlobalVariable.send_Request_Status = true;
                Debug.Log("Post Success");
            }
            response.Dispose();
        }
        StartCoroutine(GetDataFromAPI());
        GlobalVariable.send_Request_Status = true;
    }

    public IEnumerator PutDataToAPI(Student_Infor_Model student)
    {
        GlobalVariable.send_Request_Status = false;

        string url = $"{GlobalVariable.url}/{student.studentId}";
        UnityWebRequest response = null;
        try
        {
            string jsonData = JsonConvert.SerializeObject(student);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            response = new UnityWebRequest(url, "PUT")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            response.SetRequestHeader("Content-Type", "application/json");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
        if (response != null)
        {
            yield return response.SendWebRequest();
            if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error While Sending: {response.error}");
            }
            if (response.result == UnityWebRequest.Result.Success)
            {
                GlobalVariable.send_Request_Status = true;
                Debug.Log("Put Success");
            }
            response.Dispose();
        }
        StartCoroutine(GetDataFromAPI());
        GlobalVariable.send_Request_Status = true;
    }

    public IEnumerator DeleteDataFromAPI(string studentId)
    {
        GlobalVariable.send_Request_Status = false;

        string endpoint = studentId;
        string url = GlobalVariable.url + "/" + endpoint;
        UnityWebRequest response = null;
        try
        {
            response = UnityWebRequest.Delete(url);
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
        if (response != null)
        {
            yield return response.SendWebRequest();
            if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error While Sending: {response.error}");
            }
            if (response.result == UnityWebRequest.Result.Success)
            {
                GlobalVariable.send_Request_Status = true;
                Debug.Log("Delete Success");
            }
            response.Dispose();
        }
        StartCoroutine(GetDataFromAPI());
        GlobalVariable.send_Request_Status = true;
    }

    public IEnumerator PatchDataToAPI(Student_Infor_Model student)
    {
        GlobalVariable.send_Request_Status = false;

        string url = $"{GlobalVariable.url}/{student.studentId}";
        UnityWebRequest response = null;
        try
        {
            string jsonData = JsonConvert.SerializeObject(student);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            response = new UnityWebRequest(url, "PUT")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            response.SetRequestHeader("Content-Type", "application/json");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
        if (response != null)
        {
            yield return response.SendWebRequest();
            if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error While Sending: {response.error}");
            }
            if (response.result == UnityWebRequest.Result.Success)
            {
                GlobalVariable.send_Request_Status = true;
                Debug.Log("Patch Success");
            }
            response.Dispose();
        }
        StartCoroutine(GetDataFromAPI());
        GlobalVariable.send_Request_Status = true;
        /*  UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://www.my-server.com/image.png");
          Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

          Sprite sprite = Sprite.Create((Texture2D)myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
          GameObject.Find("Image").GetComponent<Image>().sprite = sprite;*/
    }

    void PopulateScrollView(List<Student_Infor_Model> dataList)
    {
        foreach (var data in dataList)
        {
            GameObject newItem = Instantiate(prefab, parentTransform, false);
            newItem.GetComponent<StudentUI>().SetData(data);
        }
    }
}
