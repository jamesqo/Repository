using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using static Repository.Common.Verify;

namespace Repository
{
    public partial class ChooseRepoActivity
    {
        private class Adapter : RecyclerView.Adapter
        {
            private class ViewHolder : RecyclerView.ViewHolder
            {
                public TextView RepoNameView { get; }

                internal ViewHolder(View view, Action<int> onClick)
                    : base(view)
                {
                    RepoNameView = NotNull(view.FindViewById<TextView>(Resource.Id.ChooseRepo_RepoNameView));

                    view.Click += (sender, e) => onClick(AdapterPosition);
                }
            }

            internal Adapter(IReadOnlyList<Octokit.Repository> repos)
            {
                // TODO: What if there are no repos? The user will be stuck on a blank screen.
                Repos = NotNull(repos, nameof(repos));
            }

            public event EventHandler<int> ItemClick;

            public override int ItemCount => Repos.Count;

            public IReadOnlyList<Octokit.Repository> Repos { get; }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var viewHolder = (ViewHolder)holder;
                viewHolder.RepoNameView.Text = Repos[position].Name;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var inflater = LayoutInflater.From(parent.Context);
                var view = inflater.Inflate(Resource.Layout.ChooseRepo_CardView, parent, attachToRoot: false);
                return new ViewHolder(view, OnClick);
            }

            private void OnClick(int position) => ItemClick?.Invoke(this, position);
        }
    }
}