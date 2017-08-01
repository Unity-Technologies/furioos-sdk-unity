using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rise.Core;

public class AppController : RSBehaviour {
	private string _organisationId;
	public string OrganisationId {
		get { 
			return _organisationId; 
		}
	}
}