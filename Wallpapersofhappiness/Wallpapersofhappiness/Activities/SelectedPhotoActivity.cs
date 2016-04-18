
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
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.Design;
using Android.Support.Design.Widget;
using Android.Content.Res;

namespace Wallpapersofhappiness
{
	[Activity (Label = "SelectedPhotoActivity", Theme = "@style/AppTheme")]			
	public class SelectedPhotoActivity : BaseActivity
	{
		private const int REQUEST_IMAGE_CAPTURE = 1;
		private const int REQUEST_IMAGE_ALBUM = 2;
		private Bitmap _bitmap;
		private Uri selectedImage;
		private Java.IO.File photo;
		private DrawerLayout drawerLayout;
		private ActionBarDrawerToggle mDrawerToggle;

		//		protected override void OnCreate (Bundle bundle)
		//		{
		//			base.OnCreate (bundle);
		//			SetContentView (Resource.Layout.select_photo_layout);
		//			var takePhoto = FindViewById<LinearLayout> (Resource.Id.takephoto);
		//			var importPhoto = FindViewById<LinearLayout> (Resource.Id.importphoto);
		//			takePhoto.Click += delegate {
		//
		//				Intent pictureIntent = new Intent (MediaStore.ActionImageCapture);
		//
		//				try {
		//					photo = this.CreateTemporaryFile ("picture", ".jpg");
		//					photo.Delete ();
		//				} catch (Exception e) {
		//					Toast.MakeText (this, "Please check sd card", ToastLength.Short);
		//				}
		//
		//				selectedImage = Uri.FromFile (photo);
		//				pictureIntent.PutExtra (MediaStore.ExtraOutput, selectedImage);
		//				StartActivityForResult (pictureIntent, REQUEST_IMAGE_CAPTURE);
		//			};
		//			importPhoto.Click += delegate {
		//				Intent intent = new Intent (Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
		//				StartActivityForResult (intent, REQUEST_IMAGE_ALBUM);
		//			};
		//
		//		}


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.select_photo_drawer);
			ConstructActionBar ();
//			var toolbar = FindViewById<Toolbar> (Resource.Id.tool_bar);
//			SetSupportActionBar (toolbar);
//			SupportActionBar.SetDisplayShowTitleEnabled (false);
//			drawerLayout = FindViewById<DrawerLayout> (Resource.Id.DrawerLayout);
//			var navigationView = FindViewById<NavigationView> (Resource.Id.nav_view);
////			var inflate = LayoutInflater.Inflate (Resource.Layout.header, null);
////			navigationView.AddHeaderView (inflate);	
//			var takePhoto = FindViewById<LinearLayout> (Resource.Id.takephoto);
//			var choseFromPhoto = FindViewById<LinearLayout> (Resource.Id.chosefromphoto);
//			var externalPhoto = FindViewById<LinearLayout> (Resource.Id.externalphoto);
//			var languageLayout = FindViewById<LinearLayout> (Resource.Id.languagelayout);
//
//			var drawerToggle = new ActionBarDrawerToggle (this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
//			drawerLayout.SetDrawerListener (drawerToggle);
//			drawerToggle.SyncState ();
//
//			takePhoto.Click += delegate {
//				Intent pictureIntent = new Intent (MediaStore.ActionImageCapture);
//				drawerLayout.CloseDrawers ();
//				try {
//					photo = this.CreateTemporaryFile ("picture", ".jpg");
//					photo.Delete ();
//				} catch (Exception ex) {
//					Toast.MakeText (this, "Please check sd card", ToastLength.Short);
//				}				
//				selectedImage = Uri.FromFile (photo);
//				pictureIntent.PutExtra (MediaStore.ExtraOutput, selectedImage);
//				StartActivityForResult (pictureIntent, REQUEST_IMAGE_CAPTURE);
//
//			};
//
//			choseFromPhoto.Click += delegate {
//				drawerLayout.CloseDrawers ();
//				Intent intent = new Intent (Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
//				StartActivityForResult (intent, REQUEST_IMAGE_ALBUM);
//
//			};
//
//			externalPhoto.Click += delegate {
//				drawerLayout.CloseDrawers ();
//			};
//
//			languageLayout.Click += delegate {
//				drawerLayout.CloseDrawers ();
//			};
//
//
//
//		}
//
//
//			
//	
//
//
//
//
//		private Java.IO.File CreateTemporaryFile (String part, String ext)
//		{
//			var tempDir = Android.OS.Environment.ExternalStorageDirectory;
//			tempDir = new Java.IO.File (tempDir.AbsolutePath + "/capture/");
//			if (!tempDir.Exists ()) {
//				tempDir.Mkdirs ();
//			}
//			return Java.IO.File.CreateTempFile (part, ext, tempDir);
//		}
//
//		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
//		{
//			if ((requestCode == REQUEST_IMAGE_CAPTURE) && (resultCode == Result.Ok)) {	
//
//				ContentResolver.NotifyChange (selectedImage, null);
//				ContentResolver cr = ContentResolver;
//				try {
//					var intent = new Intent (this, typeof(PictureActivity));
//					intent.SetDataAndType (selectedImage, "image/*");
//					intent.PutExtra ("image-path", selectedImage.ToString ());
//					StartActivityForResult (intent, REQUEST_IMAGE_CAPTURE);
//				} catch (Exception e) {
//					Toast.MakeText (this, "Failed to load capture", ToastLength.Short).Show ();
//				}
//			} else if ((requestCode == REQUEST_IMAGE_ALBUM) && (resultCode == Result.Ok) && (data != null)) {
//				selectedImage = data.Data;
//				try {
//					var intent = new Intent (this, typeof(PictureActivity));
//					intent.SetDataAndType (selectedImage, "image/*");
//					intent.PutExtra ("image-path", selectedImage.ToString ());
//					StartActivityForResult (intent, REQUEST_IMAGE_ALBUM);
//					_bitmap = MediaStore.Images.Media.GetBitmap (this.ContentResolver, selectedImage);
//
//				} catch (Exception) {
//					
//					var toast = Toast.MakeText (this, "Failed to load", ToastLength.Short);
//					toast.Show ();
//				}
//			}
		}
	}
}

