
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
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Provider;
using Android.Graphics;
using Uri = Android.Net.Uri;
using System.Net;
using Java.Net;

namespace Wallpapersofhappiness
{
	[Activity (Label = "SelectedPhotoActivity")]			
	public class BaseActivity : ActionBarActivity
	{
		private const int REQUEST_IMAGE_CAPTURE = 1;
		private const int REQUEST_IMAGE_ALBUM = 2;
		private Bitmap _bitmap;
		private Uri selectedImage;
		private Java.IO.File photo;
		private DrawerLayout drawerLayout;
		private ActionBarDrawerToggle mDrawerToggle;
		private Toolbar toolbar;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

		}

		protected void ConstructActionBar ()
		{
			toolbar = FindViewById<Toolbar> (Resource.Id.tool_bar);

			SetSupportActionBar (toolbar);
			SupportActionBar.SetDisplayShowTitleEnabled (false);

			drawerLayout = FindViewById<DrawerLayout> (Resource.Id.DrawerLayout);
			var navigationView = FindViewById<NavigationView> (Resource.Id.nav_view);
			//			var inflate = LayoutInflater.Inflate (Resource.Layout.header, null);
			//			navigationView.AddHeaderView (inflate);	
			var takePhoto = FindViewById<LinearLayout> (Resource.Id.takephoto);
			var choseFromPhoto = FindViewById<LinearLayout> (Resource.Id.chosefromphoto);
			var externalPhoto = FindViewById<LinearLayout> (Resource.Id.externalphoto);
			var languageLayout = FindViewById<LinearLayout> (Resource.Id.languagelayout);

			var drawerToggle = new ActionBarDrawerToggle (this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
			drawerLayout.SetDrawerListener (drawerToggle);
			drawerToggle.SyncState ();

			takePhoto.Click += delegate {
				Intent pictureIntent = new Intent (MediaStore.ActionImageCapture);
				drawerLayout.CloseDrawers ();
				try {
					photo = this.CreateTemporaryFile ("picture", ".jpg");
					photo.Delete ();
				} catch (Exception ex) {
					Toast.MakeText (this, "Please check sd card", ToastLength.Short);
				}				
				selectedImage = Uri.FromFile (photo);
				pictureIntent.PutExtra (MediaStore.ExtraOutput, selectedImage);
				StartActivityForResult (pictureIntent, REQUEST_IMAGE_CAPTURE);

			};

			choseFromPhoto.Click += delegate {
				drawerLayout.CloseDrawers ();
				Intent intent = new Intent (Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
				StartActivityForResult (intent, REQUEST_IMAGE_ALBUM);

			};

			externalPhoto.Click += delegate {
				drawerLayout.CloseDrawers ();
				if (this is SelectedPhotoActivity) {
					
				} else {
					StartActivity (typeof(SelectedPhotoActivity));
					Finish ();
				}
			};

			languageLayout.Click += delegate {
				StartActivityForResult (typeof(LanguageActivity), 1);
				drawerLayout.CloseDrawers ();
			};
		}

		private Java.IO.File CreateTemporaryFile (String part, String ext)
		{
			var tempDir = Android.OS.Environment.ExternalStorageDirectory;
			tempDir = new Java.IO.File (tempDir.AbsolutePath + "/capture/");
			if (!tempDir.Exists ()) {
				tempDir.Mkdirs ();
			}
			return Java.IO.File.CreateTempFile (part, ext, tempDir);
		}

		public void SetTitle (string title)
		{
			toolbar.FindViewById<TextView> (Resource.Id.titleName).Text = title;
		}

		public void SetIconRight (int resource)
		{
			toolbar.FindViewById<ImageView> (Resource.Id.iconRight).SetImageResource (resource);
		}

		public Bitmap GetImageBitmapFromUrl (string url)
		{
			Bitmap imageBitmap = null;
			Bitmap newImageBitmap = null;
			using (var webClient = new WebClient ()) {
				try {					
					var position = url.IndexOf ("upload");
					var subUrl = url.Substring (0, position + 7);
					var afterUrl = url.Substring (position + 7);
					var newUrl = string.Format ("{0}w_{1},h_{2},c_fill/{3}", subUrl, Resources.DisplayMetrics.WidthPixels / 3, Resources.DisplayMetrics.HeightPixels / 3, afterUrl);
					var imageBytes = webClient.DownloadData (newUrl);
					if (imageBytes != null && imageBytes.Length > 0) {
						imageBitmap = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
						//					var heigh = (int)context.Resources.DisplayMetrics.HeightPixels / 4;
						//					var width = Convert.ToInt32 (context.Resources.DisplayMetrics.WidthPixels / 3.5);
						//					newImageBitmap = GetResizedBitmap (imageBitmap, heigh, width);
					}
				} catch (Exception ex) {
					var a = 0;
				}

			}

			return imageBitmap;
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if ((requestCode == REQUEST_IMAGE_CAPTURE) && (resultCode == Result.Ok)) {	

				ContentResolver.NotifyChange (selectedImage, null);
				ContentResolver cr = ContentResolver;
				try {
					var intent = new Intent (this, typeof(PictureActivity));
					intent.SetDataAndType (selectedImage, "image/*");
					intent.PutExtra ("image-path", selectedImage.ToString ());
					StartActivityForResult (intent, REQUEST_IMAGE_CAPTURE);
				} catch (Exception e) {
					Toast.MakeText (this, "Failed to load capture", ToastLength.Short).Show ();
				}
			} else if ((requestCode == REQUEST_IMAGE_ALBUM) && (resultCode == Result.Ok) && (data != null)) {
				selectedImage = data.Data;
				try {
					var intent = new Intent (this, typeof(PictureActivity));
					intent.SetDataAndType (selectedImage, "image/*");
					intent.PutExtra ("image-path", selectedImage.ToString ());
					StartActivityForResult (intent, REQUEST_IMAGE_ALBUM);
					_bitmap = MediaStore.Images.Media.GetBitmap (this.ContentResolver, selectedImage);

				} catch (Exception) {

					var toast = Toast.MakeText (this, "Failed to load", ToastLength.Short);
					toast.Show ();
				}
			}
		}
	}
}

