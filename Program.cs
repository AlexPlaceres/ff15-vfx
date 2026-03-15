using System.Xml;
using CommunityToolkit.HighPerformance.Buffers;
using ReblackVfx;



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
                t_.__relative(t_.__vector(offset) + i * 4), t_.buffer_);
    }
    public int NodesLength
    {
        get { int offset = t_.__offset(4); return offset != 0 ? t_.__vector_length(offset) : 0; }
    }
    // Tray Data
    // Signal Link Data
    // Property Link Data
}

public struct VfxGraphTable : IFlatBufferObj
{
    public FlatBufferTable t_;
    public void init(int origin, FlatBuffer buffer) { t_.origin_ = origin; t_.buffer_ = buffer; }
    public VfxGraphTable assign(int origin, FlatBuffer buffer) { init(origin, buffer); return this; }


    // Version Info
    public VfxGraphContainerTable GraphContainer
    {
        get
        {
            int offset = t_.__offset(6);
            return new VfxGraphContainerTable().assign(t_.__relative(t_.origin_ + offset), t_.buffer_);
        }
    }

    public GraphPropertyValueTable ValueData
    {
        get
        {
            int offset = t_.__offset(8);
            return new GraphPropertyValueTable().assign(t_.__relative(t_.origin_ + offset), t_.buffer_);
        }
    }

}

public struct VfxTimeline : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }
    public VfxTimeline assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }
}

public struct VfxGraphRoot
{
    public FlatBufferTable t_;
    public VfxGraphTable Graph
    {
        get
        {
            int offset = t_.__offset(4);
            int origin = t_.__relative(t_.origin_ + offset);
            return new VfxGraphTable().assign(origin, t_.buffer_);
        }
    }
    public VfxTimeline? Timeline
    {
        get
        {
            int offset = t_.__offset(6);
            int origin = t_.__relative(t_.origin_ + offset);
            return new VfxTimeline().assign(origin, t_.buffer_);
        }
    }

    public static VfxGraphRoot GetRoot(FlatBuffer buffer, int origin)
    {
        var result = new VfxGraphRoot();

        // Initialize
        result.t_.buffer_ = buffer;
        result.t_.origin_ = origin;
        return result;
    }
}

internal static class Program
{
    public delegate VfxPropertyBase PropertyFunc(VfxPropertyDataTable prop, GraphPropertyValueTable val);
    private static readonly string FILEPATH = @"/home/alex/Downloads/ramuh_exp.vfx";

    private static readonly PropertyFunc[] funcs = {
        VfxPropertyBase.Invalid,
        VfxBoolProperty.FromFlat,
        VfxInt32Property.FromFlat,
        VfxFloat32Property.FromFlat,
        VfxVector2Property.FromFlat,
        VfxVector3Property.FromFlat,
        VfxVector4Property.FromFlat,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
        VfxPropertyBase.Invalid,
    };

    public static void Main(string[] args)
    {
        long size;
        MemoryOwner<byte> buffer;
        using (var stream = new FileStream(FILEPATH, FileMode.Open))
        {
            size = stream.Length;
            buffer = MemoryOwner<byte>.Allocate((int)size);
            stream.ReadExactly(buffer.Span);
        }

        FlatBuffer fb = new FlatBuffer(buffer);
        int origin = fb.ReadS32(16);

        var root = VfxGraphRoot.GetRoot(fb, 16 + origin);

        var graph = root.Graph;
        var container = graph.GraphContainer;
        var count = container.NodesLength;

        VfxGraphData graphData = new VfxGraphData();
        graphData.Nodes.Capacity = count;


        var valueData = graph.ValueData;

        for (var i = 0; i < count; i++)
        {
            VfxNodeDataTable node = container.Nodes(i);

            var identity = node.Identity;
            var nodeObj =
                new VfxNodeData { NodeId = identity.InstanceId, NodeTypeId = identity.DesignId };

            int propCount = node.PropertiesLength;
            for (var j = 0; j < propCount; j++)
            {
                var prop = node.Properties(j);
                var valType = prop.ValueType;
                VfxPropertyBase property;
                var vIndex = prop.PropertyValue.ValueIndex;
                try
                {
                    if (valType < 7)
                        property = funcs[valType](prop, valueData);
                    else
                        continue;
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine($"Node {i}, prop {j}: {vIndex}");
                    throw;
                }

                nodeObj.Properties.Add(property);
            }
            graphData.Nodes.Add(nodeObj);
        }


        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.OmitXmlDeclaration = true;
        settings.Async = false;
        using (var stream = new FileStream("/home/alex/Programming/lm-vfx/toxml/mockup.xml", FileMode.OpenOrCreate))
        {
            using XmlWriter writer = XmlWriter.Create(stream, settings);
            graphData.WriteXml(writer);
        }
    }
}
