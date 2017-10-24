//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Text.RegularExpressions;
using MaterialUI;
using UnityEngine.UI;
using UnityEngine;

public class Example07LetterValidation : MonoBehaviour, ITextValidator
{
	private MaterialInputField m_MaterialInputField;

	public void Init(MaterialInputField materialInputField)
	{
		m_MaterialInputField = materialInputField;
	}

	public bool IsTextValid()
    {
		if (new Regex("[^a-zA-Z ]").IsMatch(m_MaterialInputField.inputField.text))
        {
			m_MaterialInputField.validationText.text = "Must only contain letters";
            return false;
        }
        else
        {
            return true;
        }
    }
}