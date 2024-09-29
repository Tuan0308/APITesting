using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudentUI : MonoBehaviour
{
    public TMP_Text dataName;
    public TMP_Text dataPhoneNumber;
    public TMP_Text dataAddress;
    public TMP_Text dataStudentId;

    public void SetData(Student_Infor_Model data)
    {
        dataName.text = data.name;
        dataPhoneNumber.text = data.phoneNumber.ToString();
        dataAddress.text = data.address;
        dataStudentId.text = data.studentId;
    }
}
