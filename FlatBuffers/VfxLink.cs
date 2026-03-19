namespace ReblackVfx.FlatBuffers;

public struct VfxLinkDataTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxLinkDataTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public int InstanceId
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : 0;
        }
    }

    public int SourceNodeId
    {
        get
        {
            int offset = t_.__offset(6);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : 0;
        }
    }

    public int SourceMemberId
    {
        get
        {
            int offset = t_.__offset(8);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : 0;
        }
    }

    public int DestinationNodeId
    {
        get
        {
            int offset = t_.__offset(10);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : 0;
        }
    }

    public int DestinationMemberId
    {
        get
        {
            int offset = t_.__offset(12);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : 0;
        }
    }

    public int LinkType
    {
        get
        {
            int offset = t_.__offset(14);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : 0;
        }
    }

    public int Unknown
    {
        get
        {
            int offset = t_.__offset(16);
            // TODO: This defaults to something
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : 0;
        }
    }
}
