
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
using System.Reflection;

namespace Wallpapersofhappiness
{
	[Activity (MainLauncher = true, Label = "@string/app_name", Theme = "@style/AppTheme", ConfigurationChanges =
		(Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize))]		


	public class SelectedPhotoActivity : BaseActivity
	{
		private const int REQUEST_IMAGE_CAPTURE = 1;
		private const int REQUEST_IMAGE_ALBUM = 2;
		private string requestURL = "https://api.cloudinary.com/v1_1/wp-of-happiness/resources/image/upload/?prefix=";
		private const string ApiKey = "966956932715847";
		private const string ApiSecret = "grc0mV1_k8xuV8xLYZgPGMpbwDw";
		private static readonly string DatabaseDirectory =
			System.IO.Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "../photo");

		private LinearLayout mainSlider;
		private List<ImageModel> images;
		private List<ImageModel> images1;
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
		private bool retrying = false;
		private TextView best;
		private TextView category;
		private TextView random;
		private TextView item;

		private LinearLayout lovelayout;
		private LinearLayout happineslayout;
		private LinearLayout sportlayout;
		private LinearLayout couplelayout;
		private LinearLayout motivationlayout;

		private RelativeLayout loading;
		private RelativeLayout homePage;
		private RelativeLayout loadingRec;
		private MemoryLimitedLruCache _memoryCache;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.select_photo_drawer);
			ConstructActionBar ();
			UpdateTexts ();
			SetTitle (GetString (Resource.String.Defaultbackground));

			loading = FindViewById<RelativeLayout> (Resource.Id.main_loading);
			loadingRec = FindViewById<RelativeLayout> (Resource.Id.main_loading_recycler);
			homePage = FindViewById<RelativeLayout> (Resource.Id.homepage);
			homePage.Visibility = ViewStates.Visible;



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
			var maxMemory = (int)(Java.Lang.Runtime.GetRuntime ().MaxMemory () / 1024);
			var cacheSize = maxMemory / 8;
			_memoryCache = new MemoryLimitedLruCache (cacheSize);
			bestBool = true;
			best.Click += delegate {
				if (bestBool) {
					return;
				} else {				
					recyclerView.Visibility = ViewStates.Gone;
					loadingRec.Visibility = ViewStates.Visible;
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
					recyclerView.Visibility = ViewStates.Gone;
					loadingRec.Visibility = ViewStates.Visible;
					ThreadPool.QueueUserWorkItem (o => GetData ("categories/love"));
					RunOnUiThread (() => {
						ClickValidator ().SetBackgroundResource (Color.Transparent);
						ClickCategoryValidator ();
						mainSlider.FindViewById<LinearLayout> (Resource.Id.category_slider).Visibility = ViewStates.Visible;
						category.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);
						mainSlider.FindViewById<TextView> (Resource.Id.loveText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
						mainSlider.FindViewById<ImageView> (Resource.Id.loveImage).SetImageResource (Resource.Drawable.ic_love_green);
						categoryBool = true;
						loveBool = true;
					});
				}

			};
			random.Click += delegate {
				if (randomBool) {
					return;
				} else {	
					recyclerView.Visibility = ViewStates.Gone;
					loadingRec.Visibility = ViewStates.Visible;
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
				recyclerView.Visibility = ViewStates.Gone;
				loadingRec.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/love"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.loveText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.loveImage).SetImageResource (Resource.Drawable.ic_love_green);
					ClickCategoryValidator ();
					loveBool = true;
				});
			};
			happineslayout.Click += delegate {
				happineslayout.Clickable = false;
				recyclerView.Visibility = ViewStates.Gone;
				loadingRec.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/happiness"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.HappinesText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.HappinesImage).SetImageResource (Resource.Drawable.ic_happiness_green);
					ClickCategoryValidator ();
					happinesBool = true;
				});

			};
			sportlayout.Click += delegate {
				sportlayout.Clickable = false;
				recyclerView.Visibility = ViewStates.Gone;
				loadingRec.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/sport"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.SportText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.SportImage).SetImageResource (Resource.Drawable.ic_sport_green);
					ClickCategoryValidator ();
					sportBool = true;
				});
			};
			couplelayout.Click += delegate {
				couplelayout.Clickable = false;
				recyclerView.Visibility = ViewStates.Gone;
				loadingRec.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/couple"));
				RunOnUiThread (() => {				

					mainSlider.FindViewById<TextView> (Resource.Id.CoupleText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.CoupleImage).SetImageResource (Resource.Drawable.ic_couple_green);
					ClickCategoryValidator ();
					coupleBool = true;
				});
			};
			motivationlayout.Click += delegate {
				motivationlayout.Clickable = false;
				recyclerView.Visibility = ViewStates.Gone;
				loadingRec.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/motivation"));
				RunOnUiThread (() => {	
					mainSlider.FindViewById<TextView> (Resource.Id.MotivationText).SetTextColor (Resources.GetColor (Resource.Color.green_main));
					mainSlider.FindViewById<ImageView> (Resource.Id.MotivationImage).SetImageResource (Resource.Drawable.ic_motivation_green);
					ClickCategoryValidator ();
					motivationBool = true;
				});
			};

		}

		private Dictionary<string, List<ImageModel>>  ConstructImageFromApp ()
		{	
			var listOfImages = new Dictionary<string, List<ImageModel>> ();
			var bestList = new List<ImageModel> ();
			var loveList = new List<ImageModel> ();
			var happinessList = new List<ImageModel> ();
			var sportList = new List<ImageModel> ();
			var coupleList = new List<ImageModel> ();
			var motivationList = new List<ImageModel> ();
			bestList.Add (new ImageModel () {
				version = Resource.Drawable.anime_devushki_okaloidy, type = "local"
			});
			bestList.Add (new ImageModel () {
				version = Resource.Drawable.cvety_rasteniya, type = "local"
			});
			loveList.Add (new ImageModel () {
				version = Resource.Drawable.cvety_rasteniya, type = "local"
			});
			loveList.Add (new ImageModel () {
				version = Resource.Drawable.hty_more_pejzazh, type = "local"
			});
			happinessList.Add (new ImageModel () {
				version = Resource.Drawable.anime_devushki_okaloidy, type = "local"
			});
			happinessList.Add (new ImageModel () {
				version = Resource.Drawable.hty_more_pejzazh, type = "local"
			});
			sportList.Add (new ImageModel () {
				version = Resource.Drawable.devushki_lyudi_novy, type = "local"
			});
			sportList.Add (new ImageModel () {
				version = Resource.Drawable.hty_more_pejzazh, type = "local"
			});
			coupleList.Add (new ImageModel () {
				version = Resource.Drawable.delfiny_zhivotnye, type = "local"
			});
			coupleList.Add (new ImageModel () {
				version = Resource.Drawable.devushki_lyudi_novy, type = "local"
			});
			motivationList.Add (new ImageModel () {
				version = Resource.Drawable.delfiny_zhivotnye, type = "local"
			});
			listOfImages.Add ("best", bestList);
			listOfImages.Add ("categories/love", loveList);
			listOfImages.Add ("categories/happiness", happinessList);			
			listOfImages.Add ("categories/sport", sportList);
			listOfImages.Add ("categories/couple", coupleList);
			listOfImages.Add ("categories/motivation", motivationList);

			return listOfImages;
		}

		private void ClickCategoryValidator ()
		{
			if (loveBool) {
				loveBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.loveText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.loveImage).SetImageResource (Resource.Drawable.ic_love);
				lovelayout.Clickable = true;
			} else if (happinesBool) {
				happinesBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.HappinesText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.HappinesImage).SetImageResource (Resource.Drawable.ic_happiness);
				happineslayout.Clickable = true;
			} else if (sportBool) {
				sportBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.SportText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.SportImage).SetImageResource (Resource.Drawable.ic_sport);
				sportlayout.Clickable = true;
			} else if (coupleBool) {
				coupleBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.CoupleText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.CoupleImage).SetImageResource (Resource.Drawable.ic_couple);
				couplelayout.Clickable = true;
			} else if (motivationBool) {
				motivationBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.MotivationText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.MotivationImage).SetImageResource (Resource.Drawable.ic_motivation);
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
			
			images = new List<ImageModel> ();

			switch (type) {
			case "best":
				{	
					images = ConstructImageFromApp () [type];
					break;
				}
			case "categories/love":
				{
					images = ConstructImageFromApp () [type];
					break;
				}
			case "categories/happiness":
				{
					images = ConstructImageFromApp () [type];
					break;
				}
			case "categories/sport":
				{
					images = ConstructImageFromApp () [type];
					break;
				}
			case "categories/couple":
				{
					images = ConstructImageFromApp () [type];
					break;
				}
			case "categories/motivation":
				{
					images = ConstructImageFromApp () [type];
					break;
				}
			}

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
				images.AddRange (deserializedStreamText.resources);

			} catch (Exception ex) {
				HandleErrors (ex);
				retrying = true;
			}
			RunOnUiThread (() => {
				homePage.Visibility = ViewStates.Gone;
				if (retrying) {
//					ShowRetry (loading, this);
//					loading.Visibility = ViewStates.Visible;
//					return;
				}

				bitmaps = new List<Bitmap> ();
				foreach (var img in images) {		
					if (!img.type.Equals ("local")) {
					
						if (_memoryCache.Get (img.url) == null) {
							var bitm = GetImageBitmapFromUrl (img.url);
							_memoryCache.Put (img.url, bitm);
							bitmaps.Add (bitm);
						} else {
							var bitm = (Bitmap)_memoryCache.Get (img.url);
							bitmaps.Add (bitm);
						}
					} else {	
							
						var bitm = BitmapFactory.DecodeResource (Resources, img.version);						
						bitmaps.Add (GetResizedBitmap (bitm, Resources.DisplayMetrics.WidthPixels / 3, Resources.DisplayMetrics.HeightPixels / 3));
						bitm.Recycle ();
						bitm = null;
					}
				}

				FindViewById<LinearLayout> (Resource.Id.selectedpagelayout).Visibility = ViewStates.Visible;
				adapter = new ImageAdapter (this, bitmaps);
				adapter.ItemClick += OnItemClick;
				recyclerView.SetAdapter (adapter);
				recyclerView.Visibility = ViewStates.Visible;
			});
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

		public Bitmap DecodeSampledBitmapFromResource (Resources res, int resId,	int reqWidth, int reqHeight)
		{

			BitmapFactory.Options options = new BitmapFactory.Options ();
			options.InJustDecodeBounds = true;
			BitmapFactory.DecodeResource (res, resId, options);

			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize (options, reqWidth, reqHeight);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;
			return BitmapFactory.DecodeResource (res, resId, options);
		}

		public int CalculateInSampleSize (BitmapFactory.Options options, int reqWidth, int reqHeight)
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

		void OnItemClick (object sender, int position)
		{			
			var intent = new Intent (this, typeof(DownloadActivity));
			if (images [position].type.Equals ("local")) {
				var urlul = images [position].version;
				intent.PutExtra ("image-path", urlul);
			} else {
				intent.PutExtra ("image-number", images [position].url);
				intent.PutExtra ("image-name", images [position].public_id);
			}
			StartActivity (intent);
		}
	}
}

