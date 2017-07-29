using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Repository.Internal;

namespace Repository
{
    [Activity]
    public partial class ChooseRepoActivity : Activity
    {
        private RecyclerView _repoView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _repoView = FindViewById<RecyclerView>(Resource.Id.ChooseRepo_RepoView);
            }

            base.OnCreate(savedInstanceState);

            Title = Resources.GetString(Resource.String.Label_ChooseRepo);
            SetContentView(Resource.Layout.ChooseRepo);
            CacheViews();

            await SetupRepoView();
        }

        private void Adapter_ItemClick(object sender, int e)
        {
            var adapter = (Adapter)sender;
            var repo = adapter.Repos[e];
            StartBrowseFiles(repoId: repo.Id);
        }

        private async Task<Adapter> GetAdapter()
        {
            var repos = await GitHub.Client.Repository.GetAllForCurrent();
            var adapter = new Adapter(repos);
            adapter.ItemClick += Adapter_ItemClick;
            return adapter;
        }

        private async Task SetupRepoView()
        {
            _repoView.SetAdapter(await GetAdapter());
            _repoView.SetLayoutManager(new LinearLayoutManager(this));
        }

        private void StartBrowseFiles(long repoId)
        {
            var intent = new Intent(this, typeof(BrowseFilesActivity));
            intent.PutExtra(Strings.Extra_BrowseFiles_RepoId, repoId);
            StartActivity(intent);
        }
    }
}