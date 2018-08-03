using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Com.Github.Library.Bubbleview;
using EnglishTranslationApp.Models;

namespace EnglishTranslationApp.Adapters
{
    public class ChatAdapter : BaseAdapter
    {
        private List<ChatModel> listChatModel, listTranslatedChatModel;
        private Context context;
        private LayoutInflater inflater;
        public ChatAdapter(List<ChatModel> listChatModel, List<ChatModel> listTranslatedChatModel, Context context)
        {
            this.context = context;
            this.listChatModel = listChatModel;
            this.listTranslatedChatModel = listTranslatedChatModel;
            inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
        }
        public override int Count
        {
            get
            {
                return listChatModel.Count;
            }
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                if (listChatModel[position].IsSend)
                {
                    view = inflater.Inflate(Resource.Layout.List_item_message_right, null);
                }

                else
                {
                    view = inflater.Inflate(Resource.Layout.List_item_message_left, null);
                }
                    

                BubbleTextView bubbleTextView = view.FindViewById<BubbleTextView>(Resource.Id.text_message);
                BubbleTextView bubbleTranslatedTextView = view.FindViewById<BubbleTextView>(Resource.Id.text_translated_message);
                bubbleTextView.Text = (listChatModel[position].ChatMessage);
                bubbleTranslatedTextView.Text = (listTranslatedChatModel[position].ChatMessage);
            }
            return view;
        }
    }
}