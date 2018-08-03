using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
using Android.Views;
using EnglishTranslationApp.Adapters;
using EnglishTranslationApp.Models;
using Android.Speech.Tts;


namespace EnglishTranslationApp
{
    [Activity(Theme = "@android:style/Theme.Material.Light", Label = "Speech", MainLauncher = true)]
    public class SpeechActivity : Activity, IRecognitionListener/*, TextToSpeech.IOnInitListener*/
    {
        public const string Tag = "VoiceRec";

        SpeechRecognizer Recognizer { get; set; }
        Intent SpeechIntent { get; set; }
        TextView Label { get; set; }

        private SlidingUpPanelLayout sliding_layout;

        private List<KeyValuePair<string, string>> languagesList;

        private string rightSelectedLanguage = "English";
        private string rightSelectedLanguageCode = "en-US";
        private string rightSelectedLanguageTransCode = "en";
        private string leftSelectedLanguage = "English";
        private string leftSelectedLanguageCode = "en-US";
        private string leftSelectedLanguageTransCode = "en";

        private Spinner spinner;

        public ListView list_of_message;
        public Button rightSpeakButton,leftSpeakButton, rightButton,leftButton;
        public List<ChatModel> list_chat = new List<ChatModel>();
        public List<ChatModel> list_translated_chat = new List<ChatModel>();
        private bool isRight = false, isLeft = false;
        private bool isRightLanguageButton=false, isLeftLanguageButton = false;

/*
        TextToSpeech textToSpeech;
        Context context;
        private readonly int MyCheckCode = 101, NeedLang = 103;
        Java.Util.Locale lang;*/

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Speech);

            Recognizer = SpeechRecognizer.CreateSpeechRecognizer(this);
            Recognizer.SetRecognitionListener(this);

            SpeechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, PackageName);
            

            sliding_layout = FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_layout);

            list_of_message = FindViewById<ListView>(Resource.Id.list_of_message);

            
            
            //var button = FindViewById<Button>(Resource.Id.speakButton);
            //button.Click += ButtonClick;

            leftSpeakButton = FindViewById<Button>(Resource.Id.leftSpeakButton);
            leftSpeakButton.Click += LeftSpeakButtonClick;
            rightSpeakButton = FindViewById<Button>(Resource.Id.rightSpeakButton);
            rightSpeakButton.Click += RightSpeakButtonClick;


            leftButton = FindViewById<Button>(Resource.Id.leftButton);
            leftButton.Click += LeftButtonClick;
            rightButton = FindViewById<Button>(Resource.Id.rightButton);
            rightButton.Click += RightButtonClick;

            /*context = rightSpeakButton.Context;
            textToSpeech = new TextToSpeech(context, this, "com.google.android.tts");*/
            /*var langAvailable = new List<string> { "Default" };

            // our spinner only wants to contain the languages supported by the tts and ignore the rest
            var localesAvailable = Java.Util.Locale.GetAvailableLocales().ToList();
            foreach (var locale in localesAvailable)
            {
                LanguageAvailableResult res = textToSpeech.IsLanguageAvailable(locale);
                switch (res)
                {
                    case LanguageAvailableResult.Available:
                        langAvailable.Add(locale.DisplayLanguage);
                        break;
                    case LanguageAvailableResult.CountryAvailable:
                        langAvailable.Add(locale.DisplayLanguage);
                        break;
                    case LanguageAvailableResult.CountryVarAvailable:
                        langAvailable.Add(locale.DisplayLanguage);
                        break;
                }

            }
            langAvailable = langAvailable.OrderBy(t => t).Distinct().ToList();*/

            // set up the speech to use the default langauge
            // if a language is not available, then the default language is used.
            /*lang = Java.Util.Locale.Us;
            textToSpeech.SetLanguage(lang);

            // set the speed and pitch
            textToSpeech.SetPitch(.5f);
            textToSpeech.SetSpeechRate(.5f);*/

            //Label = FindViewById<TextView>(Resource.Id.textV);

            spinner = FindViewById<Spinner>(Resource.Id.spinnerSpeech);

            languagesList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("English", "en-US"),
                new KeyValuePair<string, string>("Arabic", "ar-SA"),
                new KeyValuePair<string, string>("Greek", "el-GR"),
                new KeyValuePair<string, string>("Hebrew", "he-IL"),
                new KeyValuePair<string, string>("Spanish", "es-ES"),
                new KeyValuePair<string, string>("Chinese", "zh-CN"),
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
            spinner.Adapter = adapter;

            //lang = Java.Util.Locale.GetAvailableLocales().FirstOrDefault(t => t.DisplayLanguage == langAvailable[(int)e.Id]);
            // create intent to check the TTS has this language installed
            /*var checkTTSIntent = new Intent();
            checkTTSIntent.SetAction(TextToSpeech.Engine.ActionCheckTtsData);
            StartActivityForResult(checkTTSIntent, NeedLang);*/
        }

        private void LeftSpeakButtonClick(object sender, EventArgs e)
        {
            isLeft = true;
            isRight = false;

            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguage, leftSelectedLanguageCode);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference, leftSelectedLanguageCode);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraOnlyReturnLanguagePreference, leftSelectedLanguageCode);

            Recognizer.StartListening(SpeechIntent);
        }

        private void RightSpeakButtonClick(object sender, EventArgs e)
        {
            isRight = true;
            isLeft = false;

            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguage, rightSelectedLanguageCode);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference, rightSelectedLanguageCode);
            SpeechIntent.PutExtra(RecognizerIntent.ExtraOnlyReturnLanguagePreference, rightSelectedLanguageCode);

            Recognizer.StartListening(SpeechIntent);
        }


        private void RightButtonClick(object sender, EventArgs e)
        {
            isRightLanguageButton = true;
            isLeftLanguageButton = false;
            if (sliding_layout.GetPanelState() == SlidingUpPanelLayout.PanelState.Collapsed)
            {
                
                KeyValuePair<string, string> selectedLanguage =
                    new KeyValuePair<string, string>(rightSelectedLanguage, rightSelectedLanguageCode);
                spinner.SetSelection(languagesList.IndexOf(selectedLanguage));
                sliding_layout.SetPanelState(SlidingUpPanelLayout.PanelState.Expanded);


            }
            else if (sliding_layout.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
            {
                sliding_layout.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
            }




        }

        private void LeftButtonClick(object sender, EventArgs e)
            {
            isLeftLanguageButton = true;
            isRightLanguageButton = false;
            if (sliding_layout.GetPanelState() == SlidingUpPanelLayout.PanelState.Collapsed)
            {
                
                KeyValuePair<string, string> selectedLanguage =
                    new KeyValuePair<string, string>(leftSelectedLanguage, leftSelectedLanguageCode);
                spinner.SetSelection(languagesList.IndexOf(selectedLanguage));
                sliding_layout.SetPanelState(SlidingUpPanelLayout.PanelState.Expanded);


            }
            if (sliding_layout.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
            {
                sliding_layout.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
            }

        }

            private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
            {
            //Spinner spinner = (Spinner) sender;
            //ShowToast(isRightLanguageButton.ToString());
            if (isRightLanguageButton)
                {
                //ShowToast(languagesList[e.Position].Key);
                rightSelectedLanguage = languagesList[e.Position].Key;
                    rightSelectedLanguageCode = languagesList[e.Position].Value;

                rightButton.Text = languagesList[e.Position].Key;

                    rightSelectedLanguageTransCode = rightSelectedLanguageCode.Split('-')[0];
                }
                else if (isLeftLanguageButton)
            {
                leftSelectedLanguage = languagesList[e.Position].Key;
                leftSelectedLanguageCode = languagesList[e.Position].Value;

                leftButton.Text = languagesList[e.Position].Key;

                leftSelectedLanguageTransCode = leftSelectedLanguageCode.Split('-')[0];
            }
               
                

                //ocrdLabelTextView.Text = languagesList[e.Position].Key;



            }

        public string Translate(string text, string lang)
        {
            string translatedMsg = "";

            try
            {
                if (text != "")
                {
                    WebRequest request =
                    WebRequest.Create(
                        "https://translate.yandex.net/api/v1.5/tr/translate?key=trnsl.1.1.20180102T204517Z.04fa0d17c710294a.a073b91196a242b91ee168e0bf70a0b472aec91a;lang=" +
                        lang + "&text=" + text + "&format=plain");
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
                                translatedMsg = i.InnerText;
                                //ShowToast(translatedMsg);
                            }
                        }
                    }
                }
                return translatedMsg;
            }
            catch (Exception e)
            {
                ShowToast(e.Message + "\n" + e.ToString());
                return translatedMsg;
            }

            
        }

        public async void OnResults(Bundle results)
        {
            var matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);

            if (matches != null && matches.Count > 0)
            {
                if (isRight)
                {
                    ChatModel textModel = new ChatModel();
                    textModel.ChatMessage = matches[0];
                    textModel.IsSend = true;
                    list_chat.Add(textModel);
                    ChatModel translatedTextModel = new ChatModel();
                    string s = Translate(matches[0], leftSelectedLanguageTransCode);

                    translatedTextModel.ChatMessage = s;
                    translatedTextModel.IsSend = true;
                    list_translated_chat.Add(translatedTextModel);
                    ChatAdapter adapter = new ChatAdapter(this.list_chat, this.list_translated_chat, this.BaseContext);
                    this.list_of_message.Adapter = adapter;
                    try
                    {
                        //await CrossTextToSpeech.Current.Speak(s);
                        //TextToSpeech textToSpeech = new TextToSpeech(this, new Plugin.TextToSpeech.)
                        //Speak(s);
                        /*if (!string.IsNullOrEmpty(s))
                            textToSpeech.Speak(s, QueueMode.Flush, null);*/
                    }
                    catch (Exception e)
                    {
                        ShowToast(e.Message + "\n" + e.ToString());
                    }
                    
                }
                else if (isLeft)
                {
                    ChatModel textModel = new ChatModel();
                    textModel.ChatMessage = matches[0];
                    textModel.IsSend = false;
                    list_chat.Add(textModel);
                    ChatModel translatedTextModel = new ChatModel();
                    string s = Translate(matches[0], rightSelectedLanguageTransCode);

                    translatedTextModel.ChatMessage = s;
                    translatedTextModel.IsSend = false;
                    list_translated_chat.Add(translatedTextModel);
                    ChatAdapter adapter = new ChatAdapter(this.list_chat, this.list_translated_chat, this.BaseContext);
                    this.list_of_message.Adapter = adapter;
                    try
                    {
                        /*if (!string.IsNullOrEmpty(s))
                            textToSpeech.Speak(s, QueueMode.Flush, null);*/
                        //tts.Speak(s, QueueMode.Add, null);
                        //await CrossTextToSpeech.Current.Speak(s);
                        //Speak(s);
                    }
                    catch (Exception e)
                    {
                        ShowToast(e.Message + "\n" + e.ToString());
                    }
                    
                }

                //Label.Text = matches[0];

            }
        }
/*
        void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        {
            // if we get an error, default to the default language
            if (status == OperationResult.Error)
                textToSpeech.SetLanguage(Java.Util.Locale.Us);
            // if the listener is ok, set the lang
            if (status == OperationResult.Success)
                textToSpeech.SetLanguage(lang);
        }
        protected override void OnActivityResult(int req, Result res, Intent data)
        {
            if (req == NeedLang)
            {
                // we need a new language installed
                var installTTS = new Intent();
                installTTS.SetAction(TextToSpeech.Engine.ActionInstallTtsData);
                StartActivity(installTTS);
            }
        }*/
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

        public void ShowToast(string text, bool IsLengthShort = false)
        {
            Handler mainHandler = new Handler(Looper.MainLooper);
            Java.Lang.Runnable runnableToast = new Java.Lang.Runnable(() =>
            {
                var duration = IsLengthShort ? ToastLength.Short : ToastLength.Long;
                Toast.MakeText(this, text, duration).Show();
            });

            mainHandler.Post(runnableToast);
        }

        
    }
}