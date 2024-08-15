using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Configurations/SelectionAnswerButton", fileName = "SelectionAnswerButtonConfiguration")]
public class SelectionAnswerButtonConfiguration : ScriptableObject
{
    public int TimeFromSubmitToDisable;
    public Color AnswerColorAfterSubmission;
}