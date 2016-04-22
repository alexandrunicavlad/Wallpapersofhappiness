
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
using Android.Graphics;
using Java.IO;
using Android.Provider;
using MoveText;
using Android.Graphics.Drawables;
using Android.Media;
using System.Net;
using Newtonsoft.Json;
using System.Threading;



namespace Wallpapersofhappiness
{
	[Activity (Label = "PictureActivity", Theme = "@style/NoActionBar")]			
	public class PictureActivity : Activity
	{

		private Bitmap bitmap;
		private MoveImageView imageView;
		private string imagePath;
		private Android.Net.Uri saveUri = null;
		private int textSize = 20;
		private TextModel textList;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.picture_layout_main);
			imageView = FindViewById<MoveImageView> (Resource.Id.picturefromcamera);

			Bundle extras = Intent.Extras;
			var imageNumber = extras.GetString ("image-number");
			if (extras != null) {
				if (imageNumber != null) {
					if (!extras.GetString ("image-number").Equals ("")) {	
						var url = extras.GetString ("image-number");
						bitmap = GetImageBitmapFromUrl (url);
					}
				} else {
					imagePath = extras.GetString ("image-path");
					saveUri = GetImageUri (imagePath);
					if (extras.GetString (MediaStore.ExtraOutput) != null) {
						saveUri = GetImageUri (extras.GetString (MediaStore.ExtraOutput));
					}
					bitmap = GetBitmap (imagePath);
				}
				imageView.SetImageBitmap (bitmap);
			}

			var cancelIcon = FindViewById<ImageView> (Resource.Id.cancelIcon);
			var saveIcon = FindViewById<ImageView> (Resource.Id.saveIcon);
			var editIcon = FindViewById<ImageView> (Resource.Id.editIcon);
			var settingIcon = FindViewById<ImageView> (Resource.Id.settingsIcon);
			var palletteIcon = FindViewById<ImageView> (Resource.Id.palletteIcon);
			var textIcon = FindViewById<ImageView> (Resource.Id.textIcon);
			var fontIcon = FindViewById<ImageView> (Resource.Id.fontIcon);

			cancelIcon.Click += delegate {
				OnBackPressed ();
			};

			saveIcon.Click += delegate {
				saveIcon.Clickable = false;
				imageView.DrawingCacheEnabled = true;
				Bitmap newbitmapnew = imageView.DrawingCache;
				SaveImage (newbitmapnew);
				OnBackPressed ();
			};
			editIcon.Click += delegate {
				ThreadPool.QueueUserWorkItem (o => GetData ());
				//imageView.EnterText (WindowManager.DefaultDisplay.Height);
				//imageView.Invalidate ();
			};
			settingIcon.Click += delegate {
				if (palletteIcon.Visibility == ViewStates.Visible) {
					settingIcon.SetImageResource (Resource.Drawable.ic_settings);
					palletteIcon.Visibility = ViewStates.Gone;
					textIcon.Visibility = ViewStates.Gone;
					fontIcon.Visibility = ViewStates.Gone;
				} else {
					settingIcon.SetImageResource (Resource.Drawable.ic_cancel);
					palletteIcon.Visibility = ViewStates.Visible;
					textIcon.Visibility = ViewStates.Visible;
					fontIcon.Visibility = ViewStates.Visible;
				}

			};

			palletteIcon.Click += delegate {
				
				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				alert.SetTitle ("Chose color");

				var infate = LayoutInflater.Inflate (Resource.Layout.color_text_layout, null);
				var seekBar = infate.FindViewById<SeekBar> (Resource.Id.edit_seekBar);
				var indicator = infate.FindViewById<TextView> (Resource.Id.text_seekBar_indicator);
				alert.SetView (infate);
				alert.SetNegativeButton ("Cancel", delegate {

				});
				AlertDialog alertDialog = alert.Show ();
				var colorSelected = Resource.Color.white;
				infate.FindViewById<ImageView> (Resource.Id.purpleColor).Click += delegate {
					colorSelected = Resource.Color.purple_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.blueColor).Click += delegate {
					colorSelected = Resource.Color.blue_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.blueLightColor).Click += delegate {
					colorSelected = Resource.Color.blue_light_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.greenColor).Click += delegate {
					colorSelected = Resource.Color.green_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
				infate.FindViewById<ImageView> (Resource.Id.pinkColor).Click += delegate {
					colorSelected = Resource.Color.pink_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.redColor).Click += delegate {
					colorSelected = Resource.Color.red_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.orangeColor).Click += delegate {
					colorSelected = Resource.Color.orange_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
				infate.FindViewById<ImageView> (Resource.Id.yellowColor).Click += delegate {
					colorSelected = Resource.Color.yellow_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
				infate.FindViewById<ImageView> (Resource.Id.blackColor).Click += delegate {
					colorSelected = Resource.Color.black_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
				infate.FindViewById<ImageView> (Resource.Id.whiteColor).Click += delegate {
					colorSelected = Resource.Color.white_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};


			};
			textIcon.Click += delegate {
				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				alert.SetTitle ("Select font size");

				var infate = LayoutInflater.Inflate (Resource.Layout.slider_size_layout, null);
				var seekBar = infate.FindViewById<SeekBar> (Resource.Id.edit_seekBar);
				var indicator = infate.FindViewById<TextView> (Resource.Id.text_seekBar_indicator);
				alert.SetView (infate);
				alert.SetPositiveButton ("Done", delegate {		
					imageView.SizeText ((float)seekBar.Progress);
					textSize = seekBar.Progress;
					imageView.Invalidate ();
				});
				alert.SetNegativeButton ("Cancel", delegate {
					
				});

				var isLoaded = false;
				seekBar.Progress = textSize;
				seekBar.LayoutChange += delegate(object sender, View.LayoutChangeEventArgs e) {
					var surfaceOrientation = Resources.Configuration.Orientation;
					if (isLoaded)
						return;

					isLoaded = true;
					indicator.Text = (seekBar.Progress).ToString ();
					indicator.Visibility = ViewStates.Visible;

					SeekBarIndicator (seekBar, indicator);
				};

				seekBar.ProgressChanged += delegate (object sender, SeekBar.ProgressChangedEventArgs args) {
					if (!args.FromUser)
						return;

					indicator.Text = (seekBar.Progress).ToString ();

					SeekBarIndicator (seekBar, indicator);
				};

				alert.Show ();
			};
			fontIcon.Click += delegate {
				var listOfTypeface = new List<Typeface> ();
				var listOfName = new List<String> ();
				listOfTypeface.Add (Typeface.Create (Typeface.Default, TypefaceStyle.Bold));
				listOfTypeface.Add (Typeface.Create (Typeface.Default, TypefaceStyle.BoldItalic));
				listOfTypeface.Add (Typeface.Create (Typeface.Default, TypefaceStyle.Italic));
				listOfTypeface.Add (Typeface.Create (Typeface.Default, TypefaceStyle.Normal));
				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				foreach (var item in listOfTypeface) {
					listOfName.Add (item.Style.ToString ());
				}
				ListView listView = new ListView (this);
				ArrayAdapter<String> adapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, listOfName);
				alert.SetAdapter (adapter, delegate(object sender, DialogClickEventArgs e) {
					imageView.TypefaceText (listOfTypeface [e.Which]);
					imageView.Invalidate ();
				});

					                               
				alert.SetTitle ("Select font type");
				alert.SetNegativeButton ("Cancel", delegate {

				});
				AlertDialog alertDialog = alert.Show ();
			};
		}

		private void GetData ()
		{
			var reqUrl = "https://wp-of-happiness.firebaseio.com/availableText.json";
			var request = (HttpWebRequest)WebRequest.Create (reqUrl);
			request.Timeout = 10000;
			request.Method = "GET";
			request.ContentType = "application/json";

			try {
				var response = (HttpWebResponse)request.GetResponse ();
				var reader = new System.IO.StreamReader (response.GetResponseStream ());
				var streamText = reader.ReadToEnd ();
				textList = JsonConvert.DeserializeObject <TextModel> (streamText);

			} catch (Exception ex) {
				//HandleErrors (ex);
				var a = 0;
			}


			RunOnUiThread (() => {
				if (textList.ToString ().Equals ("")) {
					Toast.MakeText (this, GetString (Resource.String.ValidationText), ToastLength.Short);
				} else {
					SelectText ();
				}
			});
		}


		private void SeekBarIndicator (SeekBar seekBar, TextView indicator)
		{
			RelativeLayout.LayoutParams p = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
			Rect thumbRect = seekBar.Thumb.Bounds;
			var abc = thumbRect.CenterX ();				
			p.SetMargins (abc, 0, 0, 0);
			indicator.LayoutParameters = p;
		}


		private void SelectText ()
		{
			Dialog dialog = new Dialog (this);
//			dialog.Window.RequestFeature (WindowFeatures.NoTitle);
			dialog.SetContentView (Resource.Layout.textItems_listview);
			dialog.Window.SetGravity (GravityFlags.Center);
			dialog.Window.SetLayout (WindowManager.DefaultDisplay.Width - 100, WindowManagerLayoutParams.WrapContent);
			dialog.SetCancelable (true);
			dialog.SetCanceledOnTouchOutside (true);
			var listView = dialog.FindViewById<ListView> (Resource.Id.listview);
			var values = textList.english;
			var listAdapter = new TextListAdapter (this, values);
			listView.Adapter = listAdapter;
			listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
				dialog.Dismiss ();
				imageView.PutText (values [e.Position]);
				imageView.EnterText (WindowManager.DefaultDisplay.Height);
				imageView.Invalidate ();
			};
			dialog.Show ();

		}

		private Bitmap GetBitmap (String path)
		{
			var uri = GetImageUri (path);

			var Uti = Android.Net.Uri.Parse (path);

			System.IO.Stream ins = null;

			try {
				int IMAGE_MAX_SIZE = 1024;
				ins = ContentResolver.OpenInputStream (Uti);

				// Decode image size
				var o = new BitmapFactory.Options ();
				o.InJustDecodeBounds = true;

				BitmapFactory.DecodeStream (ins, null, o);
				ins.Close ();

				int scale = 1;
				if (o.OutHeight > IMAGE_MAX_SIZE || o.OutWidth > IMAGE_MAX_SIZE) {
					scale =
						(int)
						Math.Pow (2,
						(int)
							Math.Round (Math.Log (IMAGE_MAX_SIZE / (double)Math.Max (o.OutHeight, o.OutWidth)) /
						Math.Log (0.5)));
				}

				var o2 = new BitmapFactory.Options ();
				o2.InSampleSize = scale;
				ins = ContentResolver.OpenInputStream (Uti);
				Bitmap b = BitmapFactory.DecodeStream (ins, null, o2);
				var abc = WindowManager.DefaultDisplay;
				if (b.Width >= WindowManager.DefaultDisplay.Width) {
					imageView.SetScaleType (ImageView.ScaleType.FitXy);
				}
				ins.Close ();

				return b;
			} catch (Exception e) {
				//Log.Error (GetType ().Name, e.Message);
			}

			return null;
		}

		public Bitmap rotateBitmap (String src, Bitmap bitmap, Android.Net.Uri uti)
		{
			try {				
				var orientation = 1;

				if (orientation == 1) {
					return bitmap;
				}

				Matrix matrix = new Matrix ();
				switch (orientation) {
				case 2:
					matrix.SetScale (-1, 1);
					break;
				case 3:
					matrix.SetRotate (180);
					break;
				case 4:
					matrix.SetRotate (180);
					matrix.PostScale (-1, 1);
					break;
				case 5:
					matrix.SetRotate (90);
					matrix.PostScale (-1, 1);
					break;
				case 6:
					matrix.SetRotate (90);
					break;
				case 7:
					matrix.SetRotate (-90);
					matrix.PostScale (-1, 1);
					break;
				case 8:
					matrix.SetRotate (-90);
					break;
				default:
					return bitmap;
				}

				try {
					Bitmap oriented = Bitmap.CreateBitmap (bitmap, 0, 0,
						                  bitmap.Width, bitmap.Height, matrix, true);
					bitmap.Recycle ();
					return oriented;
				} catch (Java.Lang.OutOfMemoryError e) {
					e.PrintStackTrace ();
					return bitmap;
				}
			} catch (IOException e) {
				e.PrintStackTrace ();
			}

			return bitmap;
		}

		private Android.Net.Uri GetImageUri (String path)
		{
			return Android.Net.Uri.FromFile (new Java.IO.File (path));
		}

		private void SaveImage (Bitmap finalBitmap)
		{

			String root = Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryPictures).ToString ();

			File myDir = new File (root + "/Wallpapers");    
			myDir.Mkdirs ();
			Random generator = new Random ();
			int n = 10000;
			n = generator.Next (n);
			String fname = "Image-" + n + ".jpg";
			File file = new File (myDir, fname);
			if (file.Exists ())
				file.Delete (); 
			try {
				FileOutputStream fo = new FileOutputStream (file);
				System.IO.MemoryStream stream = new System.IO.MemoryStream ();
				finalBitmap.Compress (Bitmap.CompressFormat.Png, 100, stream);
				finalBitmap.Recycle ();
				byte[] byteArray = stream.ToArray ();
				fo.Write (byteArray);
				fo.Close ();

			} catch (Exception e) {
				var mess = e.Message;
			}
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

		public Bitmap GetImageBitmapFromUrl (string url)
		{
			Bitmap imageBitmap = null;
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
					var a = 0;
				}
			
			}
			return imageBitmap;
		}

		public Bitmap LoadAndResizeBitmap (string fileName, int width, int height)
		{			
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile (fileName, options);


			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (outHeight > height || outWidth > width) {
				inSampleSize = outWidth > outHeight
					? outHeight / height
					: outWidth / width;
			}

			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile (fileName, options);

			return resizedBitmap;
		}
	}


}

