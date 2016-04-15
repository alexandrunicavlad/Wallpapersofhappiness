
using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Provider;
using Uri = Android.Net.Uri;
using Android.Net;
using Java.IO;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Util;
using System.IO;


namespace Wallpapersofhappiness
{
	[Activity (Label = "SelectedPhotoActivity", Theme = "@style/NoActionBar")]			
	public class SelectedPhotoActivity : Activity
	{
		private const int REQUEST_IMAGE_CAPTURE = 1;
		private const int REQUEST_IMAGE_ALBUM = 2;
		private Bitmap _bitmap;
		private Uri selectedImage;
		private Java.IO.File photo;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.select_photo_layout);
			var takePhoto = FindViewById<LinearLayout> (Resource.Id.takephoto);
			var importPhoto = FindViewById<LinearLayout> (Resource.Id.importphoto);
			takePhoto.Click += delegate {
		
				Intent pictureIntent = new Intent (MediaStore.ActionImageCapture);
		
				try {
					photo = this.CreateTemporaryFile ("picture", ".jpg");
					photo.Delete ();
				} catch (Exception e) {
					Toast.MakeText (this, "Please check sd card", ToastLength.Short);
				}
		
				selectedImage = Uri.FromFile (photo);
				pictureIntent.PutExtra (MediaStore.ExtraOutput, selectedImage);
				StartActivityForResult (pictureIntent, REQUEST_IMAGE_CAPTURE);
			};
			importPhoto.Click += delegate {
				Intent intent = new Intent (Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
				StartActivityForResult (intent, REQUEST_IMAGE_ALBUM);
			};
		
		}

		//		protected override void OnCreate (Bundle bundle)
		//		{
		//			base.OnCreate (bundle);
		//			SetContentView (Resource.Layout.select_photo_layout);
		//			//var toolbar = FindViewById<Toolbar> (Resource.Id.my_awesome_toolbar);
		//
		//		}

		private Java.IO.File CreateTemporaryFile (String part, String ext)
		{
			var tempDir = Android.OS.Environment.ExternalStorageDirectory;
			tempDir = new Java.IO.File (tempDir.AbsolutePath + "/capture/");
			if (!tempDir.Exists ()) {
				tempDir.Mkdirs ();
			}
			return Java.IO.File.CreateTempFile (part, ext, tempDir);
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

