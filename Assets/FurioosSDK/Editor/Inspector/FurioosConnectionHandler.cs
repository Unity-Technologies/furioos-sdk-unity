using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DdpClient;
using DdpClient.Models;
using DdpClient.Models.Server;

namespace FurioosSDK.Editor {
	public class FurioosConnectionHandler {
		private DdpConnection _client;

		public bool Connected { get; set; }
		public bool Logged { get; set; }
		public string UserID { get; set; }
		public OrganizationCollection Organization { get; set; }
		public RegionCollection Region { get; set; }
		public bool RegionReady { get; set; }
		public SubscriptionCollection Subscription { get; set; }
		public QuotaCollection Quota { get; set; }
		public bool QuotaReady { get; set; }
		public StorageCollection Storage { get; set; }
		public bool StorageReady { get; set; }
		public ApplicationCollection[] Applications { get; set; }
		public bool ApplicationReady { get; set; }

		public void Connect(string url, bool ssl) {
			_client = new DdpConnection();
			_client.Login += OnLogin;
			_client.Connected += OnConnected;
			_client.Connect(url, ssl);
			_client.Error += OnError;
			_client.Closed += OnClosed;
		}

		void OnConnected(object sender, ConnectResponse connectResponse) {
			if (connectResponse.DidFail()) {
				Debug.Log("Connecting Failed, Server wants Version: " + connectResponse.Failed.Version);
				return;
			}

			Connected = true;
		}

		public void LoginEmail(string email, string password) {
			_client.LoginWithEmail(email, password);
		}

		public void LoginToken(string token) {
			_client.LoginWithToken(token);
		}

		void OnLogin(object sender, LoginResponse loginResponse) {
			if (loginResponse.HasError()) {
				Debug.Log(loginResponse.Error.Error);
				return;
			}

			UserID = loginResponse.Id;
			Logged = true;

			InitData();
		}

		void OnError(object sender, Exception error) {
			Debug.Log("Error");
			Debug.Log(error);
		}

		void OnClosed(object sender, EventArgs closeResponse) {
			Debug.Log("Error");
			Debug.Log(closeResponse);

			Connected = false;
		}

		void InitData() {
			_client.Call("organization.getByUserID", (response) => {
				if(response.HasError()) {
					Debug.Log("[Organization] | Error | " + response.Error.Error);
				}

				Organization = response.Get<OrganizationCollection>();

				_client.Call("subscription.get", (subscriptionReponse) => {
					if(response.HasError()) {
						Debug.Log("[Subscription] | Error | " + response.Error.Error);
					}

					Subscription = subscriptionReponse.Get<SubscriptionCollection>();

					_client.Call("quota.get", (quotaReponse) => {
						if (response.HasError()) {
							Debug.Log("[Quota] | Error | " + response.Error.Error);
						}

						Quota = quotaReponse.Get<QuotaCollection>();
						QuotaReady = true;
					}, Organization._id, Subscription.quotaID);
				}, Organization._id, Organization.subscriptionID);

				_client.Call("storage.get", (storageResponse) => {
					if(storageResponse.HasError()) {
						Debug.Log("[Storage] | Error | " + storageResponse.Error.Error);
					}

					Storage = storageResponse.Get<StorageCollection>();
					StorageReady = true;

					_client.Call("region.get", (regionResponse) => {
						if (response.HasError()) {
							Debug.Log(response.Error.Error);
						}

						Region = regionResponse.Get<RegionCollection>();
						RegionReady = true;
					}, Storage.regionID);
				}, Organization._id, Organization.storage.storageID);

				_client.Call("applications.getAll", (applicationReponse) => {
					if (applicationReponse.HasError()) {
						Debug.Log("[Application] | Error | " + applicationReponse.Error.Error);
					}

					Applications = applicationReponse.Get<ApplicationCollection[]>();
					ApplicationReady = true;
				}, Organization._id);
			});
		}

		public void SaveChanges(ApplicationCollection application) {
			_client.Call("application.update", (updateReponse) => {
				if(updateReponse.HasError()) {
					Debug.Log("[Application] | Error |" + updateReponse.Error.Error);
				}

				Debug.Log("Application updated");
			}, Organization._id, application);
		}
	}
}