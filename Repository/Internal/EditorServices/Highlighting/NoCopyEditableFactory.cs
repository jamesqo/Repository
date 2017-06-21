using Android.Text;
using Java.Lang;

namespace Repository.Internal.EditorServices.Highlighting
{
    internal class NoCopyEditableFactory : EditableFactory
    {
        public new static NoCopyEditableFactory Instance { get; } = new NoCopyEditableFactory();

        private NoCopyEditableFactory()
        {
        }

        public override IEditable NewEditable(ICharSequence source) => (IEditable)source;
    }
}