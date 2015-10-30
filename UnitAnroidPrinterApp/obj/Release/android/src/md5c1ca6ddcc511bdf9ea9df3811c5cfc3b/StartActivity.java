package md5c1ca6ddcc511bdf9ea9df3811c5cfc3b;


public class StartActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("UnitAnroidPrinterApp.StartActivity, UnitAnroidPrinterApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", StartActivity.class, __md_methods);
	}


	public StartActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == StartActivity.class)
			mono.android.TypeManager.Activate ("UnitAnroidPrinterApp.StartActivity, UnitAnroidPrinterApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onBackPressed ()
	{
		n_onBackPressed ();
	}

	private native void n_onBackPressed ();


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
