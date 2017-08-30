using Android.Text;
using Java.Lang;

namespace Repository.Internal.Editor.Highlighting
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