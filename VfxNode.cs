using ReblackVfx;

public struct VfxPropertyValueTable : IFlatBufferObj
{
    public FlatBufferStruct s_;
    public void init(int origin, FlatBuffer buffer) { s_.origin_ = origin; s_.buffer_ = buffer; }
    public VfxPropertyValueTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }

    public int Unknown { get { return s_.buffer_.ReadS32(s_.origin_ + 0); } }
    public int ValueIndex { get { return s_.buffer_.ReadS32(s_.origin_ + 4); } }
}

public struct VfxPropertyIdTable : IFlatBufferObj
{
    public FlatBufferStruct s_;

    public void init(int origin, FlatBuffer buffer) { s_.origin_ = origin; s_.buffer_ = buffer; }
    public VfxPropertyIdTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }

    public int MemberId { get { return s_.buffer_.ReadS32(s_.origin_ + 0); } }
    public uint Name { get { return s_.buffer_.ReadU32(s_.origin_ + 4); } }
}

public struct VfxPropertyDataTable : IFlatBufferObj
{
    public FlatBufferTable t_;
    public void init(int origin, FlatBuffer buffer) { t_.origin_ = origin; t_.buffer_ = buffer; }
    public VfxPropertyDataTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }

    public VfxPropertyIdTable Identity
    {
        get { int offset = t_.__offset(4); return new VfxPropertyIdTable().assign(t_.origin_ + offset, t_.buffer_); }
    }

    public int ValueType
    {
        get { int offset = t_.__offset(6); return t_.buffer_.ReadS32(t_.origin_ + offset); }
    }

    public VfxPropertyValueTable PropertyValue
    {
        get
        {
            int offset = t_.__offset(8);
            return new VfxPropertyValueTable().assign(t_.origin_ + offset, t_.buffer_);
        }
    }
}

public struct VfxNodeIdentityStruct : IFlatBufferObj
{
    public FlatBufferStruct s_;
    public void init(int origin, FlatBuffer buffer) { s_.origin_ = origin; s_.buffer_ = buffer; }
    public VfxNodeIdentityStruct assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }

    public uint InstanceId { get { return s_.buffer_.ReadU32(s_.origin_ + 0); } }
    public uint DesignId { get { return s_.buffer_.ReadU32(s_.origin_ + 4); } }
}

public struct VfxNodeDataTable : IFlatBufferObj
{
    public FlatBufferTable t_;
    public void init(int origin, FlatBuffer buffer) { t_.origin_ = origin; t_.buffer_ = buffer; }
    public VfxNodeDataTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }

    public VfxNodeIdentityStruct Identity
    {
        get
        {
            int offset = t_.__offset(4);
            return new VfxNodeIdentityStruct().assign(t_.origin_ + offset, t_.buffer_);
        }
    }

    public VfxPropertyDataTable Properties(int i)
    {
        int offset = t_.__offset(6);
        return new VfxPropertyDataTable().assign(
                    t_.__relative(t_.__vector(offset) + i * 4), t_.buffer_);
    }

    public int PropertiesLength
    {
        get { int offset = t_.__offset(6); return offset != 0 ? t_.__vector_length(offset) : 0; }
    }
}
