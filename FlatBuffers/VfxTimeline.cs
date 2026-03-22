namespace ReblackVfx.FlatBuffers;

using System.Xml.Linq;

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

    public int LoopIn
    {
        get
        {
            int offset = t_.__offset(4);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : (int)0;
        }
    }

    public int LoopOut
    {
        get
        {
            int offset = t_.__offset(6);
            return offset != 0 ? t_.buffer_.ReadS32(t_.origin_ + offset) : (int)0;
        }
    }

    public int CurveLoopType
    {
        get
        {
            int offset = t_.__offset(8);
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

    public static Offset<VfxFCurveTable> CreateVfxFCurveTable(
        FlatBufferBuilder builder,
        int LoopIn,
        int LoopOut,
        int Loop,
        VectorOffset AnchorPoints
    )
    {
        builder.StartObject(4);
        builder.AddOffset(3, AnchorPoints.Value, 0);
        builder.AddInt(Loop);
        builder.AddInt(LoopOut);
        builder.AddInt(LoopIn);

        return new Offset<VfxFCurveTable>(builder.EndObject());
    }

    public static Offset<VfxFCurveTable>[]? SerializeXml(FlatBufferBuilder builder, XElement? table)
    {
        if (table is not XElement fCurves)
            return null;

        var elements = fCurves.Elements().ToArray();
        if (elements.Length == 0)
            return null;

        Offset<VfxFCurveTable>[] result = new Offset<VfxFCurveTable>[elements.Length];
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i] is not XElement fCurve)
                throw new InvalidDataException();

            if (
                fCurve.Attribute("loopIn") is not XAttribute loopIn
                || fCurve.Attribute("loopOut") is not XAttribute loopOut
                || fCurve.Attribute("loopType") is not XAttribute loopType
            )
                throw new Exception("fCurve must have loopIn, loopOut, and loopType attributes");

            if (fCurve.Element("anchorPoints") is not XElement anchorPoints)
                throw new Exception("fCurve must have anchor points");

            var anchorPointElements = anchorPoints.Elements().ToArray();
            builder.StartVector(16, anchorPointElements.Length, 16);
            for (int j = anchorPointElements.Length - 1; j >= 0; j--)
            {
                if (anchorPointElements[j] is not XElement anchorPoint)
                    throw new InvalidDataException();

                if (anchorPoint.Attribute("time") is not XAttribute time)
                    throw new Exception("anchorPoint must have time attribute");

                float[] values = anchorPoint.Value.Split(",").Select(s => float.Parse(s)).ToArray();
                if (values.Length != 3)
                    throw new Exception($"fCurve anchor points must have 3 values");

                builder.AddFloat(values[2]);
                builder.AddFloat(values[1]);
                builder.AddFloat(values[0]);
                builder.AddFloat(float.Parse(time.Value));
            }
            VectorOffset anchorPointOffset = builder.EndVector();
            result[i] = CreateVfxFCurveTable(
                builder,
                int.Parse(loopIn.Value),
                int.Parse(loopOut.Value),
                int.Parse(loopType.Value),
                anchorPointOffset
            );
        }
        return result;
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
