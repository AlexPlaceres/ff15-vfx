using System.Text;
using System.Xml.Linq;
using CommunityToolkit.HighPerformance.Buffers;
using ReblackVfx;
using ReblackVfx.FlatBuffers;

public struct VfxGraphTable : IFlatBufferObj
{
    public FlatBufferTable t_;

    public void init(int origin, FlatBuffer buffer)
    {
        t_.origin_ = origin;
        t_.buffer_ = buffer;
    }

    public VfxGraphTable assign(int origin, FlatBuffer buffer)
    {
        init(origin, buffer);
        return this;
    }

    // NOTE: Version info isn't really necessary but maybe later

    public VfxGraphContainerTable GraphContainer
    {
        get
        {
            int offset = t_.__offset(6);
            return new VfxGraphContainerTable().assign(
                t_.__relative(t_.origin_ + offset),
                t_.buffer_
            );
        }
    }

    public GraphPropertyValueTable ValueData
    {
        get
        {
            int offset = t_.__offset(8);
            return new GraphPropertyValueTable().assign(
                t_.__relative(t_.origin_ + offset),
                t_.buffer_
            );
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
    public VfxTimelineDataTable? Timeline
    {
        get
        {
            int offset = t_.__offset(6);
            int origin = t_.__relative(t_.origin_ + offset);
            return new VfxTimelineDataTable().assign(origin, t_.buffer_);
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
    public static void ToBinary(string filePath)
    {
        XElement root = XElement.Load(filePath);
        if ((string?)root.Attribute("name") is string s)
        {
            Console.WriteLine($"Attempting to serialize VFX Graph {s}...");
        }

        // INFO: Output is built backwards
        // 1. Version info struct, graph info table
        // 2. Int32 data
        // Last: Root Table
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);

        var versionInfoOffset = VfxGraphVersionInfo.CreateVfxGraphVersionInfo(builder, 1, 2, 3);
        var graphInfoOffset = VfxGraphInfoTable.CreateVfxGraphInfoTable(builder, versionInfoOffset);

        // INFO: Step 2: Value data
        if (root.Element("data") is not XElement data)
            throw new InvalidDataException();

        if (data.Element("ints") is not XElement ints)
            throw new InvalidDataException();

        if (data.Element("booleans") is not XElement booleans)
            throw new InvalidDataException();

        var intElements = ints.Elements();
        Console.WriteLine($"Writing {intElements.Count()} int values...");
        int[] intData = new int[intElements.Count()];
        int i = 0;
        foreach (var element in intElements)
        {
            if (element is not XElement intEl)
                throw new InvalidDataException();
            intData[i++] = int.Parse(intEl.Value);
        }

        var boolElements = booleans.Elements();
        bool[] boolData = new bool[boolElements.Count()];
        i = 0;
        foreach (var element in boolElements)
        {
            if (element is not XElement e)
                throw new InvalidDataException();
            boolData[i++] = bool.Parse(e.Value);
        }

        var intOff = GraphPropertyValueTable.CreateInt32_Vector(builder, intData);
        var boolOff = GraphPropertyValueTable.CreateBoolean_Vector(builder, boolData);
        builder.Finish(boolOff.Value);

        using (var stream = new FileStream("H:/Project/ff15-vfx/output.vfx", FileMode.Create))
        {
            stream.Write(builder.SizedSpan());
        }
    }

    public static XElement NodesToXml(ref VfxGraphContainerTable container)
    {
        int count = container.NodesLength;
        XElement[] nodes = new XElement[count];
        for (int i = 0; i < count; i++)
        {
            if (container.Nodes(i) is VfxNodeDataTable t)
            {
                VfxNodeIdentityStruct id = t.Identity;
                string type = NodeMetadata.NodeType.ContainsKey((int)id.DesignId)
                    ? NodeMetadata.NodeType[(int)id.DesignId]
                    : $"VfxNode_{id.DesignId:X}";

                XElement[] properties = new XElement[t.PropertiesLength];
                XElement[] inPorts = new XElement[t.InPortsLength];
                XElement[] outPorts = new XElement[t.OutPortsLength];
                for (int j = 0; j < properties.Length; j++)
                {
                    if (t.Properties(j) is VfxPropertyDataTable p)
                    {
                        VfxMemberIdTable propId = p.Identity;
                        VfxPropertyValueTable propVal = p.PropertyValue;

                        properties[j] = new XElement(
                            "property",
                            [
                                new XAttribute("memberId", propId.MemberId),
                                new XAttribute("name", propId.Name),
                                new XAttribute("valueTable", p.ValueType),
                                new XAttribute(
                                    "baseType",
                                    NodeMetadata.VfxBaseTypeNames[propVal.ValueRef]
                                ),
                                new XAttribute("valueRef", propVal.ValueIndex),
                            ]
                        );
                    }
                    else
                        throw new InvalidDataException();
                }
                for (int j = 0; j < inPorts.Length; j++)
                {
                    if (t.InPorts(j) is not VfxSignalDataTable port)
                        throw new InvalidDataException();

                    if (port.Identity is not VfxMemberIdTable portId)
                        throw new InvalidDataException();

                    inPorts[j] = new XElement(
                        "port",
                        [
                            new XAttribute("memberId", portId.MemberId),
                            new XAttribute("name", portId.Name),
                            new XAttribute("argumentTypes", port.ArgumentTypes),
                        ]
                    );
                }
                for (int j = 0; j < outPorts.Length; j++)
                {
                    if (t.OutPorts(j) is not VfxSignalDataTable port)
                        throw new InvalidDataException();

                    if (port.Identity is not VfxMemberIdTable portId)
                        throw new InvalidDataException();

                    outPorts[j] = new XElement(
                        "port",
                        [
                            new XAttribute("memberId", portId.MemberId),
                            new XAttribute("name", portId.Name),
                            new XAttribute("argumentTypes", port.ArgumentTypes),
                        ]
                    );
                }
                nodes[i] = new XElement(
                    "node",
                    [
                        new XAttribute("instanceId", id.InstanceId),
                        new XAttribute("type", type),
                        new XAttribute("typeId", id.DesignId),
                        new XElement("properties", properties),
                        new XElement("inputPorts", inPorts),
                        new XElement("outputPorts", outPorts),
                    ]
                );
            }
            else
                throw new InvalidDataException();
        }

        return new XElement("nodes", nodes);
    }

    public static XElement TraysToXml(ref VfxGraphContainerTable container)
    {
        XElement[] trays = new XElement[container.TraysLength];
        for (int i = 0; i < trays.Length; i++)
        {
            if (container.Trays(i) is not VfxTrayDataTable t)
                throw new InvalidDataException();
            if (t.Identity is not VfxNodeIdentityStruct id)
                throw new InvalidDataException();
            string type = NodeMetadata.NodeType.ContainsKey((int)id.DesignId)
                ? NodeMetadata.NodeType[(int)id.DesignId]
                : $"VfxNode_{id.DesignId:X}";
            XElement[] children = new XElement[t.ChildrenLength];
            for (int j = 0; j < children.Length; j++)
            {
                children[j] = new XElement(
                    "reference",
                    [new XAttribute("instanceId", t.Children(j))]
                );
            }
            trays[i] = new XElement(
                "tray",
                [
                    new XAttribute("instanceId", id.InstanceId),
                    new XAttribute("type", type),
                    new XAttribute("typeId", id.DesignId),
                    new XElement("nodes", children),
                ]
            );
        }

        return new XElement("trays", trays);
    }

    public static XElement SignalLinksToXml(ref VfxGraphContainerTable container)
    {
        XElement[] links = new XElement[container.SignalLinksLength];
        for (int i = 0; i < links.Length; i++)
        {
            if (container.SignalLinks(i) is not VfxLinkDataTable t)
                throw new InvalidDataException();

            links[i] = new XElement(
                "link",
                [
                    new XAttribute("instanceId", t.InstanceId),
                    new XAttribute("sourceNodeId", t.SourceNodeId),
                    new XAttribute("sourceMemberId", t.SourceMemberId),
                    new XAttribute("destinationNodeId", t.DestinationNodeId),
                    new XAttribute("destinationMemberId", t.DestinationMemberId),
                    new XAttribute("linkType", t.LinkType),
                    new XAttribute("unknown", t.Unknown),
                ]
            );
        }

        return new XElement("signalLinks", links);
    }

    public static XElement PropertyLinksToXml(ref VfxGraphContainerTable container)
    {
        XElement[] links = new XElement[container.PropertyLinksLength];
        for (int i = 0; i < links.Length; i++)
        {
            if (container.PropertyLinks(i) is not VfxLinkDataTable t)
                throw new InvalidDataException();

            links[i] = new XElement(
                "link",
                [
                    new XAttribute("instanceId", t.InstanceId),
                    new XAttribute("sourceNodeId", t.SourceNodeId),
                    new XAttribute("sourceMemberId", t.SourceMemberId),
                    new XAttribute("destinationNodeId", t.DestinationNodeId),
                    new XAttribute("destinationMemberId", t.DestinationMemberId),
                    new XAttribute("linkType", t.LinkType),
                    new XAttribute("unknown", t.Unknown),
                ]
            );
        }

        return new XElement("propertyLinks", links);
    }

    public static XElement DataToXml(ref GraphPropertyValueTable table)
    {
        XElement[] booleans = new XElement[table.BooleanLength];
        XElement[] ints = new XElement[table.Int32Length];
        XElement[] floats = new XElement[table.Float32Length];
        XElement[] strings = new XElement[table.ByteArrayLength];
        XElement[] float2s = new XElement[table.Vector2Length];
        XElement[] float3s = new XElement[table.Vector3Length];
        XElement[] float4s = new XElement[table.Vector4Length];
        XElement[] matrices = new XElement[table.Matrix44Length];
        XElement[] int32Vectors = new XElement[table.Int32VectorLength];

        for (int i = 0; i < booleans.Length; i++)
        {
            booleans[i] = new XElement("bool", [new XAttribute("index", i), table.Boolean(i)]);
        }

        for (int i = 0; i < ints.Length; i++)
        {
            ints[i] = new XElement("int", [new XAttribute("index", i), table.Int32(i)]);
        }

        for (int i = 0; i < floats.Length; i++)
        {
            floats[i] = new XElement("float", [new XAttribute("index", i), table.Float32(i)]);
        }

        var encoding = new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true
        );
        for (int i = 0; i < strings.Length; i++)
        {
            if (table.ByteArray(i) is not byte[] array)
                throw new InvalidDataException();
            object data;
            string elementName = "string";
            try
            {
                data = encoding.GetString(array);
            }
            catch (ArgumentException)
            {
                data = BitConverter.ToString(array);
                elementName = "bytes";
            }
            strings[i] = new XElement(elementName, [new XAttribute("index", i), data]);
        }

        for (int i = 0; i < float2s.Length; i++)
        {
            if (table.LmVector2(i) is not LmVector2 v)
                throw new InvalidDataException();

            float2s[i] = new XElement("float2", [new XAttribute("index", i), $"{v.X},{v.Y}"]);
        }

        for (int i = 0; i < float3s.Length; i++)
        {
            if (table.LmVector3(i) is not LmVector3 v)
                throw new InvalidDataException();

            float3s[i] = new XElement("float3", [new XAttribute("index", i), $"{v.X},{v.Y},{v.Z}"]);
        }

        for (int i = 0; i < float4s.Length; i++)
        {
            if (table.LmVector4(i) is not LmVector4 v)
                throw new InvalidDataException();

            float4s[i] = new XElement(
                "float4",
                [new XAttribute("index", i), $"{v.X},{v.Y},{v.Z},{v.W}"]
            );
        }

        for (int i = 0; i < matrices.Length; i++)
        {
            if (table.Matrix44(i) is not Matrix44 m)
                throw new InvalidDataException();

            matrices[i] = new XElement(
                "matrix",
                [
                    new XAttribute("index", i),
                    new XElement("row0", $"{m.Row0.X},{m.Row0.Y},{m.Row0.Z},{m.Row0.W}"),
                    new XElement("row1", $"{m.Row1.X},{m.Row1.Y},{m.Row1.Z},{m.Row1.W}"),
                    new XElement("row2", $"{m.Row2.X},{m.Row2.Y},{m.Row2.Z},{m.Row2.W}"),
                    new XElement("row3", $"{m.Row3.X},{m.Row3.Y},{m.Row3.Z},{m.Row3.W}"),
                ]
            );
        }

        for (int i = 0; i < int32Vectors.Length; i++)
        {
            if (table.Int32Vector(i) is not Int32VectorTable t)
                throw new InvalidDataException();

            XElement[] values = new XElement[t.ValuesLength];
            for (int j = 0; j < values.Length; j++)
            {
                values[j] = new XElement("value", t.Values(j));
            }

            int32Vectors[i] = new XElement("int32Vector", [new XAttribute("index", i), values]);
        }

        return new XElement(
            "data",
            [
                new XElement("booleans", booleans),
                new XElement("ints", ints),
                new XElement("floats", floats),
                new XElement("strings", strings),
                new XElement("float2s", float2s),
                new XElement("float3s", float3s),
                new XElement("float4s", float4s),
                new XElement("matrices", matrices),
                new XElement("int32Vectors", int32Vectors),
            ]
        );
    }

    public static XElement ToXml(string filePath)
    {
        MemoryOwner<byte> buffer;
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            buffer = MemoryOwner<byte>.Allocate((int)stream.Length);
            stream.ReadExactly(buffer.Span);
        }

        FlatBuffer fb = new FlatBuffer(buffer);
        int origin = fb.ReadS32(16);

        var root = VfxGraphRoot.GetRoot(fb, 16 + origin);
        VfxGraphTable graph = root.Graph;
        VfxGraphContainerTable container = graph.GraphContainer;
        GraphPropertyValueTable valueData = graph.ValueData;
        var nodes = NodesToXml(ref container);
        var trays = TraysToXml(ref container);
        var signalLinks = SignalLinksToXml(ref container);
        var propertyLinks = PropertyLinksToXml(ref container);

        XElement graphXml = new XElement("components", [nodes, trays, signalLinks, propertyLinks]);
        XElement graphData = DataToXml(ref valueData);

        XElement versionInfo = new XElement(
            "versionInfo",
            [new XAttribute("self", 1), new XAttribute("tool", 2), new XAttribute("converter", 3)]
        );

        XAttribute treeName = new XAttribute("name", Path.GetFileNameWithoutExtension(filePath));
        XElement treeRoot = new XElement("vfxGraph", [treeName, versionInfo, graphXml, graphData]);
        return treeRoot;
    }

    public static void Main(string[] args)
    {
        XElement root = ToXml(
            "H:/FFXV Debug Build/6575095/datas/effects/mission/titan/02part/vfx/sg_titan_warp_01.vfx"
        );
        root.Save("H:/Project/ff15-vfx/mockup.xml");

        ToBinary("H:/Project/ff15-vfx/mockup.xml");
    }
}
