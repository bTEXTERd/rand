using Android.App;
using Android.OS;
using Android.Views;

namespace TestAndroid
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        }
    }
}

