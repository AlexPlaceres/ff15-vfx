namespace ReblackVfx.FlatBuffers;

public struct VfxGraphContainerTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxGraphContainerTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    // Node Data
    public VfxNodeDataTable Nodes(int i)
    {
        int offset = t_.__offset(4);
        return new VfxNodeDataTable().assign(
            t_.__relative(t_.__vector(offset) + i * 4),
            t_.buffer_
        );
    }

    public int NodesLength
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // Tray Data
    public VfxTrayDataTable? Trays(int i)
    {
        int offset = t_.__offset(6);
        return offset != 0
            ? (VfxTrayDataTable?)
                (new VfxTrayDataTable()).assign(
                    t_.__relative(t_.__vector(offset) + i * 4),
                    t_.buffer_
                )
            : null;
    }

    public int TraysLength
    {
        get
        {
            int offset = t_.__offset(6);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // Signal Link Data
    public VfxLinkDataTable? SignalLinks(int i)
    {
        int offset = t_.__offset(8);
        return offset != 0
            ? (VfxLinkDataTable?)
                (new VfxLinkDataTable()).assign(
                    t_.__relative(t_.__vector(offset) + i * 4),
                    t_.buffer_
                )
            : null;
    }

    public int SignalLinksLength
    {
        get
        {
            int offset = t_.__offset(8);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }

    // Property Link Data
    public VfxLinkDataTable? PropertyLinks(int i)
    {
        int offset = t_.__offset(10);
        return offset != 0
            ? (VfxLinkDataTable?)
                (new VfxLinkDataTable()).assign(
                    t_.__relative(t_.__vector(offset) + i * 4),
                    t_.buffer_
                )
            : null;
    }

    public int PropertyLinksLength
    {
        get
        {
            int offset = t_.__offset(10);
            return offset != 0 ? t_.__vector_length(offset) : 0;
        }
    }
}
