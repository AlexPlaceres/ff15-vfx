namespace ReblackVfx.FlatBuffers;

public struct VfxTrayDataTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxTrayDataTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    // FIXME: Single field for id and type id
    public VfxNodeIdentityStruct? Identity
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0
                ? (VfxNodeIdentityStruct?)
                    (new VfxNodeIdentityStruct()).assign(t_.origin_ + offset, t_.buffer_)
                : null;
        }
    }

    public int Children(int i)
    {
        int offset = t_.__offset(8);
        return offset != 0 ? t_.buffer_.ReadS32(t_.__vector(offset) + i * 4) : (int)0;
    }

    public int ChildrenLength
    {
        get
        {
            int offset = t_.__offset(8);
            return offset != 0 ? t_.__vector_length(offset) : (int)0;
        }
    }
}
