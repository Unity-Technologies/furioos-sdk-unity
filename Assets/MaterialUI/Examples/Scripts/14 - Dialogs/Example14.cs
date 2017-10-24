//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using MaterialUI;
using System.Collections;

public class Example14 : MonoBehaviour
{
	[SerializeField] private Sprite[] m_IconSpriteArray;

	private string[] m_SmallStringList = new string[] { "One", "Two", "Three", "Four" };
	private string[] m_BigStringList = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen" };

	public void OnProgressLinearButtonClicked()
	{
		DialogProgress dialog = DialogManager.ShowProgressLinear("Doing some hard work... Should be done in 5 seconds!", "Loading", MaterialIconHelper.GetIcon(MaterialIconEnum.HOURGLASS_EMPTY));
		StartCoroutine(HideWindowAfterSeconds(dialog, 5.0f));
	}

	public void OnProgressCircularButtonClicked()
	{
		DialogProgress dialog = DialogManager.ShowProgressCircular("Doing some hard work... Should be done in 5 seconds!", "Loading", MaterialIconHelper.GetIcon(MaterialIconEnum.HOURGLASS_EMPTY));
		StartCoroutine(HideWindowAfterSeconds(dialog, 5.0f));
	}

	private IEnumerator HideWindowAfterSeconds(MaterialDialog dialog, float duration)
	{
		yield return new WaitForSeconds(duration);
		dialog.Hide();
	}

	public void OnSimpleListSmallButtonClicked()
	{
		DialogManager.ShowSimpleList(m_SmallStringList, (int selectedIndex) => {
			ToastManager.Show("Item #" + selectedIndex + " selected: " + m_SmallStringList[selectedIndex]);
		});
	}

	public void OnSimpleListBigVectorButtonClicked()
	{
		OptionDataList optionDataList = new OptionDataList();
		for (int i = 0; i < m_BigStringList.Length; i++)
		{
			string itemValue = m_BigStringList[i];

			OptionData optionData = new OptionData(itemValue, MaterialIconHelper.GetRandomIcon(), () => { Debug.Log("I am selected: " + itemValue); });
			optionDataList.options.Add(optionData);
		}

		DialogManager.ShowSimpleList(optionDataList, (int selectedIndex) => {
			ToastManager.Show("Item #" + selectedIndex + " selected: " + m_BigStringList[selectedIndex]);
		}, "Big Simple List - Vector", MaterialIconHelper.GetRandomIcon());
	}

	public void OnSimpleListBigSpriteButtonClicked()
	{
		OptionDataList optionDataList = new OptionDataList();
		for (int i = 0; i < m_IconSpriteArray.Length; i++)
		{
			string itemName = m_IconSpriteArray[i].name.Replace("icon_", "").Replace("_", " ");
			itemName = itemName.Substring(0, 1).ToUpper() + itemName.Substring(1, itemName.Length - 1);

			OptionData optionData = new OptionData(itemName, new ImageData(m_IconSpriteArray[i]), () => { Debug.Log("I am selected: " + itemName); });
			optionDataList.options.Add(optionData);
		}

		DialogManager.ShowSimpleList(optionDataList, (int selectedIndex) => {
			ToastManager.Show("Item #" + selectedIndex + " selected: " + m_IconSpriteArray[selectedIndex].name);
		}, "Big Simple List - Sprite", new ImageData(m_IconSpriteArray[Random.Range(0, m_IconSpriteArray.Length)]));
	}

	public void OnAlertSimpleButtonClicked()
	{
		DialogManager.ShowAlert("Hello world", "Alert!", MaterialIconHelper.GetRandomIcon());
	}

	public void OnAlertOneCallbackButtonClicked()
	{
		DialogManager.ShowAlert("Example with just one button", () => { ToastManager.Show("You clicked the affirmative button"); }, "OK", "One callback", MaterialIconHelper.GetRandomIcon());
	}

	public void OnAlertTwoCallbacksButtonClicked()
	{
		DialogManager.ShowAlert("Example with one affirmative and one dismissive button", () => { ToastManager.Show("You clicked the affirmative button"); }, "YES", "Two callbacks", MaterialIconHelper.GetRandomIcon(), () => { ToastManager.Show("You clicked the dismissive button"); }, "NO");
	}

	public void OnAlertFromLeftButtonClicked()
	{
		DialogAlert dialog = DialogManager.CreateAlert();
		dialog.Initialize("Example with a dialog animation that comes from the left", null, "OK", "Alert!", MaterialIconHelper.GetRandomIcon(), null, null);
		dialog.dialogAnimator = new DialogAnimatorSlide(0.5f, DialogAnimatorSlide.SlideDirection.Left, DialogAnimatorSlide.SlideDirection.Right);
		dialog.Show();
	}

	public void OnCheckboxListSmallButtonClicked()
	{
		DialogManager.ShowCheckboxList(m_SmallStringList, OnCheckboxValidateClicked, "OK");
	}

	public void OnCheckboxListBigButtonClicked()
	{
		DialogManager.ShowCheckboxList(m_BigStringList, OnCheckboxValidateClicked, "OK", "Big Checkbox List", MaterialIconHelper.GetRandomIcon(), () => { ToastManager.Show("You clicked the cancel button"); }, "CANCEL");
	}

	private void OnCheckboxValidateClicked(bool[] resultArray)
	{
		string result = "";
		for (int i = 0; i < resultArray.Length; i++)
		{
			result += resultArray[i] + ((i < resultArray.Length - 1) ? " ," : "");
		}

		ToastManager.Show(result);
	}

	public void OnRadioListSmallButtonClicked()
	{
		DialogManager.ShowRadioList(m_SmallStringList, (int selectedIndex) => {
			ToastManager.Show("Item #" + selectedIndex + " selected: " + m_SmallStringList[selectedIndex]);
		}, "OK", 3);
	}

	public void OnRadioListBigButtonClicked()
	{
		DialogManager.ShowRadioList(m_BigStringList, (int selectedIndex) => {
			ToastManager.Show("Item #" + selectedIndex + " selected: " + m_BigStringList[selectedIndex]);
		}, "OK", "Big Radio List", MaterialIconHelper.GetRandomIcon(), () => { ToastManager.Show("You clicked the cancel button"); }, "CANCEL");
	}

	public void OnOneFieldPromptButtonClicked()
	{
		DialogManager.ShowPrompt("Username", (string inputFieldValue) => { ToastManager.Show("Returned: " + inputFieldValue); }, "OK", "Prompt One Field", MaterialIconHelper.GetRandomIcon(), () => { ToastManager.Show("You clicked the cancel button"); }, "CANCEL"); 
	}

	public void OnTwoFieldsPromptButtonClicked()
	{
		DialogManager.ShowPrompt("First name", "Last name", (string firstInputFieldValue, string secondInputFieldValue) => { ToastManager.Show("Returned: " + firstInputFieldValue + " and " + secondInputFieldValue); }, "OK", "Prompt Two Fields", MaterialIconHelper.GetRandomIcon(), () => { ToastManager.Show("You clicked the cancel button"); }, "CANCEL"); 
	}
}
