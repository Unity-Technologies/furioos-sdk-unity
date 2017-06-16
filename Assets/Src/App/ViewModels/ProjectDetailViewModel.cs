using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectDetailViewModel : MonoBehaviour {
	[Header("Details")]
	[Space(1)]

	public GameObject nameContainer;
	public new Text name;

	[Space(5)]
	public GameObject descriptionContainer;
	public Text description;

	[Space(5)]
	public GameObject subProjectContainer;
	public RectTransform subProjectContent;
	public GameObject subProjectPrefab;

	[Space(5)]
	public GameObject dataContainer;
	public RectTransform dataContent;
	public GameObject dataPrefab;

	[Space(10)]

	[Header("Medias")]
	[Space(1)]

	public RectTransform mediaPreviewContent;
	public GameObject mediaPreviewPrefab;

	[Space(5)]
	public RectTransform mediaIndicatorContent;
	public GameObject mediaIndicatorPrefab;
}