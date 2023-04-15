using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace YARG {
	public class UpdateChecker : MonoBehaviour {
		private CancellationTokenSource updateTokenSource;

		public bool CheckedForUpdates { get; private set; }
		public bool IsOutOfDate { get; private set; }

		public string LatestVersion { get; private set; }

		private void Start() {
			CheckForUpdates();
		}

		private async void CheckForUpdates() {
			// #if UNITY_EDITOR
			// return;
			// #endif
			updateTokenSource = new CancellationTokenSource();

			Debug.Log("Checking for updates");
			CheckedForUpdates = true;

			try {
				// Setup request
				var request = (HttpWebRequest) WebRequest.Create("https://api.github.com/repos/EliteAsian123/YARG/releases/latest");
				request.UserAgent = "YARG";

				// Sets up a cancellation token to cancel the request if needed (such as menu being hidden)
				await using (updateTokenSource.Token.Register(() => request.Abort(), useSynchronizationContext: false)) {
					var response = await request.GetResponseAsync();
					updateTokenSource.Token.ThrowIfCancellationRequested();

					if (response is null) {
						throw new WebException("Failed to get response from Update check");
					}

					// Read response
					string json = await new StreamReader(response.GetResponseStream()!).ReadToEndAsync();

					// Get version tag
					var jsonObject = JsonConvert.DeserializeObject<JToken>(json);
					var releaseTag = jsonObject["tag_name"]!.Value<string>();

					LatestVersion = releaseTag;

					if (Constants.VERSION_TAG != LatestVersion) {
						IsOutOfDate = true;
					}

					Debug.Log(LatestVersion != Constants.VERSION_TAG
						? $"Update available! New version: {releaseTag}"
						: "Game is up to date.");
				}
			} catch (Exception e) {
				Debug.Log("Update check cancelled");
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			} finally {
				updateTokenSource.Dispose();
				updateTokenSource = null;
			}
		}
	}
}