using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading;
using Android.Graphics;

namespace Wallpapersofhappiness
{
	[Activity (MainLauncher = true, Theme = "@style/NoActionBar")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{			
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			var progressBar = FindViewById<ProgressBar> (Resource.Id.splash_progressBar);

			progressBar.IndeterminateDrawable.SetColorFilter (Resources.GetColor (Resource.Color.green_main), PorterDuff.Mode.SrcAtop);

			ThreadPool.QueueUserWorkItem (o => StartMainActivity ());

		}

		private void StartMainActivity ()
		{           
			Java.Lang.Thread.Sleep (3000);
			StartActivity (typeof(SelectedPhotoActivity));
			Finish ();
		}

	}
}
