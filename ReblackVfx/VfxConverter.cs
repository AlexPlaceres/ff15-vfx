using System.Xml.Linq;
using Google.FlatBuffers;
using ReblackVfx.FlatBuffer.BaseType;
using ReblackVfx.FlatBuffer.Graph;
using ReblackVfx.FlatBuffer.Math;
using ReblackVfx.FlatBuffer.VfxGraph;

namespace ReblackVfx;

public static class VfxConverter
{
    public static Offset<GraphContainer> SerializeGraphContainer(
        FlatBufferBuilder builder,
        XElement nodes,
        XElement trays,
        XElement signalLinks,
        XElement propertyLinks
    )
    {
        VectorOffset nodesOffset = default(VectorOffset);
        VectorOffset traysOffset = default(VectorOffset);
        VectorOffset signalLinksOffset = default(VectorOffset);
        VectorOffset propertyLinksOffset = default(VectorOffset);

        Offset<GraphNodeData>[] nodeOffsets = new Offset<GraphNodeData>[0];
        if (nodes.HasElements)
        {
            var typeValDict = NodeMetadata.VfxBaseTypeNames.ToDictionary(x => x.Value, x => x.Key);

            XElement[] elements = nodes.Elements().ToArray();
            nodeOffsets = new Offset<GraphNodeData>[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                if (
                    elements[i] is not XElement node
                    || node.Attribute("instanceId") is not XAttribute instanceId
                    || node.Attribute("typeId") is not XAttribute typeId
                    || node.Element("properties") is not XElement properties
                    || node.Element("inPorts") is not XElement inPorts
                    || node.Element("outPorts") is not XElement outPorts
                )
                    throw new InvalidDataException();

                XElement[] propElements = properties.Elements().ToArray();
                Offset<GraphNodeProperty>[] propOffsets = new Offset<GraphNodeProperty>[
                    propElements.Length
                ];

                for (int j = 0; j < propElements.Length; j++)
                {
                    if (
                        propElements[j] is not XElement propElement
                        || propElement.Attribute("memberId") is not XAttribute memberId
                        || propElement.Attribute("nameHash") is not XAttribute nameHash
                        || propElement.Attribute("valueTable") is not XAttribute valueType
                        || propElement.Attribute("baseType") is not XAttribute baseType
                        || propElement.Attribute("valueIndex") is not XAttribute valueIndex
                    )
                        throw new InvalidDataException();

                    GraphNodeProperty.StartGraphNodeProperty(builder);
                    Offset<GraphNodeMemberBase> memberBase =
                        GraphNodeMemberBase.CreateGraphNodeMemberBase(
                            builder,
                            uint.Parse(memberId.Value),
                            uint.Parse(nameHash.Value)
                        );
                    GraphNodeProperty.AddIdentity(builder, memberBase);

                    Offset<Value> valueRef = Value.CreateValue(
                        builder,
                        typeValDict[baseType.Value],
                        int.Parse(valueIndex.Value)
                    );

                    GraphNodeProperty.AddValueReference(builder, valueRef);
                    GraphNodeProperty.AddValueTable(builder, int.Parse(valueType.Value));
                    propOffsets[j] = GraphNodeProperty.EndGraphNodeProperty(builder);
                }

                XElement[] inPortElements = inPorts.Elements().ToArray();
                Offset<GraphNodePort>[] inPortOffsets = new Offset<GraphNodePort>[
                    inPortElements.Length
                ];
                for (int j = 0; j < inPortElements.Length; j++)
                {
                    if (
                        inPortElements[j] is not XElement portElement
                        || portElement.Attribute("memberId") is not XAttribute memberId
                        || portElement.Attribute("nameHash") is not XAttribute nameHash
                        || portElement.Attribute("argumentTypes") is not XAttribute argumentTypes
                    )
                        throw new InvalidDataException();

                    Offset<GraphNodeMemberBase> memberBase =
                        GraphNodeMemberBase.CreateGraphNodeMemberBase(
                            builder,
                            uint.Parse(memberId.Value),
                            uint.Parse(nameHash.Value)
                        );

                    GraphNodePort.StartGraphNodePort(builder);
                    GraphNodePort.AddIdentity(builder, memberBase);
                    GraphNodePort.AddArgumentTypes(builder, int.Parse(argumentTypes.Value));
                    inPortOffsets[j] = GraphNodePort.EndGraphNodePort(builder);
                }

                XElement[] outPortElements = outPorts.Elements().ToArray();
                Offset<GraphNodePort>[] outPortOffsets = new Offset<GraphNodePort>[
                    outPortElements.Length
                ];
                for (int j = 0; j < outPortElements.Length; j++)
                {
                    if (
                        outPortElements[j] is not XElement portElement
                        || portElement.Attribute("memberId") is not XAttribute memberId
                        || portElement.Attribute("nameHash") is not XAttribute nameHash
                        || portElement.Attribute("argumentTypes") is not XAttribute argumentTypes
                    )
                        throw new InvalidDataException();

                    Offset<GraphNodeMemberBase> memberBase =
                        GraphNodeMemberBase.CreateGraphNodeMemberBase(
                            builder,
                            uint.Parse(memberId.Value),
                            uint.Parse(nameHash.Value)
                        );

                    GraphNodePort.StartGraphNodePort(builder);
                    GraphNodePort.AddIdentity(builder, memberBase);
                    GraphNodePort.AddArgumentTypes(builder, int.Parse(argumentTypes.Value));
                    outPortOffsets[j] = GraphNodePort.EndGraphNodePort(builder);
                }

                VectorOffset propsVecOffset = properties.HasElements
                    ? GraphNodeData.CreatePropertiesVector(builder, propOffsets)
                    : new VectorOffset(0);

                VectorOffset inPortsVecOffset = inPorts.HasElements
                    ? GraphNodeData.CreateInPortsVector(builder, inPortOffsets)
                    : new VectorOffset(0);

                VectorOffset outPortsVecOffset = outPorts.HasElements
                    ? GraphNodeData.CreateOutPortsVector(builder, outPortOffsets)
                    : new VectorOffset(0);

                Offset<GraphNodeDataBase> identity = GraphNodeDataBase.CreateGraphNodeDataBase(
                    builder,
                    uint.Parse(instanceId.Value),
                    uint.Parse(typeId.Value)
                );

                GraphNodeData.StartGraphNodeData(builder);
                GraphNodeData.AddIdentity(builder, identity);
                GraphNodeData.AddProperties(builder, propsVecOffset);
                GraphNodeData.AddInPorts(builder, inPortsVecOffset);
                GraphNodeData.AddOutPorts(builder, outPortsVecOffset);
                nodeOffsets[i] = GraphNodeData.EndGraphNodeData(builder);
            }
        }

        Offset<GraphNodeTray>[] trayOffsets = new Offset<GraphNodeTray>[0];
        if (trays.HasElements)
        {
            XElement[] elements = trays.Elements().ToArray();
            trayOffsets = new Offset<GraphNodeTray>[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                if (
                    elements[i] is not XElement tray
                    || tray.Attribute("instanceId") is not XAttribute instanceId
                    || tray.Attribute("typeId") is not XAttribute typeId
                    || tray.Element("references") is not XElement nodeReferences
                )
                    throw new InvalidDataException();

                XElement[] referenceElements = nodeReferences.Elements().ToArray();
                uint[] references = new uint[referenceElements.Length];
                for (int j = 0; j < referenceElements.Length; j++)
                {
                    if (
                        referenceElements[j] is not XElement reference
                        || reference.Attribute("instanceId") is not XAttribute refInstanceId
                    )
                        throw new InvalidDataException();
                    references[j] = uint.Parse(refInstanceId.Value);
                }

                VectorOffset referencesOffset = GraphNodeTray.CreateReferencesVector(
                    builder,
                    references
                );

                trayOffsets[i] = GraphNodeTray.CreateGraphNodeTray(
                    builder,
                    uint.Parse(instanceId.Value),
                    uint.Parse(typeId.Value),
                    referencesOffset
                );
            }
        }

        Offset<GraphLinkData>[] propertyLinkOffsets = new Offset<GraphLinkData>[0];
        if (propertyLinks.HasElements)
        {
            XElement[] elements = propertyLinks.Elements().ToArray();
            propertyLinkOffsets = new Offset<GraphLinkData>[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                if (
                    elements[i] is not XElement link
                    || link.Attribute("instanceId") is not XAttribute instanceId
                    || link.Attribute("sourceNodeId") is not XAttribute sourceNodeId
                    || link.Attribute("sourceMemberId") is not XAttribute sourceMemberId
                    || link.Attribute("destinationNodeId") is not XAttribute destinationNodeId
                    || link.Attribute("destinationMemberId") is not XAttribute destinationMemberId
                    || link.Attribute("linkType") is not XAttribute linkType
                    || link.Attribute("unknown") is not XAttribute unknown
                )
                    throw new InvalidDataException();

                propertyLinkOffsets[i] = GraphLinkData.CreateGraphLinkData(
                    builder,
                    int.Parse(instanceId.Value),
                    int.Parse(sourceNodeId.Value),
                    int.Parse(sourceMemberId.Value),
                    int.Parse(destinationNodeId.Value),
                    int.Parse(destinationMemberId.Value),
                    int.Parse(linkType.Value),
                    int.Parse(unknown.Value)
                );
            }
        }

        Offset<GraphLinkData>[] signalLinkOffsets = new Offset<GraphLinkData>[0];
        if (signalLinks.HasElements)
        {
            XElement[] elements = signalLinks.Elements().ToArray();
            signalLinkOffsets = new Offset<GraphLinkData>[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                if (
                    elements[i] is not XElement link
                    || link.Attribute("instanceId") is not XAttribute instanceId
                    || link.Attribute("sourceNodeId") is not XAttribute sourceNodeId
                    || link.Attribute("sourceMemberId") is not XAttribute sourceMemberId
                    || link.Attribute("destinationNodeId") is not XAttribute destinationNodeId
                    || link.Attribute("destinationMemberId") is not XAttribute destinationMemberId
                    || link.Attribute("linkType") is not XAttribute linkType
                    || link.Attribute("unknown") is not XAttribute unknown
                )
                    throw new InvalidDataException();

                signalLinkOffsets[i] = GraphLinkData.CreateGraphLinkData(
                    builder,
                    int.Parse(instanceId.Value),
                    int.Parse(sourceNodeId.Value),
                    int.Parse(sourceMemberId.Value),
                    int.Parse(destinationNodeId.Value),
                    int.Parse(destinationMemberId.Value),
                    int.Parse(linkType.Value),
                    int.Parse(unknown.Value)
                );
            }
        }

        nodesOffset = GraphContainer.CreateNodesVector(builder, nodeOffsets);

        traysOffset = GraphContainer.CreateTraysVector(builder, trayOffsets);

        propertyLinksOffset = GraphContainer.CreatePropertyLinksVector(
            builder,
            propertyLinkOffsets
        );

        signalLinksOffset = GraphContainer.CreateSignalLinksVector(builder, signalLinkOffsets);

        return GraphContainer.CreateGraphContainer(
            builder,
            nodesOffset: nodesOffset,
            traysOffset: traysOffset,
            signal_linksOffset: signalLinksOffset,
            property_linksOffset: propertyLinksOffset
        );
    }

    public static void SerializeTypeData(
        FlatBufferBuilder builder,
        XElement baseTypeData,
        XElement vfxTypeData,
        out Offset<GraphBaseTypeData> outBaseTypeData,
        out Offset<VfxTypeData> outVfxTypeData
    )
    {
        VectorOffset byteDataOffset = default(VectorOffset);
        VectorOffset int32DataOffset = default(VectorOffset);
        VectorOffset floatDataOffset = default(VectorOffset);
        VectorOffset stringDataOffset = default(VectorOffset);
        VectorOffset enumDataOffset = default(VectorOffset);
        VectorOffset vector2DataOffset = default(VectorOffset);
        VectorOffset vector3DataOffset = default(VectorOffset);
        VectorOffset vector4DataOffset = default(VectorOffset);
        VectorOffset matrix44DataOffset = default(VectorOffset);
        VectorOffset int32ArrayDataOffset = default(VectorOffset);
        VectorOffset floatArrayDataOffset = default(VectorOffset);
        VectorOffset vector3ArrayDataOffset = default(VectorOffset);

        if (baseTypeData.Element("int32Data") is XElement int32Data && int32Data.HasElements)
        {
            XElement[] elements = int32Data.Elements().ToArray();
            int[] values = new int[elements.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();

                values[i] = int.Parse(value.Value);
            }

            int32DataOffset = GraphBaseTypeData.CreateInt32DataVector(builder, values);
        }

        if (baseTypeData.Element("enumData") is XElement enumData && enumData.HasElements)
        {
            XElement[] elements = enumData.Elements().ToArray();
            int[] values = new int[elements.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();

                values[i] = int.Parse(value.Value);
            }

            enumDataOffset = GraphBaseTypeData.CreateEnumDataVector(builder, values);
        }

        if (baseTypeData.Element("stringData") is XElement stringData && stringData.HasElements)
        {
            XElement[] elements = stringData.Elements().ToArray();
            StringOffset[] values = new StringOffset[elements.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();

                values[i] = builder.CreateString(value.Value);
            }

            stringDataOffset = GraphBaseTypeData.CreateStringDataVector(builder, values);
        }

        if (baseTypeData.Element("floatData") is XElement floatData && floatData.HasElements)
        {
            XElement[] elements = floatData.Elements().ToArray();
            float[] values = new float[elements.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();

                values[i] = float.Parse(value.Value);
            }

            floatDataOffset = GraphBaseTypeData.CreateFloatDataVector(builder, values);
        }

        if (baseTypeData.Element("byteData") is XElement byteData && byteData.HasElements)
        {
            XElement[] elements = byteData.Elements().ToArray();
            sbyte[] bytes = new sbyte[elements.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();

                bytes[i] = sbyte.Parse(value.Value);
            }

            byteDataOffset = GraphBaseTypeData.CreateByteDataVector(builder, bytes);
        }

        if (
            baseTypeData.Element("floatArrayData") is XElement floatArrayData
            && floatArrayData.HasElements
        )
        {
            XElement[] elements = floatArrayData.Elements().ToArray();
            Offset<FloatArrayTable>[] values = new Offset<FloatArrayTable>[elements.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (elements[i] is not XElement array)
                    throw new InvalidDataException();

                XElement[] arrayElements = array.Elements().ToArray();
                float[] arrayValues = new float[arrayElements.Length];
                for (int j = 0; j < arrayValues.Length; j++)
                {
                    if (arrayElements[j] is not XElement arrayValue)
                        throw new InvalidDataException();
                    arrayValues[j] = float.Parse(arrayValue.Value);
                }

                VectorOffset valuesOffset = FloatArrayTable.CreateDataVector(builder, arrayValues);
                values[i] = FloatArrayTable.CreateFloatArrayTable(builder, valuesOffset);
            }

            floatArrayDataOffset = GraphBaseTypeData.CreateFloatArrayDataVector(builder, values);
        }

        if (
            baseTypeData.Element("int32ArrayData") is XElement int32ArrayData
            && int32ArrayData.HasElements
        )
        {
            XElement[] elements = int32ArrayData.Elements().ToArray();
            Offset<Int32ArrayTable>[] values = new Offset<Int32ArrayTable>[elements.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (elements[i] is not XElement array)
                    throw new InvalidDataException();

                XElement[] arrayElements = array.Elements().ToArray();
                int[] arrayValues = new int[arrayElements.Length];
                for (int j = 0; j < arrayValues.Length; j++)
                {
                    if (arrayElements[j] is not XElement arrayValue)
                        throw new InvalidDataException();
                    arrayValues[j] = int.Parse(arrayValue.Value);
                }

                VectorOffset valuesOffset = Int32ArrayTable.CreateDataVector(builder, arrayValues);
                values[i] = Int32ArrayTable.CreateInt32ArrayTable(builder, valuesOffset);
            }

            int32ArrayDataOffset = GraphBaseTypeData.CreateInt32ArrayDataVector(builder, values);
        }

        if (baseTypeData.Element("vector2Data") is XElement vector2Data && vector2Data.HasElements)
        {
            XElement[] elements = vector2Data.Elements().ToArray();
            builder.StartVector(16, elements.Length, 16);
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();

                var v = value.Value.Split(",").Select(s => float.Parse(s)).ToArray();
                if (v.Length != 2)
                    throw new Exception();

                builder.AddFloat(0.0F);
                builder.AddFloat(0.0F);
                builder.AddFloat(v[1]);
                builder.AddFloat(v[0]);
            }

            vector2DataOffset = builder.EndVector();
        }

        if (baseTypeData.Element("vector3Data") is XElement vector3Data && vector3Data.HasElements)
        {
            XElement[] elements = vector3Data.Elements().ToArray();
            builder.StartVector(16, elements.Length, 16);
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();

                var v = value.Value.Split(",").Select(s => float.Parse(s)).ToArray();
                if (v.Length != 3)
                    throw new Exception();

                builder.AddFloat(0.0F);
                builder.AddFloat(v[2]);
                builder.AddFloat(v[1]);
                builder.AddFloat(v[0]);
            }

            vector3DataOffset = builder.EndVector();
        }

        if (baseTypeData.Element("vector4Data") is XElement vector4Data && vector4Data.HasElements)
        {
            XElement[] elements = vector4Data.Elements().ToArray();
            builder.StartVector(16, elements.Length, 16);
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();

                var v = value.Value.Split(",").Select(s => float.Parse(s)).ToArray();
                if (v.Length != 4)
                    throw new Exception();

                builder.AddFloat(v[3]);
                builder.AddFloat(v[2]);
                builder.AddFloat(v[1]);
                builder.AddFloat(v[0]);
            }

            vector4DataOffset = builder.EndVector();
        }

        if (
            baseTypeData.Element("matrix44Data") is XElement matrix44Data
            && matrix44Data.HasElements
        )
        {
            XElement[] elements = matrix44Data.Elements().ToArray();
            builder.StartVector(64, elements.Length, 16);
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] is not XElement value)
                    throw new InvalidDataException();
                XElement[] xmlRows = value.Elements().ToArray();
                if (xmlRows.Length != 4)
                    throw new Exception();

                float[][] rows = new float[4][];
                for (int j = 0; j < xmlRows.Length; j++)
                {
                    var v = xmlRows[j].Value.Split(",").Select(s => float.Parse(s)).ToArray();
                    if (v.Length != 4)
                        throw new Exception();

                    rows[j] = v;
                }

                _ = Matrix44.CreateMatrix44(
                    builder,
                    rows[0][0],
                    rows[0][1],
                    rows[0][2],
                    rows[0][3],
                    rows[1][0],
                    rows[1][1],
                    rows[1][2],
                    rows[1][3],
                    rows[2][0],
                    rows[2][1],
                    rows[2][2],
                    rows[2][3],
                    rows[3][0],
                    rows[3][1],
                    rows[3][2],
                    rows[3][3]
                );
            }

            matrix44DataOffset = builder.EndVector();
        }

        if (
            baseTypeData.Element("vector3ArrayData") is XElement vector3ArrayData
            && vector3ArrayData.HasElements
        )
        {
            XElement[] elements = vector3ArrayData.Elements().ToArray();
            Offset<Vector3ArrayTable>[] tables = new Offset<Vector3ArrayTable>[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] is not XElement vector3Array)
                    throw new InvalidDataException();

                XElement[] vec3Elements = vector3Array.Elements().ToArray();
                builder.StartVector(16, vec3Elements.Length, 16);
                for (int j = vec3Elements.Length - 1; j >= 0; j--)
                {
                    if (vec3Elements[j] is not XElement vec3)
                        throw new InvalidDataException();
                    float[] v = vec3.Value.Split(",").Select(s => float.Parse(s)).ToArray();
                    Vector4.CreateVector4(builder, v[0], v[1], v[2], v[3]);
                }
                tables[i] = Vector3ArrayTable.CreateVector3ArrayTable(builder, builder.EndVector());
            }

            vector3ArrayDataOffset = GraphBaseTypeData.CreateVector3ArrayDataVector(
                builder,
                tables
            );
        }

        VectorOffset fCurveDataOffset = default(VectorOffset);
        if (vfxTypeData.Element("fCurves") is XElement fCurves && fCurves.HasElements)
        {
            XElement[] elements = fCurves.Elements().ToArray();
            Offset<VfxFCurveData>[] offsets = new Offset<VfxFCurveData>[elements.Length];
            for (int i = 0; i < offsets.Length; i++)
            {
                if (
                    elements[i] is not XElement fCurve
                    || fCurve.Attribute("loopIn") is not XAttribute loopIn
                    || fCurve.Attribute("loopOut") is not XAttribute loopOut
                    || fCurve.Attribute("loopType") is not XAttribute loopType
                    || fCurve.Element("anchorPoints") is not XElement anchorPoints
                )
                    throw new InvalidDataException();

                XElement[] anchorPointElements = anchorPoints.Elements().ToArray();
                builder.StartVector(16, anchorPointElements.Length, 16);
                for (int j = anchorPointElements.Length - 1; j >= 0; j--)
                {
                    if (
                        anchorPointElements[j] is not XElement anchorPoint
                        || anchorPoint.Attribute("time") is not XAttribute time
                    )
                        throw new InvalidDataException();

                    float[] v = anchorPoint.Value.Split(",").Select(s => float.Parse(s)).ToArray();
                    if (v.Length != 3)
                        throw new Exception();

                    FCurveAnchorPoint.CreateFCurveAnchorPoint(
                        builder,
                        float.Parse(time.Value),
                        v[0],
                        v[1],
                        v[2]
                    );
                }

                VectorOffset anchorPointsOffset = builder.EndVector();
                offsets[i] = VfxFCurveData.CreateVfxFCurveData(
                    builder,
                    int.Parse(loopIn.Value),
                    int.Parse(loopOut.Value),
                    int.Parse(loopType.Value),
                    anchorPointsOffset
                );
            }
            fCurveDataOffset = VfxTypeData.CreateFCurveDataVector(builder, offsets);
        }

        outBaseTypeData = GraphBaseTypeData.CreateGraphBaseTypeData(
            builder,
            byte_dataOffset: byteDataOffset,
            int32_dataOffset: int32DataOffset,
            float_dataOffset: floatDataOffset,
            string_dataOffset: stringDataOffset,
            enum_dataOffset: enumDataOffset,
            vector2_dataOffset: vector2DataOffset,
            vector3_dataOffset: vector3DataOffset,
            vector4_dataOffset: vector4DataOffset,
            matrix44_dataOffset: matrix44DataOffset,
            int32_array_dataOffset: int32ArrayDataOffset,
            float_array_dataOffset: floatArrayDataOffset,
            vector3_array_dataOffset: vector3ArrayDataOffset
        );

        outVfxTypeData = VfxTypeData.CreateVfxTypeData(
            builder,
            f_curve_dataOffset: fCurveDataOffset
        );
    }

    public static void CreateVfxBinary(FlatBufferBuilder builder, XElement root)
    {
        if (
            root.Element("graph") is not XElement graph
            || root.Element("vfxTypeData") is not XElement vfxTypeData
            || graph.Element("graphContainer") is not XElement graphContainer
            || graph.Element("baseTypeData") is not XElement baseTypeData
            || graphContainer.Element("nodes") is not XElement nodes
            || graphContainer.Element("trays") is not XElement trays
            || graphContainer.Element("signalLinks") is not XElement signalLinks
            || graphContainer.Element("propertyLinks") is not XElement propertyLinks
        )
            throw new InvalidDataException();

        Offset<GraphVersionInfo> versionOffset = GraphVersionInfo.CreateGraphVersionInfo(
            builder,
            1,
            2,
            3
        );

        GraphHeader.StartGraphHeader(builder);
        GraphHeader.AddVersion(builder, versionOffset);
        Offset<GraphHeader> headerOffset = GraphHeader.EndGraphHeader(builder);

        Offset<GraphBaseTypeData> baseTypeDataOffset = default;
        Offset<VfxTypeData> vfxTypeDataOffset = default;

        SerializeTypeData(
            builder,
            baseTypeData,
            vfxTypeData,
            out baseTypeDataOffset,
            out vfxTypeDataOffset
        );

        Offset<GraphContainer> graphContainerOffset = SerializeGraphContainer(
            builder,
            nodes,
            trays,
            signalLinks,
            propertyLinks
        );

        Offset<GraphBinary> graphBinOffset = GraphBinary.CreateGraphBinary(
            builder,
            headerOffset,
            graphContainerOffset,
            baseTypeDataOffset
        );

        Offset<VfxGraphBinary> vfxGraphBinOffset = VfxGraphBinary.CreateVfxGraphBinary(
            builder,
            graphBinOffset,
            vfxTypeDataOffset
        );

        builder.FinishVfx(vfxGraphBinOffset.Value, "LMVG");
    }

    private static XElement graphContainerToXml(GraphContainer data)
    {
        XElement[] nodesXml = new XElement[data.NodesLength];
        XElement[] traysXml = new XElement[data.TraysLength];
        XElement[] signalLinksXml = new XElement[data.SignalLinksLength];
        XElement[] propertyLinksXml = new XElement[data.PropertyLinksLength];

        // Nodes
        for (int i = 0; i < nodesXml.Length; i++)
        {
            if (
                data.Nodes(i) is not GraphNodeData graphNodeData
                || graphNodeData.Identity is not GraphNodeDataBase identity
            )
                throw new InvalidDataException();

            XElement[] properties = new XElement[graphNodeData.PropertiesLength];
            XElement[] inPorts = new XElement[graphNodeData.InPortsLength];
            XElement[] outPorts = new XElement[graphNodeData.OutPortsLength];

            string? name = null;
            if (!NodeMetadata.NodeType.TryGetValue((int)identity.TypeId, out name))
                name = $"Node_{identity.TypeId:X}";

            for (int j = 0; j < properties.Length; j++)
            {
                if (
                    graphNodeData.Properties(j) is not GraphNodeProperty p
                    || p.Identity is not GraphNodeMemberBase propId
                    || p.ValueReference is not Value propVal
                )
                    throw new InvalidDataException();
                properties[j] = new XElement(
                    "property",
                    [
                        new XAttribute("memberId", propId.MemberId),
                        new XAttribute("nameHash", propId.NameHash),
                        new XAttribute("valueTable", p.ValueTable),
                        new XAttribute(
                            "baseType",
                            NodeMetadata.VfxBaseTypeNames[propVal.ValueType]
                        ),
                        new XAttribute("valueIndex", propVal.ValueIndex),
                    ]
                );
            }
            for (int j = 0; j < inPorts.Length; j++)
            {
                if (graphNodeData.InPorts(j) is not GraphNodePort port)
                    throw new InvalidDataException();

                if (port.Identity is not GraphNodeMemberBase portId)
                    throw new InvalidDataException();

                inPorts[j] = new XElement(
                    "port",
                    [
                        new XAttribute("memberId", portId.MemberId),
                        new XAttribute("nameHash", portId.NameHash),
                        new XAttribute("argumentTypes", port.ArgumentTypes),
                    ]
                );
            }
            for (int j = 0; j < outPorts.Length; j++)
            {
                if (graphNodeData.OutPorts(j) is not GraphNodePort port)
                    throw new InvalidDataException();

                if (port.Identity is not GraphNodeMemberBase portId)
                    throw new InvalidDataException();

                outPorts[j] = new XElement(
                    "port",
                    [
                        new XAttribute("memberId", portId.MemberId),
                        new XAttribute("nameHash", portId.NameHash),
                        new XAttribute("argumentTypes", port.ArgumentTypes),
                    ]
                );
            }

            nodesXml[i] = new XElement(
                "node",
                [
                    new XAttribute("instanceId", identity.InstanceId),
                    new XAttribute("type", name),
                    new XAttribute("typeId", identity.TypeId),
                    new XElement("properties", properties),
                    new XElement("inPorts", inPorts),
                    new XElement("outPorts", outPorts),
                ]
            );
        }

        for (int i = 0; i < traysXml.Length; i++)
        {
            if (data.Trays(i) is not GraphNodeTray tray)
                throw new InvalidDataException();
            string? name = null;
            if (!NodeMetadata.NodeType.TryGetValue((int)tray.TypeId, out name))
                name = $"Node_{tray.TypeId:X}";

            XElement[] references = new XElement[tray.ReferencesLength];
            for (int j = 0; j < references.Length; j++)
                references[j] = new XElement(
                    "reference",
                    new XAttribute("instanceId", tray.References(j))
                );
            traysXml[i] = new XElement(
                "tray",
                [
                    new XAttribute("instanceId", tray.InstanceId),
                    new XAttribute("type", name),
                    new XAttribute("typeId", tray.TypeId),
                    new XElement("references", [references]),
                ]
            );
        }

        for (int i = 0; i < signalLinksXml.Length; i++)
        {
            if (data.SignalLinks(i) is not GraphLinkData link)
                throw new InvalidDataException();
            signalLinksXml[i] = new XElement(
                "link",
                [
                    new XAttribute("instanceId", link.InstanceId),
                    new XAttribute("sourceNodeId", link.SourceNodeId),
                    new XAttribute("sourceMemberId", link.SourceMemberId),
                    new XAttribute("destinationNodeId", link.DestinationNodeId),
                    new XAttribute("destinationMemberId", link.DestinationMemberId),
                    new XAttribute("linkType", link.LinkType),
                    new XAttribute("unknown", link.Unknown),
                ]
            );
        }

        for (int i = 0; i < propertyLinksXml.Length; i++)
        {
            if (data.PropertyLinks(i) is not GraphLinkData link)
                throw new InvalidDataException();
            propertyLinksXml[i] = new XElement(
                "link",
                [
                    new XAttribute("instanceId", link.InstanceId),
                    new XAttribute("sourceNodeId", link.SourceNodeId),
                    new XAttribute("sourceMemberId", link.SourceMemberId),
                    new XAttribute("destinationNodeId", link.DestinationNodeId),
                    new XAttribute("destinationMemberId", link.DestinationMemberId),
                    new XAttribute("linkType", link.LinkType),
                    new XAttribute("unknown", link.Unknown),
                ]
            );
        }

        return new XElement(
            "graphContainer",
            [
                new XElement("nodes", nodesXml),
                new XElement("trays", traysXml),
                new XElement("signalLinks", signalLinksXml),
                new XElement("propertyLinks", propertyLinksXml),
            ]
        );
    }

    private static XElement baseTypeDataToXml(GraphBaseTypeData data)
    {
        // TODO: Throw an exception if an unexpected value type is present

        XElement[] byteDataXml = new XElement[data.ByteDataLength];
        XElement[] int32DataXml = new XElement[data.Int32DataLength];
        XElement[] floatDataXml = new XElement[data.FloatDataLength];
        XElement[] stringDataXml = new XElement[data.StringDataLength];
        XElement[] enumDataXml = new XElement[data.EnumDataLength];
        XElement[] vector2DataXml = new XElement[data.Vector2DataLength];
        XElement[] vector3DataXml = new XElement[data.Vector3DataLength];
        XElement[] vector4DataXml = new XElement[data.Vector4DataLength];
        XElement[] matrix44DataXml = new XElement[data.Matrix44DataLength];
        XElement[] int32ArrayDataXml = new XElement[data.Int32ArrayDataLength];
        XElement[] floatArrayDataXml = new XElement[data.FloatArrayDataLength];
        XElement[] vector3ArrayDataXml = new XElement[data.Vector3ArrayDataLength];

        for (int i = 0; i < byteDataXml.Length; i++)
            byteDataXml[i] = new XElement("value", [new XAttribute("index", i), data.ByteData(i)]);
        for (int i = 0; i < int32DataXml.Length; i++)
            int32DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.Int32Data(i)]
            );
        for (int i = 0; i < floatDataXml.Length; i++)
            floatDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.FloatData(i)]
            );
        for (int i = 0; i < stringDataXml.Length; i++)
            stringDataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), data.StringData(i)]
            );
        for (int i = 0; i < enumDataXml.Length; i++)
            enumDataXml[i] = new XElement("value", [new XAttribute("index", i), data.EnumData(i)]);

        for (int i = 0; i < vector2DataXml.Length; i++)
        {
            if (data.Vector2Data(i) is not Vector4 v)
                throw new InvalidDataException();

            vector2DataXml[i] = new XElement("value", [new XAttribute("index", i), $"{v.X},{v.Y}"]);
        }

        for (int i = 0; i < vector3DataXml.Length; i++)
        {
            if (data.Vector3Data(i) is not Vector4 v)
                throw new InvalidDataException();

            vector3DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), $"{v.X},{v.Y},{v.Z}"]
            );
        }

        for (int i = 0; i < vector4DataXml.Length; i++)
        {
            if (data.Vector4Data(i) is not Vector4 v)
                throw new InvalidDataException();

            vector4DataXml[i] = new XElement(
                "value",
                [new XAttribute("index", i), $"{v.X},{v.Y},{v.Z},{v.W}"]
            );
        }

        for (int i = 0; i < matrix44DataXml.Length; i++)
        {
            if (data.Matrix44Data(i) is not Matrix44 m)
                throw new InvalidDataException();
            matrix44DataXml[i] = new XElement(
                "value",
                [
                    new XAttribute("index", i),
                    new XElement("row0", $"{m.Row0.X},{m.Row0.Y},{m.Row0.Z},{m.Row0.W}"),
                    new XElement("row1", $"{m.Row1.X},{m.Row1.Y},{m.Row1.Z},{m.Row1.W}"),
                    new XElement("row2", $"{m.Row2.X},{m.Row2.Y},{m.Row2.Z},{m.Row2.W}"),
                    new XElement("row3", $"{m.Row3.X},{m.Row3.Y},{m.Row3.Z},{m.Row3.W}"),
                ]
            );
        }

        for (int i = 0; i < int32ArrayDataXml.Length; i++)
        {
            if (data.Int32ArrayData(i) is not Int32ArrayTable t)
                throw new InvalidDataException();

            XElement[] values = new XElement[t.DataLength];
            for (int j = 0; j < values.Length; j++)
                values[j] = new XElement("value", t.Data(j));
            int32ArrayDataXml[i] = new XElement("int32Array", [new XAttribute("index", i), values]);
        }

        for (int i = 0; i < floatArrayDataXml.Length; i++)
        {
            if (data.FloatArrayData(i) is not FloatArrayTable t)
                throw new InvalidDataException();

            XElement[] values = new XElement[t.DataLength];
            for (int j = 0; j < values.Length; j++)
                values[j] = new XElement("value", t.Data(j));
            floatArrayDataXml[i] = new XElement("floatArray", [new XAttribute("index", i), values]);
        }

        for (int i = 0; i < vector3ArrayDataXml.Length; i++)
        {
            if (data.Vector3ArrayData(i) is not Vector3ArrayTable t)
                throw new InvalidDataException();
            XElement[] values = new XElement[t.DataLength];
            for (int j = 0; j < values.Length; j++)
            {
                if (t.Data(j) is not Vector3 v)
                    throw new InvalidDataException();
                values[j] = new XElement("value", $"{v.X},{v.Y},{v.Z},{v.Padding}");
            }
            vector3ArrayDataXml[i] = new XElement(
                "vector3Array",
                [new XAttribute("index", i), values]
            );
        }

        // Bool = 1,
        // Int32 = 2,
        // Float32 = 3,
        // Vector2 = 4,
        // Vector3 = 5,
        // Vector4 = 6,
        // Matrix44 = 7,
        // String = 8,
        // Pointer = 9,
        // Int32Array = 10,
        // FCurve = 11,
        // FloatArray = 12,
        // Vector4Array = 15,
        // VectorData = 16

        return new XElement(
            "baseTypeData",
            [
                new XElement("byteData", byteDataXml),
                new XElement("int32Data", int32DataXml),
                new XElement("floatData", floatDataXml),
                new XElement("stringData", stringDataXml),
                new XElement("enumData", enumDataXml),
                new XElement("vector2Data", vector2DataXml),
                new XElement("vector3Data", vector3DataXml),
                new XElement("vector4Data", vector4DataXml),
                new XElement("matrix44Data", matrix44DataXml),
                new XElement("int32ArrayData", int32ArrayDataXml),
                new XElement("floatArrayData", floatArrayDataXml),
                new XElement("vector3ArrayData", vector3ArrayDataXml),
            ]
        );
    }

    private static XElement vfxTypeDataToXml(VfxTypeData data)
    {
        // Right now this only handles VFX Curves, which should cover most vfx data
        // this leaves ~25 vfx files with issues
        if (
            data.BoxDataLength > 0
            || data.SphereDataLength > 0
            || data.PointerDataLength > 0
            || data.VectorDataLength > 0
            || data.OffsetDataLength > 0
            || data.OffsetArrayDataLength > 0
        )
            throw new NotImplementedException("Unsupported Vfx type");

        XElement[] fCurvesXml = new XElement[data.FCurveDataLength];
        for (int i = 0; i < fCurvesXml.Length; i++)
        {
            if (data.FCurveData(i) is not VfxFCurveData curveTable)
                throw new InvalidDataException();

            XElement[] anchorPoints = new XElement[curveTable.AnchorPointsLength];
            for (int j = 0; j < anchorPoints.Length; j++)
            {
                if (curveTable.AnchorPoints(j) is not FCurveAnchorPoint anchorPoint)
                    throw new InvalidDataException();

                anchorPoints[j] = new XElement(
                    "anchorPoint",
                    [
                        new XAttribute("time", $"{anchorPoint.Time}"),
                        $"{anchorPoint.X},{anchorPoint.Y},{anchorPoint.Z}",
                    ]
                );
            }

            fCurvesXml[i] = new XElement(
                "fCurve",
                [
                    new XAttribute("loopIn", curveTable.LoopIn),
                    new XAttribute("loopOut", curveTable.LoopOut),
                    new XAttribute("loopType", curveTable.CurveLoopType),
                    new XElement("anchorPoints", anchorPoints),
                ]
            );
        }
        return new XElement("vfxTypeData", [new XElement("fCurves", fCurvesXml)]);
    }

    public static XElement ToXml(VfxGraphBinary data, string name)
    {
        if (data.GraphBinary is not GraphBinary graphBinary)
            throw new InvalidDataException();

        if (graphBinary.GraphContainer is not GraphContainer graphContainer)
            throw new InvalidDataException();

        XElement graphContainerXml = graphContainerToXml(graphContainer);

        if (graphBinary.BaseTypeData is not GraphBaseTypeData baseTypeData)
            throw new InvalidDataException();
        XElement baseTypeDataXml = baseTypeDataToXml(baseTypeData);

        if (data.VfxTypeData is not VfxTypeData vfxTypeData)
            throw new InvalidDataException();

        XElement vfxTypeDataXml = vfxTypeDataToXml(vfxTypeData);

        return new XElement(
            "vfxGraph",
            [
                new XAttribute("name", name),
                new XElement("graph", [graphContainerXml, baseTypeDataXml]),
                vfxTypeDataXml,
            ]
        );
    }
}
