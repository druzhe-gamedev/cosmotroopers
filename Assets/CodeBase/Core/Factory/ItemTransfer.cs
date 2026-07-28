namespace CodeBase.Core.Factory
{
    public struct ItemTransfer
    {
        public ItemOnBelt Item { get; private set; }
        public FactoryNode Emitter { get; private set; }

        public ItemTransfer(ItemOnBelt item, FactoryNode emitter)
        {
            Item = item;
            Emitter = emitter;
        }
    }
}