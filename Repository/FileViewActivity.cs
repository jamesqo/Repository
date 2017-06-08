using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Repository.Internal;

namespace Repository
{
    // TODO: "Select a File" instead?
    // TODO: Put the current directory path in tiny text below the activity label?
    [Activity(Label = "File View")]
    public class FileViewActivity : Activity
    {
        private sealed class GitHubFileAdapter : RecyclerView.Adapter
        {
            public override int ItemCount => throw new NotImplementedException();

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                throw new NotImplementedException();
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                throw new NotImplementedException();
            }
        }

        private RecyclerView _fileView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.FileView);

            // GitHub.Client.Repository.

            _fileView = FindViewById<RecyclerView>(Resource.Id.FileView);
        }
    }
}