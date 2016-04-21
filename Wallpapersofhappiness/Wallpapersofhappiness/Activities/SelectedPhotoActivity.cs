
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
using Java.Net;
using System.Net;
using Newtonsoft.Json;
using System.Threading;



namespace Wallpapersofhappiness
{
	[Activity (MainLauncher = true, Label = "SelectedPhotoActivity", Theme = "@style/AppTheme")]			
	public class SelectedPhotoActivity : BaseActivity
	{
		private const int REQUEST_IMAGE_CAPTURE = 1;
		private const int REQUEST_IMAGE_ALBUM = 2;
		private string requestURL = "https://api.cloudinary.com/v1_1/wp-of-happiness/resources/image/upload/?prefix=";
		private const string ApiKey = "966956932715847";
		private const string ApiSecret = "grc0mV1_k8xuV8xLYZgPGMpbwDw";
		private LinearLayout mainSlider;
		private List<ImageModel> images;
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
		private RecyclerView recyclerView;
		private List<Bitmap> bitmaps;
		private ImageAdapter adapter;
		private bool bestBool = false;
		private bool categoryBool = false;
		private bool randomBool = false;

		private bool loveBool = false;
		private bool happinesBool = false;
		private bool sportBool = false;
		private bool coupleBool = false;
		private bool motivationBool = false;

		private TextView best;
		private TextView category;
		private TextView random;
		private TextView item;

		private LinearLayout lovelayout;
		private LinearLayout happineslayout;
		private LinearLayout sportlayout;
		private LinearLayout couplelayout;
		private LinearLayout motivationlayout;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.select_photo_drawer);
			ConstructActionBar ();
			SetTitle ("Default background");

			ThreadPool.QueueUserWorkItem (o => GetData ("best"));

			recyclerView = FindViewById<RecyclerView> (Resource.Id.image_recycler);
			GridLayoutManager glm = new GridLayoutManager (this, 3);
			recyclerView.SetLayoutManager (glm);
			mainSlider = FindViewById<LinearLayout> (Resource.Id.main_slider);
			best = mainSlider.FindViewById<TextView> (Resource.Id.bestItem);
			category = mainSlider.FindViewById<TextView> (Resource.Id.categoryItem);
			random = mainSlider.FindViewById<TextView> (Resource.Id.randomItem);
			best.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);

			lovelayout = mainSlider.FindViewById<LinearLayout> (Resource.Id.Lovelayout);
			happineslayout = mainSlider.FindViewById<LinearLayout> (Resource.Id.Happineslayout);
			sportlayout = mainSlider.FindViewById<LinearLayout> (Resource.Id.Sportlayout);
			couplelayout = mainSlider.FindViewById<LinearLayout> (Resource.Id.Couplelayout);
			motivationlayout = mainSlider.FindViewById<LinearLayout> (Resource.Id.Motivationlayout);

			bestBool = true;
			best.Click += delegate {
				if (bestBool) {
					return;
				} else {				
					ThreadPool.QueueUserWorkItem (o => GetData ("best"));
					RunOnUiThread (() => {
						ClickValidator ().SetBackgroundResource (Color.Transparent);
						best.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);
						bestBool = true;
					});
				}
			};
			category.Click += delegate {
				if (categoryBool) {
					return;
				} else {					
					ThreadPool.QueueUserWorkItem (o => GetData ("categories/love"));
					RunOnUiThread (() => {
						ClickValidator ().SetBackgroundResource (Color.Transparent);
						ClickCategoryValidator ();
						mainSlider.FindViewById<LinearLayout> (Resource.Id.category_slider).Visibility = ViewStates.Visible;
						category.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);
						mainSlider.FindViewById<TextView> (Resource.Id.loveText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
						mainSlider.FindViewById<ImageView> (Resource.Id.loveImage).SetImageResource (Resource.Drawable.ic_format_text);
						categoryBool = true;
						loveBool = true;
					});
				}

			};
			random.Click += delegate {
				if (randomBool) {
					return;
				} else {					
					ThreadPool.QueueUserWorkItem (o => GetData ("random"));
					RunOnUiThread (() => {
						ClickValidator ().SetBackgroundResource (Color.Transparent);
						random.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);
						randomBool = true;
					});
				}
			};
		
		
			lovelayout.Click += delegate {
				lovelayout.Clickable = false;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/love"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.loveText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.loveImage).SetImageResource (Resource.Drawable.ic_format_text);
					ClickCategoryValidator ();
					loveBool = true;
				});
			};
			happineslayout.Click += delegate {
				happineslayout.Clickable = false;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/happiness"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.HappinesText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.HappinesImage).SetImageResource (Resource.Drawable.ic_format_text);
					ClickCategoryValidator ();
					happinesBool = true;
				});

			};
			sportlayout.Click += delegate {
				sportlayout.Clickable = false;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/sport"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.SportText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.SportImage).SetImageResource (Resource.Drawable.ic_format_text);
					ClickCategoryValidator ();
					sportBool = true;
				});
			};
			couplelayout.Click += delegate {
				couplelayout.Clickable = false;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/couple"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.CoupleText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.CoupleImage).SetImageResource (Resource.Drawable.ic_format_text);
					ClickCategoryValidator ();
					coupleBool = true;
				});
			};
			motivationlayout.Click += delegate {
				motivationlayout.Clickable = false;
				RunOnUiThread (() => {
					ThreadPool.QueueUserWorkItem (o => GetData ("categories/motivation"));
					mainSlider.FindViewById<TextView> (Resource.Id.MotivationText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.MotivationImage).SetImageResource (Resource.Drawable.ic_format_text);
					ClickCategoryValidator ();
					motivationBool = true;
				});
			};

		}

		private void ClickCategoryValidator ()
		{
			if (loveBool) {
				loveBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.loveText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.loveImage).SetImageResource (Resource.Drawable.ic_palette);
				lovelayout.Clickable = true;
			} else if (happinesBool) {
				happinesBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.HappinesText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.HappinesImage).SetImageResource (Resource.Drawable.ic_palette);
				happineslayout.Clickable = true;
			} else if (sportBool) {
				sportBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.SportText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.SportImage).SetImageResource (Resource.Drawable.ic_palette);
				sportlayout.Clickable = true;
			} else if (coupleBool) {
				coupleBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.CoupleText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.CoupleImage).SetImageResource (Resource.Drawable.ic_palette);
				couplelayout.Clickable = true;
			} else if (motivationBool) {
				motivationBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.MotivationText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.MotivationImage).SetImageResource (Resource.Drawable.ic_palette);
				motivationlayout.Clickable = true;
			}
			//return item;
		}

		private TextView ClickValidator ()
		{
			
			if (bestBool) {
				bestBool = false;
				item = best;
			} else if (categoryBool) {
				categoryBool = false;
				item = category;
				ClickCategoryValidator ();
				mainSlider.FindViewById<LinearLayout> (Resource.Id.category_slider).Visibility = ViewStates.Gone;
			} else if (randomBool) {
				randomBool = false;
				item = random;
			}
			return item;
		}

		private void GetData (string type)
		{

			var reqUrl = string.Format ("{0}{1}/&max_results=500", requestURL, type);
			var request = (HttpWebRequest)WebRequest.Create (reqUrl);
			request.Timeout = 10000;
			request.Method = "GET";
			request.ContentType = "application/json";
			request.Credentials = CredentialCache.DefaultCredentials;
			var encoded = System.Convert.ToBase64String (System.Text.Encoding.GetEncoding ("ISO-8859-1").GetBytes (ApiKey + ":" + ApiSecret));

			request.Headers.Add ("Authorization", "Basic " + encoded);
			try {
				var response = (HttpWebResponse)request.GetResponse ();
				var reader = new StreamReader (response.GetResponseStream ());
				var streamText = reader.ReadToEnd ();
				var deserializedStreamText = JsonConvert.DeserializeObject<Images> (streamText);
				images = deserializedStreamText.resources;
				
			} catch (Exception ex) {
				var a = 0;
			}

			bitmaps = new List<Bitmap> ();
			foreach (var img in images) {				
				bitmaps.Add (GetImageBitmapFromUrl (img.url));
			}

			RunOnUiThread (() => {
				FindViewById<RelativeLayout> (Resource.Id.homepage).Visibility = ViewStates.Gone;
				FindViewById<LinearLayout> (Resource.Id.selectedpagelayout).Visibility = ViewStates.Visible;
				adapter = new ImageAdapter (this, bitmaps);
				adapter.ItemClick += OnItemClick;
				recyclerView.SetAdapter (adapter);
			});
		}

		void OnItemClick (object sender, int position)
		{			
			var intent = new Intent (this, typeof(PictureActivity));
			intent.PutExtra ("image-number", images [position].url);
			StartActivity (intent);
		}
	}
}

