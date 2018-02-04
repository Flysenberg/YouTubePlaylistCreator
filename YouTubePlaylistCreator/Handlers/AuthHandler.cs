using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using YouTubePlaylistCreator.Properties;

namespace YouTubePlaylistCreator.Handlers
{
	public class AuthHandler
	{
		private static string AppName = Resources.AppName;

		/// <summary>
		/// Login using OAuth 2.0, credentials and secrets are read from a client_secrets.json
		/// file created previously from http://console.developers.google.com
		/// </summary>
		/// <returns>Returns the youtube service needed to create a playlist further on</returns>
		public static async Task<YouTubeService> Login(string clientSecretsUri)
		{
			try
			{
				UserCredential credential;
				using (var stream = new FileStream(clientSecretsUri, FileMode.Open, FileAccess.Read))
				{
					credential = await GoogleWebAuthorizationBroker
						.AuthorizeAsync(
							GoogleClientSecrets.Load(stream).Secrets,
							// This OAuth 2.0 access scope allows for full read/write access to the
							// authenticated user's account.
							new[] { YouTubeService.Scope.Youtube },
							"user",
							CancellationToken.None,
							new FileDataStore(AppName)
						);
				}

				return new YouTubeService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential,
					ApplicationName = AppName
				});
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to authenticate\nException:\n" + e);
				return null;
			}
		}
	}
}
