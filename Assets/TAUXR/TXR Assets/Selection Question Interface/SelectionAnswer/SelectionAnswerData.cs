using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SelectionAnswerData
{
    [TextArea]
    public string Text;
    public bool IsCorrect;
    public string AnswerInfo;
}