
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
using Square.Picasso;
using Java.Lang.Annotation;


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
			int size = (int)Math.Ceiling (Math.Sqrt (Resources.DisplayMetrics.WidthPixels * Resources.DisplayMetrics.HeightPixels));
			ShowImage ();

			saveButton.Click += delegate {	
				if (retry)
					return;
				saveButton.Clickable = false;
				var bitm = ((BitmapDrawable)imageView.Drawable).Bitmap;
				//imageView.DrawingCacheEnabled = true;
				//Bitmap newbitmapnew = imageView.DrawingCache;
				loading.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => SaveImage (bitm));	
			};

		}

		public void ShowImage ()
		{
			var imageNumber = extras.GetString ("image-number");
			var imageName = extras.GetString ("image-name");
			var imagePath = extras.GetInt ("image-path");
			if (extras != null) {
				if (imagePath == 0) {
					if (imageNumber != null) {
						if (!extras.GetString ("image-number").Equals ("")) {	
							var url = extras.GetString ("image-number");
							Picasso.With (this).Load (url)
								.MemoryPolicy (MemoryPolicy.NoCache)
								.NetworkPolicy (NetworkPolicy.NoStore)
								.Into (imageView, delegate {							
								var b =	System.GC.GetTotalMemory (true);								
								HideRetry ();
								imageView.Visibility = ViewStates.Visible;
								loading.Visibility = ViewStates.Gone;			
							}, delegate {
								ShowRetry ();
							});	
						}
					}
				} else {	
					Picasso.With (this).Load (imagePath)
						.Fit ()
						.CenterInside ()
						.Placeholder (Resource.Drawable.seekbar_progress)
						.SkipMemoryCache ()					
						.Error (Resource.Drawable.ic_edit_pencil)
						.Into (imageView, delegate {							
						var b =	System.GC.GetTotalMemory (true);
					}, delegate {
						var a =	System.GC.GetTotalMemory (true);
					});				
					imageView.Visibility = ViewStates.Visible;
					loading.Visibility = ViewStates.Gone;	
				}
			}

		}

		public void HideRetry ()
		{
			retry = false;
			imageView.Visibility = ViewStates.Visible;
			loading.Visibility = ViewStates.Gone;
			loading.FindViewById<Button> (Resource.Id.loading_retry).Visibility = ViewStates.Gone;
			loading.FindViewById<ProgressBar> (Resource.Id.splash_progressBar).Visibility = ViewStates.Visible;
		}

		public void ShowRetry ()
		{
			retry = true;
			imageView.Visibility = ViewStates.Gone;
			loading.Visibility = ViewStates.Visible;
			loading.FindViewById<Button> (Resource.Id.loading_retry).Visibility = ViewStates.Visible;
			loading.FindViewById<ProgressBar> (Resource.Id.splash_progressBar).Visibility = ViewStates.Gone;
			loading.FindViewById<Button> (Resource.Id.loading_retry).Click += delegate {
				loading.FindViewById<Button> (Resource.Id.loading_retry).Visibility = ViewStates.Gone;
				loading.FindViewById<ProgressBar> (Resource.Id.splash_progressBar).Visibility = ViewStates.Visible;
				ShowImage ();
			};

		}

		public override void OnWindowFocusChanged (bool hasFocus)
		{
			int width = imageView.Width;
			int height = imageView.Height;
			base.OnWindowFocusChanged (hasFocus);
		}


		protected override void OnDestroy ()
		{			
			Picasso.With (this).CancelRequest (imageView);
			base.OnDestroy ();
		}

		private void ConstructActionBar ()
		{
			toolbar = FindViewById<Toolbar> (Resource.Id.tool_bar);
			SetSupportActionBar (toolbar);
			SupportActionBar.SetDisplayShowTitleEnabled (false);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetDisplayShowHomeEnabled (true);
			toolbar.NavigationClick += delegate {
				if (saveButton.Clickable == false) {
					return;
				} else {			
				
					Finish ();
				}
			};

			toolbar.NavigationIcon = Resources.GetDrawable (Resource.Drawable.ic_back);
			toolbar.FindViewById<TextView> (Resource.Id.titleName).Text = GetString (Resource.String.Previewbackground);
			saveButton = toolbar.FindViewById<ImageView> (Resource.Id.iconRight);
			saveButton.SetImageResource (Resource.Drawable.ic_save);							

		}

		public static int CalculateInSampleSize (
			BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Raw height and width of image
			int height = options.OutHeight;
			int width = options.OutWidth;
			int inSampleSize = 1;

			if (height > reqHeight || width > reqWidth) {

				int halfHeight = height / 2;
				int halfWidth = width / 2;

				// Calculate the largest inSampleSize value that is a power of 2 and keeps both
				// height and width larger than the requested height and width.
				while ((halfHeight / inSampleSize) > reqHeight
				       && (halfWidth / inSampleSize) > reqWidth) {
					inSampleSize *= 2;
				}
			}

			return inSampleSize;
		}

		public static Bitmap DecodeSampledBitmapFromResource (Resources res, int resId,
		                                                      int reqWidth, int reqHeight)
		{

			// First decode with inJustDecodeBounds=true to check dimensions
			BitmapFactory.Options options = new BitmapFactory.Options ();
			options.InJustDecodeBounds = true;
			BitmapFactory.DecodeResource (res, resId, options);

			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize (options, reqWidth, reqHeight);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;
			return BitmapFactory.DecodeResource (res, resId, options);
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
					Finish ();
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

		public Bitmap Decode (byte[] encodeByte)
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


			RunOnUiThread (() => {				
				imageView.Visibility = ViewStates.Visible;
				loading.Visibility = ViewStates.Gone;
			});		
		}


	}


	public class BitmapTransform : Java.Lang.Object, ITransformation
	{

		int maxWidth;
		int maxHeight;

		public BitmapTransform (int maxWidth, int maxHeight)
		{
			this.maxWidth = maxWidth;
			this.maxHeight = maxHeight;
		}

		public Bitmap Transform (Bitmap source)
		{
			int size = Math.Min (source.Width, source.Height);
			Bitmap result = Bitmap.CreateBitmap (source, 0, size, maxWidth, maxHeight);
			//Bitmap result = Bitmap.CreateScaledBitmap (source, 600, 800, false);

			if (result != source) {
				source.Recycle ();
			}
			return result;
		}

		//		public  Bitmap Transform (Bitmap source)
		//		{
		//			int targetWidth, targetHeight;
		//			double aspectRatio;
		//
		//			if (source.Width > source.Height) {
		//				targetWidth = maxWidth;
		//				aspectRatio = (double)source.Height / (double)source.Width;
		//				targetHeight = (int)(targetWidth * aspectRatio);
		//			} else {
		//				targetHeight = maxHeight;
		//				aspectRatio = (double)source.Width / (double)source.Height;
		//				targetWidth = (int)(targetHeight * aspectRatio);
		//			}
		//
		//			Bitmap result = Bitmap.CreateScaledBitmap (source, targetWidth, targetHeight, false);
		//			if (result != source) {
		//				source.Recycle ();
		//			}
		//			return result;
		//		}



		public string Key {
			get { return "square()"; } 
		}

	};


}

