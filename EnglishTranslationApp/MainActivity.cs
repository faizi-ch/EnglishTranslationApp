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
using Android.Database;
using Android.Graphics;
using Android.Hardware;
using Android.Hardware.Camera2;
using Android.Media;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Speech.Tts;
using Android.Text;
using Com.Googlecode.Tesseract.Android;
using EnglishTranslationApp.Services;
using Java.Lang;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Tesseract;
using Tesseract.Droid;
using Double = System.Double;
using Stream = System.IO.Stream;
using String = System.String;
using Thread = Java.Lang.Thread;

namespace EnglishTranslationApp
{
    [Activity(Theme = "@android:style/Theme.Material", Label = "English OCR", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Button button;
        private Button button2;
        
        private ImageView imageView;
        private TextView textView;
        //private TextView textView2;
        private EditText editText;

        private CheckBox handwrittenCheckBox;
        String path = null;
        public static readonly int PickImageId = 1000;

        MediaFile photo;
        private static string text = "";
        bool takePhoto = false, chooseFromLib = false;


        public void textToast(string textToDisplay)
        {
            Toast.MakeText(this,
            textToDisplay, ToastLength.Long).Show();
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //StartService(new Intent(this, typeof(FloatingWidgetService)));
            StartService(new Intent(this, typeof(TextCopiedService)));
            

            button = FindViewById<Button>(Resource.Id.button1);
            button2 = FindViewById<Button>(Resource.Id.button2);
            
            textView = FindViewById<TextView>(Resource.Id.textView1);
            //textView2 = FindViewById<TextView>(Resource.Id.textView2);
            imageView = FindViewById<ImageView>(Resource.Id.myImageView);
            editText = FindViewById<EditText>(Resource.Id.editText1);
            handwrittenCheckBox = FindViewById<CheckBox>(Resource.Id.handwrittenCheckBox);

            //editText.InputType = InputTypes.Null;
            //editText.SetTextIsSelectable(true);

            button.Click += ButtonOnClick;
            button2.Click += Button2OnClick;

            /*var btnSayIt = FindViewById<Button>(Resource.Id.speakbtn);
            context = btnSayIt.Context;
            textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");
            lang = Java.Util.Locale.Default;
            textToSpeech.SetLanguage(lang);

            // set the speed and pitch
            textToSpeech.SetPitch(.5f);
            textToSpeech.SetSpeechRate(.5f);
            btnSayIt.Click += delegate
            {
                // if there is nothing to say, don't say it

                textToSpeech.Speak("don't say it", QueueMode.Flush, null);
            };*/
        }
/*
        void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        {
            // if we get an error, default to the default language
            if (status == OperationResult.Error)
                textToSpeech.SetLanguage(Java.Util.Locale.Default);
            // if the listener is ok, set the lang
            if (status == OperationResult.Success)
                textToSpeech.SetLanguage(lang);
        }
        protected override void OnActivityResult(int req, Android.App.Result res, Intent data)
        {
            if (req == NeedLang)
            {
                // we need a new language installed
                var installTTS = new Intent();
                installTTS.SetAction(TextToSpeech.Engine.ActionInstallTtsData);
                StartActivity(installTTS);
            }
        }*/
        async void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            /*Toast.MakeText(this,
            "Clicked!", ToastLength.Long).Show();*/

            

            String[] items = { "Take Photo", "Choose from Library", "Cancel" };

            using (var dialogBuilder = new AlertDialog.Builder(this))
            {
                dialogBuilder.SetTitle("Add Photo");
                dialogBuilder.SetItems(items, (d, args) =>
                {
                    //Take photo
                    if (args.Which == 0)
                    {
                        //                         Task.Run(async () =>
                        //                         {
                        //                             if (CrossMedia.Current.IsCameraAvailable)
                        //                             {
                        //                                 //photo = await CrossMedia.Current.PickPhotoAsync();
                        //                                 photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        //                                 {
                        //                                     Directory = "Invoices",
                        //                                     Name = "Invoice.jpg"
                        //                                 });
                        //                             }
                        //                             else
                        //                             {
                        // 
                        //                                 photo = await CrossMedia.Current.PickPhotoAsync();
                        //                             }
                        // 
                        //                         });

                        if (CrossMedia.Current.IsCameraAvailable)
                        {
                            takePhoto = true;
                            GetPicture();
                        }
                        else
                        {
                            chooseFromLib = true;
                            GetPicture();
                        }
                    }

                    //Choose from gallery
                    else if (args.Which == 1)
                    {
                        //                         Task.Run(async () =>
                        //                         {
                        // 
                        // 
                        //                             photo = await CrossMedia.Current.PickPhotoAsync();
                        // 
                        // 
                        //                         });
                        chooseFromLib = true;
                        GetPicture();
                    }
                });

                dialogBuilder.Show();
            }

            
            /*if (CrossMedia.Current.IsCameraAvailable)
            {
                photo = await CrossMedia.Current.PickPhotoAsync();
                / *photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Invoices",
                    Name = "Invoice.jpg"\\\\\\
                });* /
            }
            else
            {
                photo = await CrossMedia.Current.PickPhotoAsync();
            }*/

            

            /*Intent = new Intent();
            Intent.SetType("image/ *");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);*/

            
            
            //await SelectPicture();

        }
        public void MessageBox(string MyMessage)
        {

            AlertDialog.Builder builder;
            builder = new AlertDialog.Builder(this);
            builder.SetTitle("Translated text:");
            builder.SetMessage(MyMessage);
            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { });
            Dialog dialog = builder.Create();
            dialog.Show();
            return;
        }
        async void GetPicture()
        {
            await CrossMedia.Current.Initialize();
            if (takePhoto)
            {
                photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "OCR",
                    Name = "test.jpg",
                    PhotoSize = PhotoSize.Small
                });

                takePhoto = false;
            }
            else if (chooseFromLib)
            {
                photo = await CrossMedia.Current.PickPhotoAsync();

                chooseFromLib = false;
            }

            if (photo == null)
            {
                Toast.MakeText(this,
            "Photo was null :(", ToastLength.Long).Show();

                return;
            }
            else
            {
                textView.Text = photo.Path;
            }

            path = photo.Path;
            textView.Text = path;
            Android.Net.Uri uri = Android.Net.Uri.FromFile(new Java.IO.File(path));
            imageView.SetImageURI(uri);
        }
        private async Task<AnalysisResult> GetImageDescription(Stream imageStream)
        {
            VisionServiceClient visionClient = new VisionServiceClient("f5abbb3610b74e6fbda3af91c5d80d5f", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
            //VisionServiceClient visionClient = new VisionServiceClient("3401fb450931403bb8ed1c3a2e43f060");
            VisualFeature[] features = { VisualFeature.Tags };
            return await visionClient.AnalyzeImageAsync(imageStream, features.ToList(), null);
        }

        private async Task SelectPicture()
        {
            
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                var image = await CrossMedia.Current.PickPhotoAsync();
 
                try
                {
                    var result = await GetImageDescription(image.GetStream());
                    foreach (var tag in result.Tags)
                    {
                        editText.Text = tag.Name + "\n";
                    }
                }
                catch (ClientException ex)
                {
                    textView.Text = ex.Message;
                }
                
            }
        }
        async private void Button2OnClick(object sender, EventArgs eventArgs)
        {
            text = "";
            /*Task.Run(async () =>
            {*/
            /*TesseractApi api = new TesseractApi(this, AssetsDeployment.OncePerVersion);
            await api.Init("eng");
            await api.SetImage(path);
            string text = api.Text;
            editText.Text = text;*/

            /*OcrResults text;
            var client = new VisionServiceClient("d24a37c59473400eac2432564a5cc7dc");
            using (var photoStream = photo.GetStream())
            {
                text = await client.RecognizeTextAsync(photoStream);
                //photoStream.Seek(0, SeekOrigin.Begin);
            }
            
            editText.Text = LogOcrResults(text);*/
            /*});*/


            /*var client = new VisionServiceClient("d24a37c59473400eac2432564a5cc7dc");
            var output = "";
            using (var photoStream = photo.GetStream())
            {
                var result = client.RecognizeTextAsync(photoStream).Result;
                var words = from r in result.Regions
                            from l in r.Lines
                            from w in l.Words
                            select w.Text;

                output = string.Join(" ", words.ToArray());
                editText.Text = output;
            //write text to console
            //Console.WriteLine("Read text from image: " + output);

        }*/
            /*ProgressDialog progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage("Extracting Text..."); // Setting Message
            progressDialog.SetTitle("OCR"); // Setting Title
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner); // Progress Dialog Style Spinner
            progressDialog.Show(); // Display Progress Dialog
            progressDialog.SetCancelable(false);
            
            new Thread(new ThreadStart(() =>
  {
      
                try
                {*/

                    if (handwrittenCheckBox.Checked)
                    {
                        RecognizeHandwrittenTextAsync(photo);
                    }
                    else
                    {
                        await RecognizeTextAsync(photo);
                        //await RecognizeHandwrittenTextAsync(photo);
                    }

                    editText.Text = text;
                /*}
                catch (Java.Lang.Exception e)
                {
                    
                }
                progressDialog.
            
        })).Start();*/
            

            Intent translationActivity = new Intent(this, typeof(TranslationActivity));
            translationActivity.PutExtra("OCRdText", editText.Text);
            StartActivity(translationActivity);
        }

        protected string LogOcrResults(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (results != null && results.Regions != null)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append("\n");
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }
                        stringBuilder.Append("\n");
                    }
                    stringBuilder.Append("\n");
                }
            }
            return stringBuilder.ToString();
        }
        public async Task RecognizeTextAsync(MediaFile photo)
        {
            OcrResults ocrResults;
            try
            {
                var client = new VisionServiceClient("d641ed8db191419f86dbf38d68fbbc39", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");

                using (var photoStream = photo.GetStream())
                {
                    ocrResults = await client.RecognizeTextAsync(photoStream);
                }

                //var text = "";
                foreach (var region in ocrResults.Regions)
                {
                    foreach (var line in region.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            text = $"{text} {word.Text}";
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                MessageBox(e.Message + "\n" + e.ToString());
                //ShowToast(e.Message + "\n" + e.ToString());
            }


            //return text;
        }
        public void RecognizeHandwrittenTextAsync(MediaFile photo)
        {
            Task<HandwritingRecognitionOperation> s;
            Task<HandwritingRecognitionOperationResult> f;
            HandwritingTextResult rs;
            var client = new VisionServiceClient("d641ed8db191419f86dbf38d68fbbc39", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");

            

            try
            {
                using (var photoStream = photo.GetStream())
                {
                    s = client.CreateHandwritingRecognitionOperationAsync(photoStream);

                    f = client.GetHandwritingRecognitionOperationResultAsync(s.Result);
                    rs = f.Result.RecognitionResult;
                }

                foreach (var line in rs.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        text = $"{text} {word.Text}";
                    }
                }
            }
            catch (System.Exception e)
            {
                ShowToast(e.Message+"\n"+e.ToString());
            }
            

            //return text;
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

        private string GetPathToImage(Android.Net.Uri uri)
        {
            string path = null;
            // The projection contains the columns we want to return in our query.
            string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data };
            using (ICursor cursor = ManagedQuery(uri, projection, null, null, null))
            {
                if (cursor != null)
                {
                    int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    path = cursor.GetString(columnIndex);
                }
            }
            return path;
        }
        private string GetFilePath(Android.Net.Uri uri)
        {
            string[] proj = { MediaStore.Images.ImageColumns.Data };
            //Deprecated
            //var cursor = ManagedQuery(uri, proj, null, null, null);
            var cursor = ContentResolver.Query(uri, proj, null, null, null);
            var colIndex = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data);
            cursor.MoveToFirst();
            return cursor.GetString(colIndex);
        }
        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            string text="";
            if ((requestCode == PickImageId) && (resultCode == Android.App.Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                imageView.SetImageURI(uri);
                path = GetFilePath(uri);
                textView.Text = path;
                /*Task.Run(async () =>
            {
                ITesseractApi api = new TesseractApi(this,AssetsDeployment.OncePerVersion);
                bool initialised = await api.Init("eng");
                bool success = await api.SetImage(path);
                if (initialised)
                {
                    text = api.Text;
                    Toast.MakeText(this,
            "Clicked!", ToastLength.Long).Show();
                }
                
            });
                textView.Text = text;*/

                /*TessBaseAPI api = new TessBaseAPI(new MyProcessNotifier());
                bool result = api.Init("/mnt/sdcard/tesseract-ocr/", "eng");
                //storage/emulated/0/DCIM/Camera/IMG_20150530_124916.jpg
                //ExifInterface exif = new ExifInterface("/storage/emulated/0/DCIM/Camera/IMG_20150530_124916.jpg");
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InSampleSize = 4;

                Bitmap bitmap = BitmapFactory.DecodeFile(path, options);
                api.SetImage(bitmap);
                String recognizedText = api.UTF8Text;
                api.End();
                textView.Text = recognizedText;*/

            }
        }

    }


}

