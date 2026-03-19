namespace ReblackVfx.FlatBuffers;

public struct VfxSignalDataTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxSignalDataTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public VfxMemberIdTable? Identity
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0
                ? (VfxMemberIdTable?)
                    (new VfxMemberIdTable()).assign(t_.origin_ + offset, t_.buffer_)
                : null;
        }
    }

    public int ArgumentTypes
    {
        get
        {
            int offset = t_.__offset(6);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : (int)0;
        }
    }
}
