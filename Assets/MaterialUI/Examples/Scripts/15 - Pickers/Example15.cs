//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using MaterialUI;
using Random = UnityEngine.Random;

public class Example15 : MonoBehaviour
{
    public void OnTimePickerButtonClicked()
    {
        DialogManager.ShowTimePicker(DateTime.MinValue.AddHours(Random.Range(0, 24)).AddMinutes(Random.Range(0, 60)), time => ToastManager.Show(time.ToShortTimeString()), MaterialColor.Random500());
    }

    public void OnDatePickerButtonClicked()
    {
        DialogManager.ShowDatePicker(Random.Range(1980, 2050), Random.Range(1, 12), Random.Range(1, 30), (System.DateTime date) =>
        {
            ToastManager.Show(date.ToString("dd MMM, yyyy"));
        }, MaterialColor.Random500());
    }
}
