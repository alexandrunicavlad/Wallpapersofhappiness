package md5d4a16fa48a49fd45dafa93b777be6d40;


public class SelectedPhotoActivity
	extends md5d4a16fa48a49fd45dafa93b777be6d40.BaseActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onWindowFocusChanged:(Z)V:GetOnWindowFocusChanged_ZHandler\n" +
			"";
		mono.android.Runtime.register ("Wallpapersofhappiness.SelectedPhotoActivity, Wallpapersofhappiness, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SelectedPhotoActivity.class, __md_methods);
	}


	public SelectedPhotoActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SelectedPhotoActivity.class)
			mono.android.TypeManager.Activate ("Wallpapersofhappiness.SelectedPhotoActivity, Wallpapersofhappiness, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onWindowFocusChanged (boolean p0)
	{
		n_onWindowFocusChanged (p0);
	}

	private native void n_onWindowFocusChanged (boolean p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
