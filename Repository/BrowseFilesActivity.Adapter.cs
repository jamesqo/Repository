using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Octokit;
using Repository.Internal;
using static Repository.Common.Verify;
using Debug = System.Diagnostics.Debug;

namespace Repository
{
    public partial class BrowseFilesActivity
    {
        private class Adapter : RecyclerView.Adapter
        {
            private class ViewHolder : RecyclerView.ViewHolder
            {
                public TextView RepoNameView { get; }

                internal ViewHolder(View view, Action<int> onClick)
                    : base(view)
                {
                    RepoNameView = NotNull(view.FindViewById<TextView>(Resource.Id.FilenameView));

                    view.Click += (sender, e) => onClick(AdapterPosition);
                }
            }

            private readonly long _repoId;
            private readonly Stack<string> _directoryStack;

            private IReadOnlyList<RepositoryContent> _contents;

            private Adapter(long repoId)
            {
                _repoId = repoId;
                _directoryStack = new Stack<string>();
            }

            internal static async Task<Adapter> Create(long repoId)
            {
                var adapter = new Adapter(repoId);
                await adapter.PushDirectory("/");
                return adapter;
            }

            public IReadOnlyList<RepositoryContent> Contents => _contents;

            public string CurrentDirectory => _directoryStack.Peek();

            public event EventHandler<int> ItemClick;

            public bool IsAtRoot => _directoryStack.Count == 1;

            public override int ItemCount => Contents.Count;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var viewHolder = (ViewHolder)holder;
                var content = Contents[position];
                viewHolder.RepoNameView.Text = content.Name;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.BrowseFiles_CardView, parent, attachToRoot: false);
                return new ViewHolder(view, OnClick);
            }

            internal async Task<RepositoryContent> GetFullContent(string filePath)
            {
                var fullContents = await GitHub.Client.Repository.Content.GetAllContents(_repoId, filePath);
                return fullContents.Single();
            }

            internal Task PopDirectory()
            {
                Debug.Assert(!IsAtRoot);
                _directoryStack.Pop();
                return UpdateContents();
            }

            internal Task PushDirectory(string directory)
            {
                _directoryStack.Push(directory);
                return UpdateContents();
            }

            private void OnClick(int position) => ItemClick?.Invoke(this, position);

            private IEnumerable<RepositoryContent> SortContents(IEnumerable<RepositoryContent> contents)
            {
                int GetPriority(ContentType type)
                {
                    switch (type)
                    {
                        case ContentType.Dir: return 0;
                        case ContentType.File: return 1;
                        default: throw new NotImplementedException();
                    }
                }

                return contents.OrderBy(c => GetPriority(c.Type)).ThenBy(c => c.Name);
            }

            private async Task UpdateContents()
            {
                var unsortedContents = await GitHub.Client.Repository.Content.GetAllContents(_repoId, CurrentDirectory);
                _contents = SortContents(unsortedContents).ToReadOnly();
                NotifyDataSetChanged();
            }
        }
    }
}