namespace ReblackVfx.FlatBuffers;

public struct Int32VectorTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public Int32VectorTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    public int Values(int i)
    {
        int offset = t_.__offset(4);
        return offset != 0 ? t_.buffer_.ReadS32(t_.__vector(offset) + i * 4) : (int)0;
    }

    public int ValuesLength
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }
}
