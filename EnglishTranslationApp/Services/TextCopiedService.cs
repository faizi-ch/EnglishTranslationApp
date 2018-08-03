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
using EnglishTranslationApp.Services;
using Java.Interop;

namespace EnglishTranslationApp
{
    [Service]
    class TextCopiedService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();

            ClipboardManager clipBoard = (ClipboardManager)GetSystemService(ClipboardService);
            clipBoard.AddPrimaryClipChangedListener(new OnPrimaryClipChangedListener());
        }

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }

        class OnPrimaryClipChangedListener : Java.Lang.Object, ClipboardManager.IOnPrimaryClipChangedListener
        {

            /// <summary>
            /// Callback that gets invoked when the primary clip of the device changes.
            /// </summary>
            public void OnPrimaryClipChanged()
            {

                ClipboardManager clipBoard = (ClipboardManager)Android.App.Application.Context.GetSystemService(ClipboardService);
                String pasteData = "";
                ClipData.Item item = clipBoard.PrimaryClip.GetItemAt(0);
                pasteData = item.Text;

                //Toast.MakeText(Application.Context, "Copied value: " + pasteData, ToastLength.Long).Show();

                /*Intent translationActivity = new Intent(Application.Context, typeof(TranslationActivity));
                translationActivity.PutExtra("OCRdText", pasteData);
                Application.Context.StartActivity(translationActivity);*/


                
                Application.Context.StartService(new Intent(Application.Context, typeof(FloatingWidgetService)));

                //Intent translationActivity = new Intent(Application.Context, typeof(MainActivity2));
                
                //Application.Context.StartActivity(translationActivity);
            }
        }
    }
    

}