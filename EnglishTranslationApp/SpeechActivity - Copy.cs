using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Widget;
using Com.Sothree.Slidinguppanel;
using Android.Support.V4.View;

namespace EnglishTranslationApp
{
    [Activity(Theme = "@android:style/Theme.Material", Label = "Speech", MainLauncher = true)]
    public class SpeechActivity : Activity, IRecognitionListener
    {
        public const string Tag = "VoiceRec";

        SpeechRecognizer Recognizer { get; set; }
        Intent SpeechIntent { get; set; }
        TextView Label { get; set; }

        private SlidingUpPanelLayout sliding_layout;

        private List<KeyValuePair<string, string>> languagesList;

        private string rightSelectedLanguage = "English";
        private string rightSelectedLanguageCode = "en-US";
        private string leftSelectedLanguage = "English";
        private string leftSelectedLanguageCode = "en-US";

        private Spinner spinner;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Speech);

            Recognizer = SpeechRecognizer.CreateSpeechRecognizer(this);
            Recognizer.SetRecognitionListener(this);

            SpeechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, PackageName);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguage, "ur-PK");
            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference, "ur-PK");
            SpeechIntent.PutExtra(RecognizerIntent.ExtraOnlyReturnLanguagePreference, "ur-PK");

            sliding_layout = FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_layout);

            var button = FindViewById<Button>(Resource.Id.speakButton);
            button.Click += ButtonClick;
            //var leftButton = FindViewById<Button>(Resource.Id.leftButton);
            //leftButton.Click += LeftButtonClick;
            //var rightButton = FindViewById<Button>(Resource.Id.rightButton);
            //rightButton.Click += RightButtonClick;


            Label = FindViewById<TextView>(Resource.Id.textV);

            /*spinner = FindViewById<Spinner>(Resource.Id.spinnerSpeech);

            languagesList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("English", "en-US"),
                new KeyValuePair<string, string>("Arabic", "ar-SA"),
                new KeyValuePair<string, string>("Greek", "el-GR"),
                new KeyValuePair<string, string>("Hebrew", "he-IL"),
                new KeyValuePair<string, string>("Spanish", "es-ES"),
                new KeyValuePair<string, string>("Chinese", "cmn-Hans-CN"),
                new KeyValuePair<string, string>("Korean", "ko-KR"),
                new KeyValuePair<string, string>("German", "de-DE"),
                new KeyValuePair<string, string>("Persian", "fa-IR"),
                new KeyValuePair<string, string>("Russian", "ru-RU"),
                new KeyValuePair<string, string>("Turkish", "tr-TR"),
                new KeyValuePair<string, string>("Urdu", "ur-PK"),
                new KeyValuePair<string, string>("French", "fr-FR"),
                new KeyValuePair<string, string>("Hindi", "hi-IN"),
                new KeyValuePair<string, string>("Japanese", "ja-JP")
            };

            List<string> languages = new List<string>();
            foreach (var item in languagesList)
                languages.Add(item.Key);



            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, languages);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
            spinner.Adapter = adapter;*/

        }


        private void RightButtonClick(object sender, EventArgs e)
            {
                if (sliding_layout.GetPanelState() == SlidingUpPanelLayout.PanelState.Hidden)
                {
                    KeyValuePair<string, string> selectedLanguage =
                        new KeyValuePair<string, string>(rightSelectedLanguage, rightSelectedLanguageCode);
                    spinner.SetSelection(languagesList.IndexOf(selectedLanguage));
                    sliding_layout.SetPanelState(SlidingUpPanelLayout.PanelState.Expanded);

                }


            }

            private void LeftButtonClick(object sender, EventArgs e)
            {
                sliding_layout.SetPanelState(SlidingUpPanelLayout.PanelState.Expanded);
            }

            private void ButtonClick(object sender, EventArgs e)
            {
                Recognizer.StartListening(SpeechIntent);
            }

            private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
            {
                Spinner spinner = (Spinner) sender;


                rightSelectedLanguage = languagesList[e.Position].Value;

                //ocrdLabelTextView.Text = languagesList[e.Position].Key;



            }

            public void OnResults(Bundle results)
            {
                var matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
                if (matches != null && matches.Count > 0)
                {
                    Label.Text = matches[0];
                }
            }

            public void OnReadyForSpeech(Bundle @params)
            {
                Log.Debug(Tag, "OnReadyForSpeech");
            }

            public void OnBeginningOfSpeech()
            {
                Log.Debug(Tag, "OnBeginningOfSpeech");
            }

            public void OnEndOfSpeech()
            {
                Log.Debug(Tag, "OnEndOfSpeech");
            }

            public void OnError([GeneratedEnum] SpeechRecognizerError error)
            {
                Log.Debug("OnError", error.ToString());
            }

            public void OnBufferReceived(byte[] buffer)
            {
            }

            public void OnEvent(int eventType, Bundle @params)
            {
            }

            public void OnPartialResults(Bundle partialResults)
            {
            }

            public void OnRmsChanged(float rmsdB)
            {
            }

        
    }
}