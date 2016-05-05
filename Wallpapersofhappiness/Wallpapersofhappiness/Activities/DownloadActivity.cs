
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Graphics;
using System.Threading;
using System.Net;
using Android.Support.V4.Widget;
using Java.IO;
using Android.Content.Res;
using Android.Graphics.Drawables;

namespace Wallpapersofhappiness
{
	[Activity (Label = "DownloadActivity", Theme = "@style/AppTheme", ConfigurationChanges =
		(Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize))]			
	public class DownloadActivity : ActionBarActivity
	{
		private Toolbar toolbar;
		private Bitmap bitmap;
		private ImageView imageView;
		private Bundle extras;
		private bool retry = false;
		private RelativeLayout loading;
		private ImageView saveButton;
		private byte[] byteArray;
		private MemoryLimitedLruCache _memoryCache;
		private Bitmap imageBitmap;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.download_layout);
			ConstructActionBar ();
			var maxMemory = (int)(Java.Lang.Runtime.GetRuntime ().MaxMemory () / 1024);
			var cacheSize = maxMemory / 8;
			_memoryCache = new MemoryLimitedLruCache (cacheSize);
			imageView = FindViewById<ImageView> (Resource.Id.picturemain);
			loading = FindViewById<RelativeLayout> (Resource.Id.main_loading);
			imageView.Visibility = ViewStates.Gone;
			loading.Visibility = ViewStates.Visible;
			extras = Intent.Extras;
			var imageNumber = extras.GetString ("image-number");
			var imageName = extras.GetString ("image-name");
			var imagePath = extras.GetInt ("image-path");
			if (extras != null) {
				if (imagePath == 0) {
					if (imageNumber != null) {
						if (!extras.GetString ("image-number").Equals ("")) {	
							var url = extras.GetString ("image-number");
							ThreadPool.QueueUserWorkItem (o => GetImageBitmapFromUrl (url));
						}
					}
				} else {
					imageView.SetImageResource (imagePath);
					imageView.Visibility = ViewStates.Visible;
					loading.Visibility = ViewStates.Gone;				
				}
			}
			saveButton.Click += delegate {
				saveButton.Clickable = false;
				imageView.DrawingCacheEnabled = true;
				Bitmap newbitmapnew = imageView.DrawingCache;
				loading.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => SaveImage (newbitmapnew));	
			};

		}

		private void ConstructActionBar ()
		{
			toolbar = FindViewById<Toolbar> (Resource.Id.tool_bar);
			SetSupportActionBar (toolbar);
			SupportActionBar.SetDisplayShowTitleEnabled (false);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetDisplayShowHomeEnabled (true);
			toolbar.NavigationClick += delegate {
				OnBackPressed ();

				Finish ();
			};

			toolbar.NavigationIcon = Resources.GetDrawable (Resource.Drawable.ic_back);
			toolbar.FindViewById<TextView> (Resource.Id.titleName).Text = GetString (Resource.String.Previewbackground);
			saveButton = toolbar.FindViewById<ImageView> (Resource.Id.iconRight);
			saveButton.SetImageResource (Resource.Drawable.ic_save);							

		}




		private void SaveImage (Bitmap finalBitmap)
		{					
			File myDir = new File (Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryPictures), "Wallpapers");
			if (!myDir.Exists ()) {
				myDir.Mkdirs ();
			}
			Random generator = new Random ();
			int n = 10000;
			n = generator.Next (n);
			String fname = "Image-" + n + ".jpg";
			File file = new File (myDir, fname);
			Intent mediaScanIntent = new Intent (Intent.ActionMediaScannerScanFile);
			var contentUri = Android.Net.Uri.FromFile (file);
			mediaScanIntent.SetData (contentUri);
			SendBroadcast (mediaScanIntent);

			if (file.Exists ())
				file.Delete (); 
			try {
				FileOutputStream fo = new FileOutputStream (file);
				System.IO.MemoryStream stream = new System.IO.MemoryStream ();
				finalBitmap.Compress (Bitmap.CompressFormat.Png, 100, stream);
				finalBitmap.Recycle ();
				byteArray = stream.ToArray ();
				fo.Write (byteArray);
				fo.Close ();						
			} catch (Exception ex) {					
				retry = true;
				RunOnUiThread (() => {
					var toast = Toast.MakeText (this, GetString (Resource.String.ValidationRequestTimeOut), ToastLength.Short);
					toast.Show ();
				});
			}			
			RunOnUiThread (() => {
				if (retry) {
					return;
				}
				imageView.Visibility = ViewStates.Visible;
				loading.Visibility = ViewStates.Gone;
				var alert = new Android.App.AlertDialog.Builder (this);
				alert.SetTitle (GetString (Resource.String.saveWallpapers));
				alert.SetNegativeButton (GetString (Resource.String.cancelbutton), delegate {
					OnBackPressed ();
				});
				alert.SetPositiveButton (GetString (Resource.String.saveWallpapersMess), delegate {
					DialogToSetWallpaper (byteArray);
				});
				var alertDialog = alert.Create ();
				alertDialog.SetTitle (Resources.GetString (Resource.String.saveWallpapers));
				alertDialog.Show ();
			});
		}

		private void DialogToSetWallpaper (byte[] bitmap)
		{				
			WallpaperManager myWallpaper = WallpaperManager.GetInstance (this);
			try {
				int height = Resources.DisplayMetrics.HeightPixels;
				int width = Resources.DisplayMetrics.WidthPixels;
				myWallpaper.SetBitmap (GetResizedBitmap (Decode (bitmap), width, height));
			} catch (IOException e) {
				e.PrintStackTrace ();
			}
			OnBackPressed ();
			
		}

		public Bitmap GetResizedBitmap (Bitmap bm, int newWidth, int newHeight)
		{
			int width = bm.Width;
			int height = bm.Height;
			float scaleWidth = ((float)newWidth) / width;
			float scaleHeight = ((float)newHeight) / height;
			Matrix matrix = new Matrix ();
			matrix.PostScale (scaleWidth, scaleHeight);
			Bitmap resizedBitmap = Bitmap.CreateBitmap (bm, 0, 0, width, height, matrix, false);

			return resizedBitmap;
		}

		public  Bitmap Decode (byte[] encodeByte)
		{
			try {				
				Bitmap bitmap = BitmapFactory.DecodeByteArray (encodeByte, 0, encodeByte.Length);
				return bitmap;
			} catch (Exception) {
				return null;
			}
		}

		public void GetImageBitmapFromUrl (string url)
		{
			imageBitmap = null;
			using (var webClient = new WebClient ()) {
				try {					
					var imageBytes = webClient.DownloadData (url);
					if (imageBytes != null && imageBytes.Length > 0) {
						imageBitmap = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
						//					var heigh = (int)context.Resources.DisplayMetrics.HeightPixels / 4;
						//					var width = Convert.ToInt32 (context.Resources.DisplayMetrics.WidthPixels / 3.5);
						//					newImageBitmap = GetResizedBitmap (imageBitmap, heigh, width);
					}

				} catch (Exception ex) {					
					retry = true;
					RunOnUiThread (() => {
						var toast = Toast.MakeText (this, GetString (Resource.String.ValidationRequestTimeOut), ToastLength.Short);
						toast.Show ();
					});
				}			
				RunOnUiThread (() => {
					if (retry) {
						return;
					}
					imageView.SetImageBitmap (imageBitmap);
					imageView.Visibility = ViewStates.Visible;
					loading.Visibility = ViewStates.Gone;
				});
			}

		}

		protected override void OnStop ()
		{
			base.OnStop ();
			if (imageBitmap != null) {
				imageBitmap.Recycle ();
				imageBitmap = null;
			}
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			if (imageBitmap != null) {
				imageBitmap.Recycle ();
				imageBitmap = null;
			}
		}
	}
}

