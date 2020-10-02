namespace AnimationEditorCore.Interfaces
{
    public interface IMementoOriginator
    {
        IMemento SaveState();
        void LoadState(IMemento state);
    }
}
