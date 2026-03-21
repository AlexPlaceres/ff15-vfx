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

    public uint InstanceId
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0 ? t_.buffer_.ReadU32(t_.origin_ + offset) : 0;
        }
    }

    public uint SourceNodeId
    {
        get
        {
            int offset = t_.__offset(6);
            return offset != 0 ? t_.buffer_.ReadU32(t_.origin_ + offset) : 0;
        }
    }

    public uint SourceMemberId
    {
        get
        {
            int offset = t_.__offset(8);
            return offset != 0 ? t_.buffer_.ReadU32(t_.origin_ + offset) : 0;
        }
    }

    public uint DestinationNodeId
    {
        get
        {
            int offset = t_.__offset(10);
            return offset != 0 ? t_.buffer_.ReadU32(t_.origin_ + offset) : 0;
        }
    }

    public uint DestinationMemberId
    {
        get
        {
            int offset = t_.__offset(12);
            return offset != 0 ? t_.buffer_.ReadU32(t_.origin_ + offset) : 0;
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

    public static Offset<VfxLinkDataTable> CreateVfxLinkDataTable(
        FlatBufferBuilder builder,
        uint instanceId,
        uint sourceNodeId,
        uint sourceMemberId,
        uint destinationNodeId,
        uint destinationMemberId,
        int linkType,
        int unknown
    )
    {
        builder.StartObject(7);
        builder.AddUint(0, instanceId, 0);
        builder.AddUint(1, sourceNodeId, 0);
        builder.AddUint(2, sourceMemberId, 0);
        builder.AddUint(3, destinationNodeId, 0);
        builder.AddUint(4, destinationMemberId, 0);
        builder.AddInt(5, linkType, 0);
        builder.AddInt(6, unknown, 0);
        return new Offset<VfxLinkDataTable>(builder.EndObject());
    }
}
