using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Repository.Internal;

namespace Repository
{
    [Activity(Label = Strings.Label_ChooseRepo)]
    public partial class ChooseRepoActivity : Activity
    {
        private RecyclerView _repoView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _repoView = FindViewById<RecyclerView>(Resource.Id.RepoView);
            }

            base.OnCreate(savedInstanceState);

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

        private async Task<Adapter> GetRepoViewAdapter()
        {
            var repos = await GitHub.Client.Repository.GetAllForCurrent();
            var adapter = new Adapter(repos);
            adapter.ItemClick += Adapter_ItemClick;
            return adapter;
        }

        private async Task SetupRepoView()
        {
            _repoView.SetAdapter(await GetRepoViewAdapter());
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