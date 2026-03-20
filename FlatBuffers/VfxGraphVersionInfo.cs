namespace ReblackVfx.FlatBuffers;

public struct VfxGraphVersionInfo : IFlatBufferObj
{
    public FlatBufferStruct s_;

    public void init(int origin, FlatBuffer buffer)
    {
        s_.origin_ = origin;
        s_.buffer_ = buffer;
    }

    public VfxGraphVersionInfo assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public int Self
    {
        get { return s_.buffer_.ReadS32(s_.origin_ + 0); }
    }
    public int Tool
    {
        get { return s_.buffer_.ReadS32(s_.origin_ + 4); }
    }
    public int Converter
    {
        get { return s_.buffer_.ReadS32(s_.origin_ + 8); }
    }

    public static Offset<VfxGraphVersionInfo> CreateVfxGraphVersionInfo(
        FlatBufferBuilder builder,
        int Self,
        int Tool,
        int Converter
    )
    {
        builder.Prep(4, 12);
        builder.PutInt(Converter);
        builder.PutInt(Tool);
        builder.PutInt(Self);
        return new Offset<VfxGraphVersionInfo>(builder.Offset);
    }
}
