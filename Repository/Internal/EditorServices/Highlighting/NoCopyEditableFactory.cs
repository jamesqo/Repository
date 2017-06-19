using Android.Text;
using Java.Lang;

namespace Repository.Internal.EditorServices.Highlighting
{
    // TODO: Should this be in an Internal namespace? It's used directly by an Activity.
    internal class NoCopyEditableFactory : EditableFactory
    {
        public new static NoCopyEditableFactory Instance { get; } = new NoCopyEditableFactory();

        private NoCopyEditableFactory()
        {
        }

        public override IEditable NewEditable(ICharSequence source) => (IEditable)source;
    }
}