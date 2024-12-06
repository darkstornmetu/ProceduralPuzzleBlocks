namespace ProceduralBlocks.Selection
{
    public interface ISelectable
    {
        bool CanSelect();
        void Select();
        void Deselect();
    }
}