using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Plus;
using Android.Gms.Common;
using Android.Media;
using Android.Gms.Plus.Model.People;
using System.Collections.Generic;
using com.refractored.monodroidtoolkit.imageloader;

namespace GooglePlusSignIn
{
	[Activity (Label = "GooglePlusSignIn", Icon = "@drawable/ic_launcher", Theme = "@style/Theme", MainLauncher = true)]
	public class MainActivity : Activity, IGooglePlayServicesClientConnectionCallbacks, IGooglePlayServicesClientOnConnectionFailedListener, PlusClient.IOnAccessRevokedListener
	{
		private static int REQUEST_CODE_RESOLVE_ERR = 9000;
		//request code must be >= 0
		private static int PLUS_ONE_REQUEST_CODE = 0;

		PlusClient plusClient;
		ConnectionResult connectionResult;
		ProgressDialog progressDialog;
		PlusOneButton plusOneButton;
		ImageLoader imageLoader; 
		SignInButton googleLoginButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			imageLoader = new ImageLoader (this);
			progressDialog = new ProgressDialog (this);
			progressDialog.Indeterminate = true;
			progressDialog.SetMessage ("Connecting");

			plusClient = new PlusClient.Builder(this, this, this).Build();

			googleLoginButton = FindViewById<SignInButton> (Resource.Id.sign_in_button);
			plusOneButton = FindViewById<PlusOneButton> (Resource.Id.plus_one_button);

			var logoutButton = FindViewById<Button> (Resource.Id.logout_button);
			logoutButton.Click += (sender, e) => {
				if(!plusClient.IsConnected || plusClient.IsConnecting)
					return;

				plusClient.RevokeAccessAndDisconnect(this);
			};

			googleLoginButton.Click += (sender, e) => {
				if(plusClient.IsConnected || plusClient.IsConnecting)
					return;

				progressDialog.Show();

				if (connectionResult == null) {
					plusClient.Connect();
				} 
				else{
					ResolveLogin(connectionResult);
				}
			};

			plusClient.RegisterConnectionCallbacks (this);
			plusClient.IsConnectionFailedListenerRegistered (this);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (requestCode != REQUEST_CODE_RESOLVE_ERR)
				return;

			if (resultCode == Result.Ok) {
				plusClient.Connect ();
			} else {
				progressDialog.Dismiss ();
			}
		}

		public void OnConnected (Bundle p0)
		{
			progressDialog.Dismiss ();
			RefreshPlusOneButton ();
			UpdateProfile ();
			Toast.MakeText (this, "Connected!", ToastLength.Long).Show ();
		}

		public void OnDisconnected ()
		{
			Toast.MakeText (this, "Disconnected!", ToastLength.Long).Show ();
		}

		public void OnConnectionFailed (ConnectionResult result)
		{
			if (progressDialog.IsShowing) {
				ResolveLogin (result);
			}

			connectionResult = result;
			Toast.MakeText (this, "Connection Failed!", ToastLength.Long).Show ();
		}

		/// <summary>
		/// Checks to see if we have a resolution
		/// </summary>
		/// <param name="result">Result.</param>
		private void ResolveLogin(ConnectionResult result)
		{
			if (result.HasResolution) {
				try {
					result.StartResolutionForResult (this, REQUEST_CODE_RESOLVE_ERR);
				} catch (Android.Content.IntentSender.SendIntentException e) {
					plusClient.Connect ();
				}
			} else if(progressDialog != null && progressDialog.IsShowing) {
				progressDialog.Dismiss ();
			}
		}

		public void OnAccessRevoked (ConnectionResult p0)
		{
			RefreshPlusOneButton ();
		}

		private void RefreshPlusOneButton ()
		{	
			plusOneButton.Initialize ("http://blog.xamarin.com/microsoft-and-xamarin-partner-globally/", PLUS_ONE_REQUEST_CODE);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			if (plusClient.IsConnected)
				RefreshPlusOneButton ();

		}

		protected override void OnStart ()
		{
			base.OnStart ();
			plusClient.Connect ();
		}

		protected override void OnStop ()
		{
			base.OnStop ();
			plusClient.Disconnect ();
		}

		private void UpdateProfile()
		{
			var imageView = FindViewById<ImageView> (Resource.Id.profile_image);
			FindViewById<TextView> (Resource.Id.profile_name).Text = plusClient.CurrentPerson.DisplayName;
			FindViewById<TextView> (Resource.Id.profile_nickname).Text = plusClient.CurrentPerson.Nickname;
			FindViewById<TextView> (Resource.Id.profile_about).Text = plusClient.CurrentPerson.AboutMe;
			imageLoader.DisplayImage (plusClient.CurrentPerson.Image.Url, imageView, Resource.Drawable.default_icon); 
		}
	}
}


