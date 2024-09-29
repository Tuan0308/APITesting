using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class GetDataField : MonoBehaviour
{
    [SerializeField] private GameObject dataField;
    [SerializeField] private GameObject content;
    [SerializeField] private GetDataAPI getDataAPI;
    [SerializeField] private List<TMP_InputField> inputFields = new List<TMP_InputField>();
    private int count = 0;

    [Header("Button")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button backButton;
    private Student_Infor_Model student_Infor_Model = new Student_Infor_Model();

    private void OnEnable()
    {
        AddListenerBtn();
    }

    private void OnDisable()
    {
        selectButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
    }

    private void AddListenerBtn()
    {
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(() => SendRequest(student_Infor_Model));
        }
        else
        {

        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(() => BackToMenu());
        }
        else
        {

        }
    }

    private void SendRequest(Student_Infor_Model student_Infor_Model) // Gửi request
    {
        foreach (var inputField in inputFields)
        {
            if (inputField != null && inputField.transform.parent.gameObject.activeSelf)
            {
                string value = inputField.text;
                var propertyName = inputField.name.Split('_')[0];
                var property = student_Infor_Model.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    if (int.TryParse(value, out int valueConverted) && !propertyName.Contains("Id"))
                    {
                        property.SetValue(student_Infor_Model, valueConverted);
                    }
                    else
                    {
                        property.SetValue(student_Infor_Model, value);
                    }
                }
            }
            DoCommand(GlobalVariable.command); // Thực hiện lệnh
        }
    }

    private void BackToMenu()
    {
        dataField.SetActive(false); // Tắt dataField
    }

    private void Update() // Kiểm tra các trường hợp của các lệnh
    {
        if (GlobalVariable.command != null && GlobalVariable.send_Request_Status)
        {
            ResetData();
            dataField.SetActive(false);
            GlobalVariable.send_Request_Status = false;
        }
    }

    public void CheckCommand(string command) // Kiểm tra lệnh và hiển thị các inputField cần thiết
    {
        if (dataField.activeSelf)
        {
            foreach (var inputField in inputFields)
            {
                inputField.transform.parent.gameObject.SetActive(true);
            }

            if (command == "Post")
            {
                AdjustTransformDataField(3);
                DisableInputField(inputFields, 3, 4);
            }
            else if (command == "Delete")
            {
                AdjustTransformDataField(1);
                DisableInputField(inputFields, 0, 3);
            }
            else if (command == "Put" || command == "Patch")
            {
                AdjustTransformDataField(4);
            }
            count = CheckListInputField(inputFields);
            // Debug.Log("Count: " + count);
        }
    }

    private void ResetData()
    {
        Debug.Log("ResetData");
        inputFields.ForEach(inputField => inputField.text = null);
    }

    private int CheckListInputField(List<TMP_InputField> inputFields) // Đếm số inputField cần thiết
    {
        int count = 0;
        foreach (var inputField in inputFields)
        {
            if (inputField.transform.parent.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    private void DisableInputField(List<TMP_InputField> inputFields, int start, int end) // Tắt các inputField không cần thiết
    {
        for (int i = start; i < end; i++)
        {
            inputFields[i].transform.parent.gameObject.SetActive(false);
        }
    }

    private void AdjustTransformDataField(int count) // Điều chỉnh kích thước của dataField
    {
        if (count == 1)
        {
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 250);
            dataField.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 300);
        }
        else if (count == 3)
        {
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 550);
            dataField.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 550);
        }
        else if (count == 4)
        {
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 700);
            dataField.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 700);
        }
    }

    private void DoCommand(string command) // Thực hiện các lệnh
    {
        switch (command)
        {
            case "Post":
                if (count == 3)
                    StartCoroutine(getDataAPI.PostDataToAPI(student_Infor_Model)); // Gọi hàm post data trong GetDataAPI
                else
                    Debug.Log("Please fill all fields POST");
                break;
            case "Put":
                if (count == 4)
                    StartCoroutine(getDataAPI.PutDataToAPI(student_Infor_Model)); // Gọi hàm put data trong GetDataAPI
                else
                    Debug.Log("Please fill all fields PUT");
                break;
            case "Delete":
                if (count == 1)
                {
                    StartCoroutine(getDataAPI.DeleteDataFromAPI(student_Infor_Model.studentId)); // Gọi hàm delete data trong GetDataAPI
                }
                else
                    Debug.Log("Please fill all fields DELETE");
                break;
            case "Patch":
                if (count == 4)
                {
                    StartCoroutine(getDataAPI.PatchDataToAPI(student_Infor_Model)); // Gọi hàm patch data trong GetDataAPI
                }
                else
                    Debug.Log("Please fill all fields PATCH");
                break;
            default:
                Debug.LogWarning("Unknown command");
                break;
        }
    }
}
