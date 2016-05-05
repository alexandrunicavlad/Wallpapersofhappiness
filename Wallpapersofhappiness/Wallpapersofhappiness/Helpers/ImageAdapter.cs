using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Graphics.Drawables;
using Java.Net;
using System.Collections.Generic;
using Java.IO;
using Android.Graphics;
using System.Net;
using Android.Support.V7.Widget;

namespace Wallpapersofhappiness
{
	public class ImageAdapter : RecyclerView.Adapter
	{
		Context context;
		List<Bitmap> images;

		public event EventHandler<int> ItemClick;

		public ImageAdapter (Context c, List<Bitmap> imgs)
		{
			context = c;
			images = imgs;
		}

		public ImageAdapter ()
		{			
		}

		public override int ItemCount {
			get {
				return images.Count;
			}
		}

		public override void 
		OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			PhotoViewHolder vh = holder as PhotoViewHolder;
			var item = images [position];
			vh.Image.SetImageBitmap (item);
			item = null;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (Android.Views.ViewGroup parent, int viewType)
		{	
			View itemView = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.picture_layout, parent, false);
			PhotoViewHolder vh = new PhotoViewHolder (itemView, OnClick); 
			return vh;
		}

		void OnClick (int position)
		{
			if (ItemClick != null)
				ItemClick (this, position);
		}

	}

	public class PhotoViewHolder : RecyclerView.ViewHolder
	{
		public ImageView Image { get; private set; }

		public int Position { get; set; }

		public PhotoViewHolder (View itemView, Action<int> listener) : base (itemView)
		{			
			Image = itemView.FindViewById<ImageView> (Resource.Id.imageView);

			itemView.Click += (sender, e) => listener (base.Position);
		}


	}
}

