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

    public uint Children(int i)
    {
        int offset = t_.__offset(8);
        return offset != 0 ? t_.buffer_.ReadU32(t_.__vector(offset) + i * 4) : (int)0;
    }

    public int ChildrenLength
    {
        get
        {
            int offset = t_.__offset(8);
            return offset != 0 ? t_.__vector_length(offset) : (int)0;
        }
    }

    public static Offset<VfxTrayDataTable> CreateVfxTrayDataTable(
        FlatBufferBuilder builder,
        uint instanceId,
        uint typeId,
        uint[] references
    )
    {
        builder.StartVector(4, references.Length, 4);
        for (int i = references.Length - 1; i >= 0; i--)
            builder.AddUint(references[i]);
        VectorOffset children = builder.EndVector();

        builder.StartObject(3);
        builder.AddOffset(2, children.Value, 0);
        builder.AddUint(1, typeId, 0);
        builder.AddUint(0, instanceId, 0);
        return new Offset<VfxTrayDataTable>(builder.EndObject());
    }
}
