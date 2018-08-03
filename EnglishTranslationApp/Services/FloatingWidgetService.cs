using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace EnglishTranslationApp.Services
{
    [Service]
    public class FloatingWidgetService : Service, View.IOnTouchListener
    {
        private IWindowManager _windowManager;
        private WindowManagerLayoutParams _layoutParams;
        private View _floatingView;
        private View _expandedContainer;

        private int _initialX;
        private int _initialY;
        private float _initialTouchX;
        private float _initialTouchY;

        private TextView translatedTextView, ocrdLabelTextView;
        ImageButton translatedCopyButton;
        private Spinner spinner;

        private List<KeyValuePair<string, string>> languagesList;
        private string selectedLanguage = "";

        private string copiedText = "";

        public override void OnCreate()
        {
            base.OnCreate();

            _floatingView = LayoutInflater.From(this).Inflate(Resource.Layout.layout_floating_widget, null);
            SetTouchListener();

            _layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent,
                WindowManagerTypes.Phone,
                WindowManagerFlags.NotFocusable,
                Format.Translucent)
            {
                Gravity = GravityFlags.Left | GravityFlags.Top
            };

            _windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            _windowManager.AddView(_floatingView, _layoutParams);
            _expandedContainer = _floatingView.FindViewById(Resource.Id.flyout);


            translatedTextView = _floatingView.FindViewById<TextView>(Resource.Id.translatedTextView);
            ocrdLabelTextView = _floatingView.FindViewById<TextView>(Resource.Id.ocrdLabelTextView);

            translatedTextView.MovementMethod = new ScrollingMovementMethod();

            translatedCopyButton = _floatingView.FindViewById<ImageButton>(Resource.Id.translatedCopyButton);

            translatedCopyButton.Click += TranslatedCopyButtonOnClick;

            spinner = _floatingView.FindViewById<Spinner>(Resource.Id.spinner);

            



            languagesList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("English", "en"),
                new KeyValuePair<string, string>("Arabic", "ar"),
                new KeyValuePair<string, string>("Greek", "el"),
                new KeyValuePair<string, string>("Hebrew", "he"),
                new KeyValuePair<string, string>("Spanish", "es"),
                new KeyValuePair<string, string>("Chinese", "zh"),
                new KeyValuePair<string, string>("Korean", "ko"),
                new KeyValuePair<string, string>("German", "de"),
                new KeyValuePair<string, string>("Punjabi", "pa"),
                new KeyValuePair<string, string>("Persian", "fa"),
                new KeyValuePair<string, string>("Russian", "ru"),
                new KeyValuePair<string, string>("Turkish", "tr"),
                new KeyValuePair<string, string>("Urdu", "ur"),
                new KeyValuePair<string, string>("French", "fr"),
                new KeyValuePair<string, string>("Hindi", "hi"),
                new KeyValuePair<string, string>("Japanese", "ja")
            };

            List<string> languages = new List<string>();
            foreach (var item in languagesList)
                languages.Add(item.Key);



            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, languages);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
            spinner.Adapter = adapter;
        }

        private void SetTouchListener()
        {
            var rootContainer = _floatingView.FindViewById<RelativeLayout>(Resource.Id.root);
            rootContainer.SetOnTouchListener(this);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (_floatingView != null)
            {
                _windowManager.RemoveView(_floatingView);
            }
        }

        public bool OnTouch(View view, MotionEvent motion)
        {
            ClipboardManager clipBoard = (ClipboardManager)Android.App.Application.Context.GetSystemService(ClipboardService);
            ClipData.Item item = clipBoard.PrimaryClip.GetItemAt(0);
            copiedText = item.Text;

            /*Intent translationActivity = new Intent(this, typeof(TranslationActivity));
            translationActivity.PutExtra("OCRdText", copiedText);

            var t = new Thread(() =>
            {
                StartActivity(translationActivity);
                StopSelf();
            }
        );
            t.Start();

            
            return true;*/

            switch (motion.Action)
            {
                case MotionEventActions.Down:
                    //initial position
                    _initialX = _layoutParams.X;
                    _initialY = _layoutParams.Y;

                    //touch point
                    _initialTouchX = motion.RawX;
                    _initialTouchY = motion.RawY;
                    return true;

                case MotionEventActions.Up:
                    int offsetX = (int) motion.RawX - (int) _initialTouchX;
                    int offsetY = (int) motion.RawY - (int) _initialTouchY;

                    if (offsetX < 10 && offsetY < 10)
                    {
                         
                        if(_expandedContainer.Visibility == ViewStates.Gone)
                        {
                            _expandedContainer.Visibility = ViewStates.Visible;

                              
                        }
                        else
                        {
                            _expandedContainer.Visibility = ViewStates.Gone;
                            StopSelf();
                        }
                        
                    }

                    return true;

                case MotionEventActions.Move:
                    //Calculate the X and Y coordinates of the view.
                    _layoutParams.X = _initialX + (int) (motion.RawX - _initialTouchX);
                    _layoutParams.Y = _initialY + (int) (motion.RawY - _initialTouchY);

                    //Update the layout with new X & Y coordinate
                    _windowManager.UpdateViewLayout(_floatingView, _layoutParams);
                    return true;
            }

            return false;
        }

        private void TranslatedCopyButtonOnClick(object sender, EventArgs eventArgs)
        {
            ClipboardManager clipboard = (ClipboardManager) GetSystemService(ClipboardService);
            ClipData clip = ClipData.NewPlainText("Translated Text", translatedTextView.Text);
            clipboard.PrimaryClip = clip;
            Toast.MakeText(this, "Text is copied to clipboard", ToastLength.Long).Show();
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner) sender;
            selectedLanguage = languagesList[e.Position].Value;

            ocrdLabelTextView.Text = languagesList[e.Position].Key;

            string text = "";

            if (copiedText != "")
            {
                text = copiedText;

                WebRequest request =
                WebRequest.Create(
                    "https://translate.yandex.net/api/v1.5/tr/translate?key=trnsl.1.1.20180102T204517Z.04fa0d17c710294a.a073b91196a242b91ee168e0bf70a0b472aec91a;lang=" +
                    selectedLanguage + "&text=" + text + "&format=plain");
                WebResponse response = request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    var fetchedXml = sr.ReadToEnd();
                    XmlDocument d = new XmlDocument();

                    d.LoadXml(fetchedXml);
                    XmlNodeList textNodes = d.GetElementsByTagName("text");
                    foreach (XmlNode i in textNodes)
                    {
                        if (i.InnerText.Length > 0)
                        {
                            translatedTextView.Text = i.InnerText;
                        }
                    }
                }
            }

        }

    }
}