using ReblackVfx;

public struct VfxTrayDataTable : IFlatBufferObj
{
    public FlatBufferTable t_;
    public void init(int origin, FlatBuffer buffer) { t_.origin_ = origin; t_.buffer_ = buffer; }
    public VfxTrayDataTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }
}
