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
    public static VectorOffset FloatVecToFlat(FlatBufferBuilder builder, XElement xml, int count)
    {
        var elements = xml.Elements().ToArray();
        builder.StartVector(16, elements.Length, 16);
        for (int i = elements.Length - 1; i >= 0; i--)
        {
            if (elements[i] is not XElement e)
                throw new InvalidDataException();
            float[] values = e.Value.Split(',').Select(s => float.Parse(s)).ToArray();
            if (values.Length != count)
                throw new Exception($"Element must contain {count} comma-separated values");

            for (int j = 0; j < (4 - count); j++)
                builder.AddFloat(0.0F);

            for (int j = values.Length - 1; j >= 0; j--)
                builder.AddFloat(values[j]);
        }
        return builder.EndVector();
    }

    public static VectorOffset Matrix44VecToFlat(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements().ToArray();
        builder.StartVector(64, elements.Length, 16);
        for (int i = elements.Length - 1; i >= 0; i--)
        {
            if (elements[i] is not XElement e)
                throw new InvalidDataException();
            var rows = e.Elements().ToArray();

            if (rows.Length != 4)
                throw new Exception("4x4 Matrix must have 4 rows");

            for (int j = 3; j >= 0; j--)
            {
                if (rows[j] is not XElement row)
                    throw new InvalidDataException();

                var values = row.Value.Split(',').Select(s => float.Parse(s)).ToArray();
                if (values.Length != 4)
                    throw new Exception("4x4 Matrix row must have 4 comma-separated values");
                builder.AddFloat(values[3]);
                builder.AddFloat(values[2]);
                builder.AddFloat(values[1]);
                builder.AddFloat(values[0]);
            }
        }

        return builder.EndVector();
    }

    public static VectorOffset IntVectorToFlat(FlatBufferBuilder builder, XElement xml)
    {
        var elements = xml.Elements().ToArray();
        Offset<Int32VectorTable>[] offsets = new Offset<Int32VectorTable>[elements.Length];
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i] is not XElement e)
                throw new InvalidDataException();

            var ints = e.Elements().ToArray();
            builder.StartVector(4, ints.Length, 4);
            for (int j = ints.Length - 1; j >= 0; j--)
            {
                if (ints[j] is not XElement intValue)
                    throw new InvalidDataException();

                builder.AddInt(int.Parse(intValue.Value));
            }

            var off = builder.EndVector();
            builder.StartObject(1);
            builder.AddOffset(0, off.Value, 0);
            offsets[i] = new Offset<Int32VectorTable>(builder.EndObject());
        }

        return builder.CreateVectorOfTables<Int32VectorTable>(offsets);
    }

    public static Offset<VfxLinkDataTable>[] VfxLinkToFlat(FlatBufferBuilder builder, XElement xml)
    {
        var links = xml.Elements().ToArray();
        Offset<VfxLinkDataTable>[] result = new Offset<VfxLinkDataTable>[links.Length];
        for (int i = 0; i < links.Length; i++)
        {
            if (links[i] is not XElement link)
                throw new InvalidDataException();

            if (link.Attribute("instanceId") is not XAttribute instanceId)
                throw new Exception("link must have \"instanceId\" attribute");
            if (link.Attribute("sourceNodeId") is not XAttribute sourceNodeId)
                throw new Exception("link must have \"instanceId\" attribute");
            if (link.Attribute("sourceMemberId") is not XAttribute sourceMemberId)
                throw new Exception("link must have \"instanceId\" attribute");
            if (link.Attribute("destinationNodeId") is not XAttribute destinationNodeId)
                throw new Exception("link must have \"instanceId\" attribute");
            if (link.Attribute("destinationMemberId") is not XAttribute destinationMemberId)
                throw new Exception("link must have \"instanceId\" attribute");
            if (link.Attribute("linkType") is not XAttribute linkType)
                throw new Exception("link must have \"instanceId\" attribute");
            if (link.Attribute("unknown") is not XAttribute unknown)
                throw new Exception("link must have \"instanceId\" attribute");
            result[i] = VfxLinkDataTable.CreateVfxLinkDataTable(
                builder,
                uint.Parse(instanceId.Value),
                uint.Parse(sourceNodeId.Value),
                uint.Parse(sourceMemberId.Value),
                uint.Parse(destinationNodeId.Value),
                uint.Parse(destinationMemberId.Value),
                int.Parse(linkType.Value),
                int.Parse(unknown.Value)
            );
        }
        return result;
    }

    public static void ToBinary(string filePath)
    {
        XElement root = XElement.Load(filePath);
        if ((string?)root.Attribute("name") is string s)
        {
            Console.WriteLine($"Attempting to serialize VFX Graph {s}...");
        }

        // INFO: Output is built backwards
        // 1. Version info struct, graph info table
        // 2. Value Data followed by table
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

        if (data.Element("floats") is not XElement floats)
            throw new InvalidDataException();

        if (data.Element("strings") is not XElement strings)
            throw new InvalidDataException();

        if (data.Element("enums") is not XElement enums)
            throw new InvalidDataException();

        var intElements = ints.Elements();
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

        var floatElements = floats.Elements();
        float[] floatData = new float[floatElements.Count()];
        i = 0;
        foreach (var element in floatElements)
        {
            if (element is not XElement e)
                throw new InvalidDataException();
            floatData[i++] = float.Parse(e.Value);
        }

        var stringElements = strings.Elements();
        StringOffset[] stringData = new StringOffset[stringElements.Count()];
        i = 0;
        foreach (var element in stringElements)
        {
            if (element is not XElement e)
                throw new InvalidDataException();

            stringData[i++] = builder.CreateString(e.Value);
        }

        var enumElements = enums.Elements();
        int[] enumData = new int[enumElements.Count()];
        i = 0;
        foreach (var element in enumElements)
        {
            if (element is not XElement enumEl)
                throw new InvalidDataException();
            enumData[i++] = int.Parse(enumEl.Value);
        }

        var intOff = GraphPropertyValueTable.CreateInt32_Vector(builder, intData);
        var boolOff = GraphPropertyValueTable.CreateBoolean_Vector(builder, boolData);
        var floatOff = GraphPropertyValueTable.CreateFloat32_Vector(builder, floatData);
        var stringOff = GraphPropertyValueTable.CreateString_Vector(builder, stringData);
        var enumOff = GraphPropertyValueTable.CreateInt32_Vector(builder, enumData);

        if (data.Element("float4s") is not XElement float4s)
            throw new InvalidDataException();

        var float4Off = FloatVecToFlat(builder, float4s, 4);

        if (data.Element("float3s") is not XElement float3s)
            throw new InvalidDataException();

        var float3Off = FloatVecToFlat(builder, float3s, 3);

        if (data.Element("float2s") is not XElement float2s)
            throw new InvalidDataException();

        var float2Off = FloatVecToFlat(builder, float2s, 2);

        if (data.Element("matrices") is not XElement matrices)
            throw new InvalidDataException();

        var matricesOff = Matrix44VecToFlat(builder, matrices);

        if (data.Element("int32Vectors") is not XElement int32Vectors)
            throw new InvalidDataException();
        var int32VecOff = IntVectorToFlat(builder, int32Vectors);

        if (root.Element("vfxData") is not XElement vfxData)
            throw new InvalidDataException();
        var fCurveOffsets = VfxFCurveTable.SerializeXml(builder, vfxData.Element("fCurves"));

        // INFO: Step 3: Value data table
        builder.StartObject(42);
        builder.AddOffset(3, intOff.Value, 0);
        builder.AddOffset(9, floatOff.Value, 0);
        builder.AddOffset(0, boolOff.Value, 0);
        builder.AddOffset(11, stringOff.Value, 0);
        builder.AddOffset(12, enumOff.Value, 0); // Enum values
        builder.AddOffset(15, float4Off.Value, 0);
        builder.AddOffset(13, float2Off.Value, 0);
        builder.AddOffset(19, matricesOff.Value, 0);
        builder.AddOffset(14, float3Off.Value, 0);
        builder.AddOffset(25, int32VecOff.Value, 0);
        builder.AddOffset(41, 0, 0);
        builder.AddOffset(40, 0, 0);
        builder.AddOffset(39, 0, 0);
        builder.AddOffset(38, 0, 0);
        builder.AddOffset(37, 0, 0);
        builder.AddOffset(36, 0, 0); // Vector4 array
        builder.AddOffset(35, 0, 0); // Vector3 array
        builder.AddOffset(34, 0, 0); // Vector3 array
        builder.AddOffset(33, 0, 0);
        builder.AddOffset(32, 0, 0);
        builder.AddOffset(31, 0, 0);
        builder.AddOffset(30, 0, 0); // Float arrays
        builder.AddOffset(29, 0, 0);
        builder.AddOffset(28, 0, 0);
        builder.AddOffset(27, 0, 0);
        builder.AddOffset(26, 0, 0);
        builder.AddOffset(24, 0, 0);
        builder.AddOffset(23, 0, 0);
        builder.AddOffset(22, 0, 0);
        builder.AddOffset(21, 0, 0);
        builder.AddOffset(20, 0, 0);
        builder.AddOffset(18, 0, 0);
        builder.AddOffset(17, 0, 0);
        builder.AddOffset(16, 0, 0);
        builder.AddOffset(10, 0, 0);
        builder.AddOffset(8, 0, 0);
        builder.AddOffset(7, 0, 0);
        builder.AddOffset(6, 0, 0);
        builder.AddOffset(5, 0, 0);
        builder.AddOffset(4, 0, 0);
        builder.AddOffset(2, 0, 0);
        builder.AddOffset(1, 0, 0);
        var valueTableOffset = new Offset<GraphPropertyValueTable>(builder.EndObject());

        // INFO: Step 4: VFX-Specific Value table
        VectorOffset fCurveTables = fCurveOffsets is null
            ? new VectorOffset(0)
            : builder.CreateVectorOfTables<VfxFCurveTable>(fCurveOffsets);

        builder.StartObject(7);
        builder.AddOffset(6, fCurveTables.Value, 0);
        var vfxValueTableOffset = new Offset<VfxTimelineDataTable>(builder.EndObject());

        if (root.Element("components") is not XElement components)
            throw new InvalidDataException();
        // INFO: Step 5: Nodes

        var baseTypeDict = NodeMetadata.VfxBaseTypeNames.ToDictionary(x => x.Value, x => x.Key);
        if (components.Element("nodes") is not XElement nodes)
            throw new InvalidDataException();
        var nodeElements = nodes.Elements().ToArray();
        Offset<VfxNodeDataTable>[] nodeTables = new Offset<VfxNodeDataTable>[nodeElements.Length];
        i = 0;
        foreach (var element in nodeElements)
        {
            if (element is not XElement node)
                throw new InvalidDataException();
            if (node.Element("properties") is not XElement properties)
                throw new InvalidDataException();
            var propElements = properties.Elements().ToArray();
            var propTables = new Offset<VfxPropertyDataTable>[propElements.Length];
            for (int j = 0; j < propElements.Length; j++)
            {
                if (propElements[j] is not XElement prop)
                    throw new InvalidDataException();

                if (prop.Attribute("memberId") is not XAttribute memberId)
                    throw new Exception("property must have \"memberId\" attribute");
                if (prop.Attribute("name") is not XAttribute name)
                    throw new Exception("property must have \"name\" attribute");
                if (prop.Attribute("valueTable") is not XAttribute valueTable)
                    throw new Exception("property must have \"valueTable\" attribute");
                if (prop.Attribute("baseType") is not XAttribute baseType)
                    throw new Exception("property must have \"baseType\" attribute");
                if (prop.Attribute("valueRef") is not XAttribute valueRef)
                    throw new Exception("property must have \"valueRef\" attribute");

                builder.StartObject(3);
                var identityOffset = VfxMemberIdTable.CreateVfxMemberIdTable(
                    builder,
                    uint.Parse(memberId.Value),
                    uint.Parse(name.Value)
                );

                builder.AddStruct(0, identityOffset.Value, 0);

                var valueRefOffset = VfxPropertyValueTable.CreateVfxPropertyValueTable(
                    builder,
                    baseTypeDict[baseType.Value],
                    int.Parse(valueRef.Value)
                );
                builder.AddStruct(2, valueRefOffset.Value, 0);

                builder.AddInt(1, int.Parse(valueTable.Value), 0);

                propTables[j] = new Offset<VfxPropertyDataTable>(builder.EndObject());
            }

            if (node.Element("inputPorts") is not XElement inputPorts)
                throw new InvalidDataException();
            var inPortElements = inputPorts.Elements().ToArray();
            var inPortTables = new Offset<VfxSignalDataTable>[inPortElements.Length];
            for (int j = 0; j < inPortElements.Length; j++)
            {
                if (inPortElements[j] is not XElement inPort)
                    throw new InvalidDataException();
                if (inPort.Attribute("memberId") is not XAttribute memberId)
                    throw new Exception("ports must have \"memberId\" attribute");
                if (inPort.Attribute("name") is not XAttribute name)
                    throw new Exception("ports must have \"name\" attribute");
                if (inPort.Attribute("argumentTypes") is not XAttribute argumentTypes)
                    throw new Exception("ports must have \"argumentTypes\" attribute");

                var identityOffset = VfxMemberIdTable.CreateVfxMemberIdTable(
                    builder,
                    uint.Parse(memberId.Value),
                    uint.Parse(name.Value)
                );

                builder.StartObject(2);
                builder.AddStruct(0, identityOffset.Value, 0);
                builder.AddInt(1, int.Parse(argumentTypes.Value), 0);
                inPortTables[j] = new Offset<VfxSignalDataTable>(builder.EndObject());
            }

            if (node.Element("outputPorts") is not XElement outputPorts)
                throw new InvalidDataException();
            var outPortElements = outputPorts.Elements().ToArray();
            var outPortTables = new Offset<VfxSignalDataTable>[outPortElements.Length];
            for (int j = 0; j < outPortElements.Length; j++)
            {
                if (outPortElements[j] is not XElement outPort)
                    throw new InvalidDataException();
                if (outPort.Attribute("memberId") is not XAttribute memberId)
                    throw new Exception("ports must have \"memberId\" attribute");
                if (outPort.Attribute("name") is not XAttribute name)
                    throw new Exception("ports must have \"name\" attribute");
                if (outPort.Attribute("argumentTypes") is not XAttribute argumentTypes)
                    throw new Exception("ports must have \"argumentTypes\" attribute");

                var identityOffset = VfxMemberIdTable.CreateVfxMemberIdTable(
                    builder,
                    uint.Parse(memberId.Value),
                    uint.Parse(name.Value)
                );

                builder.StartObject(2);
                builder.AddStruct(0, identityOffset.Value, 0);
                builder.AddInt(1, int.Parse(argumentTypes.Value), 0);
                outPortTables[j] = new Offset<VfxSignalDataTable>(builder.EndObject());
            }

            builder.StartVector(4, propTables.Length, 4);
            for (int j = propTables.Length - 1; j >= 0; j--)
                builder.AddOffset(propTables[j].Value);
            var propTablesOffset = builder.EndVector();

            builder.StartVector(4, inPortTables.Length, 4);
            for (int j = inPortTables.Length - 1; j >= 0; j--)
                builder.AddOffset(inPortTables[j].Value);
            var inPortsOffset = builder.EndVector();

            builder.StartVector(4, outPortTables.Length, 4);
            for (int j = outPortTables.Length - 1; j >= 0; j--)
                builder.AddOffset(outPortTables[j].Value);
            var outPortsOffset = builder.EndVector();

            if (node.Attribute("instanceId") is not XAttribute instanceId)
                throw new Exception("node must have \"instanceId\" attribute");
            if (node.Attribute("typeId") is not XAttribute typeId)
                throw new Exception("node must have \"typeId\" attribute");
            var nodeIdentityOffset = VfxNodeIdentityStruct.CreateVfxNodeIdentityStruct(
                builder,
                uint.Parse(instanceId.Value),
                uint.Parse(typeId.Value)
            );

            builder.StartObject(4);
            builder.AddStruct(0, nodeIdentityOffset.Value, 0);
            builder.AddOffset(1, propTablesOffset.Value, 0);
            builder.AddOffset(2, inPortsOffset.Value, 0);
            builder.AddOffset(3, outPortsOffset.Value, 0);
            nodeTables[i++] = new Offset<VfxNodeDataTable>(builder.EndObject());
        }

        // INFO: Step 6: Trays
        if (components.Element("trays") is not XElement trays)
            throw new InvalidDataException();
        var trayElements = trays.Elements().ToArray();
        var trayTables = new Offset<VfxTrayDataTable>[trayElements.Length];
        for (i = 0; i < trayElements.Length; i++)
        {
            if (trayElements[i] is not XElement tray)
                throw new InvalidDataException();
            if (tray.Element("nodes") is not XElement childNodes)
                throw new InvalidDataException();

            var childElements = childNodes.Elements().ToArray();
            var childReferences = new uint[childElements.Length];
            for (int j = 0; j < childElements.Length; j++)
            {
                if (childElements[j] is not XElement reference)
                    throw new InvalidDataException();

                if (reference.Attribute("instanceId") is not XAttribute instanceId)
                    throw new Exception("references must have \"instanceId\" attribute");

                childReferences[j] = uint.Parse(instanceId.Value);
            }
            if (tray.Attribute("instanceId") is not XAttribute trayInstanceId)
                throw new Exception("tray must have \"instanceId\" attribute");
            if (tray.Attribute("typeId") is not XAttribute typeId)
                throw new Exception("tray must have \"typeId\" attribute");

            trayTables[i] = VfxTrayDataTable.CreateVfxTrayDataTable(
                builder,
                uint.Parse(trayInstanceId.Value),
                uint.Parse(typeId.Value),
                childReferences
            );
        }

        // INFO: Step 8: Property links
        if (components.Element("propertyLinks") is not XElement propertyLinks)
            throw new InvalidDataException();
        var propertyLinkOffsets = VfxLinkToFlat(builder, propertyLinks);

        // INFO: Step 7: Signal links
        if (components.Element("signalLinks") is not XElement signalLinks)
            throw new InvalidDataException();
        var signalLinkOffsets = VfxLinkToFlat(builder, signalLinks);

        // Step 9: Node Offset Vector
        builder.StartVector(4, nodeTables.Length, 4);
        for (i = nodeTables.Length - 1; i >= 0; i--)
            builder.AddOffset(nodeTables[i].Value);
        var nodeTablesOffset = builder.EndVector();
        // Step 10: Tray Offset Vector
        builder.StartVector(4, trayTables.Length, 4);
        for (i = trayTables.Length - 1; i >= 0; i--)
            builder.AddOffset(trayTables[i].Value);
        var trayTablesOffset = builder.EndVector();
        // Step 11: Property Link Offset Vector
        builder.StartVector(4, propertyLinkOffsets.Length, 4);
        for (i = propertyLinkOffsets.Length - 1; i >= 0; i--)
            builder.AddOffset(propertyLinkOffsets[i].Value);
        var propertyLinkTablesOffset = builder.EndVector();
        // Step 12: Signal Link Offset Vector
        builder.StartVector(4, signalLinkOffsets.Length, 4);
        for (i = signalLinkOffsets.Length - 1; i >= 0; i--)
            builder.AddOffset(signalLinkOffsets[i].Value);
        var signalLinkTablesOffset = builder.EndVector();
        // Step 13: Graph Container Table
        builder.StartObject(4);
        builder.AddOffset(3, propertyLinkTablesOffset.Value, 0);
        builder.AddOffset(2, signalLinkTablesOffset.Value, 0);
        builder.AddOffset(1, trayTablesOffset.Value, 0);
        builder.AddOffset(0, nodeTablesOffset.Value, 0);
        var containerTableOffset = new Offset<VfxGraphContainerTable>(builder.EndObject());

        // Step 14: Graph Table
        builder.StartObject(3);
        builder.AddOffset(2, valueTableOffset.Value, 0);
        builder.AddOffset(1, containerTableOffset.Value, 0);
        builder.AddOffset(0, graphInfoOffset.Value, 0);
        var graphTableOffset = new Offset<VfxGraphTable>(builder.EndObject());

        // Step 15: Root table
        builder.StartObject(2);
        builder.AddOffset(1, vfxValueTableOffset.Value, 0);
        builder.AddOffset(0, graphTableOffset.Value, 0);
        var rootTable = new Offset<VfxGraphRoot>(builder.EndObject());

        builder.FinishVfx(rootTable.Value, "LMVG");

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
        XElement[] enums = new XElement[table.EnumLength];
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

        for (int i = 0; i < enums.Length; i++)
            enums[i] = new XElement("enum", [new XAttribute("index", i), table.Enum(i)]);

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
                new XElement("enums", enums),
                new XElement("float2s", float2s),
                new XElement("float3s", float3s),
                new XElement("float4s", float4s),
                new XElement("matrices", matrices),
                new XElement("int32Vectors", int32Vectors),
            ]
        );
    }

    public static XElement VfxDataToXml(VfxTimelineDataTable? table)
    {
        if (table is not VfxTimelineDataTable t)
            return new XElement("vfxData");

        XElement[] FCurves = new XElement[t.CurvesLength];
        for (int i = 0; i < FCurves.Length; i++)
        {
            if (t.Curves(i) is not VfxFCurveTable curveTable)
                throw new InvalidDataException();

            XElement[] anchorPoints = new XElement[curveTable.KeysLength];
            for (int j = 0; j < anchorPoints.Length; j++)
            {
                if (curveTable.Keys(j) is not LmVector4 anchorPoint)
                    throw new InvalidDataException();

                anchorPoints[j] = new XElement(
                    "anchorPoint",
                    [
                        new XAttribute("time", $"{anchorPoint.X}"),
                        $"{anchorPoint.Y},{anchorPoint.Z},{anchorPoint.W}",
                    ]
                );
            }

            FCurves[i] = new XElement(
                "fCurve",
                [
                    new XAttribute("loopIn", curveTable.LoopIn),
                    new XAttribute("loopOut", curveTable.LoopOut),
                    new XAttribute("loopType", curveTable.CurveLoopType),
                    new XElement("anchorPoints", anchorPoints),
                ]
            );
        }
        return new XElement("vfxData", [new XElement("fCurves", FCurves)]);
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
        var vfxData = root.Timeline;

        var nodes = NodesToXml(ref container);
        var trays = TraysToXml(ref container);
        var signalLinks = SignalLinksToXml(ref container);
        var propertyLinks = PropertyLinksToXml(ref container);
        var vfxDataElements = VfxDataToXml(vfxData);

        XElement graphXml = new XElement("components", [nodes, trays, signalLinks, propertyLinks]);
        XElement graphData = DataToXml(ref valueData);

        XElement versionInfo = new XElement(
            "versionInfo",
            [new XAttribute("self", 1), new XAttribute("tool", 2), new XAttribute("converter", 3)]
        );

        XAttribute treeName = new XAttribute("name", Path.GetFileNameWithoutExtension(filePath));
        XElement treeRoot = new XElement(
            "vfxGraph",
            [treeName, versionInfo, graphXml, graphData, vfxDataElements]
        );
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
