namespace ReblackVfx.FlatBuffers;

public struct VfxFCurveTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxFCurveTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public int CurveLoopType
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : (int)0;
        }
    }

    public LmVector4? Keys(int i)
    {
        int offset = t_.__offset(10);
        return offset != 0
            ? (LmVector4?)(new LmVector4()).assign(t_.__vector(offset) + i * 16, t_.buffer_)
            : null;
    }

    public int KeysLength
    {
        get
        {
            int offset = t_.__offset(10);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }
}

public struct VfxTimelineDataTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxTimelineDataTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    // TODO: 6 Fields

    public VfxFCurveTable? Curves(int i)
    {
        int offset = t_.__offset(16);
        return offset != 0
            ? (VfxFCurveTable?)
                (new VfxFCurveTable()).assign(
                    t_.__relative(t_.__vector(offset) + i * 4),
                    t_.buffer_
                )
            : null;
    }

    public int CurvesLength
    {
        get
        {
            int offset = t_.__offset(16);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }
}
