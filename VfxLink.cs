using ReblackVfx;

public struct VfxSignalLinkTable : IFlatBufferObj
{
    public FlatBufferTable t_;
    public void init(int origin, FlatBuffer buffer) { t_.origin_ = origin; t_.buffer_ = buffer; }
    public VfxSignalLinkTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }
}

public struct VfxPropertyLinkTable : IFlatBufferObj
{
    public FlatBufferTable t_;
    public void init(int origin, FlatBuffer buffer) { t_.origin_ = origin; t_.buffer_ = buffer; }
    public VfxPropertyLinkTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }
}
