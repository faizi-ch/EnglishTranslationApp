using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Widget;

namespace EnglishTranslationApp
{
    [Activity(Theme = "@android:style/Theme.Material", Label = "Translation", MainLauncher = true)]
    public class TranslationActivity : Activity
    {
        private TextView ocrdTextView, translatedTextView, ocrdLabelTextView;
        private Button button3;
        ImageButton ocrdCopyButton, translatedCopyButton;

        private List<KeyValuePair<string, string>> languagesList;
        private string selectedLanguage = "";

        private bool firstTime = true;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Translation);

            ocrdTextView = FindViewById<TextView>(Resource.Id.ocrdTextView);
            translatedTextView = FindViewById<TextView>(Resource.Id.translatedTextView);
            ocrdLabelTextView = FindViewById<TextView>(Resource.Id.ocrdLabelTextView);

            ocrdTextView.MovementMethod=new ScrollingMovementMethod();
            translatedTextView.MovementMethod=new ScrollingMovementMethod();

            button3 = FindViewById<Button>(Resource.Id.button3);
            ocrdCopyButton = FindViewById<ImageButton>(Resource.Id.ocrdCopyButton);
            translatedCopyButton = FindViewById<ImageButton>(Resource.Id.translatedCopyButton);

            

            string text = Intent.GetStringExtra("OCRdText") ?? "Data not available";

            ocrdTextView.Text = text;

            button3.Click += Button3OnClick;
            ocrdCopyButton.Click += OcrdCopyButtonOnClick;
            translatedCopyButton.Click += TranslatedCopyButtonOnClick;

            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);

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

        private void Button3OnClick(object sender, EventArgs eventArgs)
        {
            string text = "";

            if (ocrdTextView.Text != "")
            {
                text = ocrdTextView.Text;
                

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

        private void OcrdCopyButtonOnClick(object sender, EventArgs eventArgs)
        {
            ClipboardManager clipboard = (ClipboardManager)GetSystemService(ClipboardService);
            ClipData clip = ClipData.NewPlainText("Translated Text", ocrdTextView.Text);
            clipboard.PrimaryClip = clip;
            Toast.MakeText(this, "Text is copied to clipboard", ToastLength.Long).Show();
        }

        private void TranslatedCopyButtonOnClick(object sender, EventArgs eventArgs)
        {
            ClipboardManager clipboard = (ClipboardManager)GetSystemService(ClipboardService);
            ClipData clip = ClipData.NewPlainText("Translated Text", translatedTextView.Text);
            clipboard.PrimaryClip = clip;
            Toast.MakeText(this, "Text is copied to clipboard", ToastLength.Long).Show();
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            /*string toast = string.Format("Language code for language {0} is {1}",
                spinner.GetItemAtPosition(e.Position), languagesList[e.Position].Value);
            Toast.MakeText(this, toast, ToastLength.Long).Show();*/

            selectedLanguage = languagesList[e.Position].Value;

            ocrdLabelTextView.Text = languagesList[e.Position].Key;

            if(!firstTime)
            Button3OnClick(button3, new System.EventArgs());

            firstTime = false;
            /*var progressDialog = ProgressDialog.Show(this, "Please wait...", "Checking account info...", true);
            new Thread(new ThreadStart(delegate
            {                 //LOAD METHOD TO GET ACCOUNT INFO    
                
                RunOnUiThread(() => Toast.MakeText(this, "Toast within progress dialog.", ToastLength.Long).Show()); 
                //HIDE PROGRESS DIALOG                 
                RunOnUiThread(() => progressDialog.Hide());
            })).Start();*/


        }


        /*public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.languages_menu, menu);

            / *IMenuItem item = menu.FindItem(Resource.Id.spinner1);
            Spinner spinner = (Spinner) MenuItemCompat.GetActionView(item);

            //Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);

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
            foreach (var it in languagesList)
                languages.Add(it.Key);



            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, languages);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
            spinner.Adapter = adapter;* /

            return true;


        }*/

    }
}