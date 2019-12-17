using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DdpClient;
using DdpClient.Models;
using DdpClient.Models.Server;

namespace FurioosSDK.Editor {
	public class FurioosConnectionHandler : MonoBehaviour
	{
		public string ddpUrl;
		public bool ssl;

		private DdpConnection _client;

		public string UserID { get; set; }
		public OrganizationCollection Organization { get; set; }
		public RegionCollection Region { get; set; }
		public SubscriptionCollection Subscription { get; set; }
		public QuotaCollection Quota{ get; set; }
		public StorageCollection Storage { get; set; }
		public ApplicationCollection[] Applications { get; set; }

		void Start() {
			_client = new DdpConnection();
			_client.Login += OnLogin;
			_client.Connected += OnConnected;
			_client.Connect(ddpUrl, ssl);
			_client.Error += OnError;
			_client.Closed += OnClosed;
		}

		void OnConnected(object sender, ConnectResponse connectResponse) {
			Debug.Log("Connected");
			Debug.Log(connectResponse);

			if(connectResponse.DidFail()) {
				Debug.Log("Connecting Failed, Server wants Version: " + connectResponse.Failed.Version);
			}

			_client.LoginWithEmail("m.korbas@obvioos.com", "MK09J1291");
		}

		void OnLogin(object sender, LoginResponse loginResponse) {
			if (loginResponse.HasError())
				Debug.Log(loginResponse.Error.Error);

			Debug.Log("Token: " + loginResponse.Token);
			Debug.Log("Token expires In: " + loginResponse.TokenExpires.DateTime);
			Debug.Log("ID: " + loginResponse.Id);

			UserID = loginResponse.Id;

			InitData();
		}

		void OnError(object sender, Exception error) {
			Debug.Log("Error");
			Debug.Log(error);
		}

		void OnClosed(object sender, EventArgs closeResponse) {
			Debug.Log("Error");
			Debug.Log(closeResponse);
		}

		void InitData() {
			_client.Call("organization.getByUserID", (response) => {
				if(response.HasError()) {
					Debug.Log("[Organization] | Error | " + response.Error.Error);
				}

				Organization = response.Get<OrganizationCollection>();
				Debug.Log("Org ID: " + Organization._id);

				_client.Call("subscription.get", (subscriptionReponse) => {
					if(response.HasError()) {
						Debug.Log("[Subscription] | Error | " + response.Error.Error);
					}

					Subscription = subscriptionReponse.Get<SubscriptionCollection>();

					Debug.Log("Sub ID: " + Subscription._id);

					_client.Call("quota.get", (quotaReponse) => {
						if (response.HasError()) {
							Debug.Log("[Quota] | Error | " + response.Error.Error);
						}

						Quota = quotaReponse.Get<QuotaCollection>();
						Debug.Log("Quota ID: " + Quota._id);
					}, Organization._id, Subscription.quotaID);
				}, Organization._id, Organization.subscriptionID);

				_client.Call("storage.get", (storageResponse) => {
					if(storageResponse.HasError()) {
						Debug.Log("[Storage] | Error | " + storageResponse.Error.Error);
					}

					Storage = storageResponse.Get<StorageCollection>();

					_client.Call("region.get", (regionResponse) => {
						if (response.HasError()) {
							Debug.Log(response.Error.Error);
						}

						Region = regionResponse.Get<RegionCollection>();
					}, Storage.regionID);
				});

				_client.Call("applications.getAll", (applicationReponse) => {
					if (applicationReponse.HasError()) {
						Debug.Log("[Application] | Error | " + applicationReponse.Error.Error);
					}

					Applications = applicationReponse.Get<ApplicationCollection[]>();

					foreach (ApplicationCollection application in Applications) {
						Debug.Log(application.name);
					}
				}, Organization._id);
			});
		}
	}
}