namespace ReblackVfx.FlatBuffers;

public struct VfxGraphInfoTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxGraphInfoTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public VfxGraphVersionInfo? VersionInfo
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0
                ? (VfxGraphVersionInfo?)
                    (new VfxGraphVersionInfo()).assign(t_.origin_ + offset, t_.buffer_)
                : null;
        }
    }

    public static Offset<VfxGraphInfoTable> CreateVfxGraphInfoTable(
        FlatBufferBuilder builder,
        Offset<VfxGraphVersionInfo> versionInfoOffset
    )
    {
        builder.StartObject(1);
        builder.AddStruct(0, versionInfoOffset.Value, 0);
        int o = builder.EndObject();
        return new Offset<VfxGraphInfoTable>(o);
    }
}
