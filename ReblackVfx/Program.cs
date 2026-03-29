using System.Collections.Concurrent;
using System.CommandLine;
using System.Xml.Linq;
using Google.FlatBuffers;
using ReblackVfx;
using ReblackVfx.FlatBuffer.BaseType;
using ReblackVfx.FlatBuffer.Graph;
using ReblackVfx.FlatBuffer.VfxGraph;

internal static class Program
{
    private static bool ValueTableCheck(string path)
    {
        byte[] fileBytes;
        using (var stream = new FileStream(path, FileMode.Open))
        {
            fileBytes = new byte[stream.Length];
            stream.ReadExactly(fileBytes);
        }
        ByteBuffer bb = new ByteBuffer(fileBytes, 16);
        VfxGraphBinary vfxData = VfxGraphBinary.GetRootAsVfxGraphBinary(bb);

        if (vfxData.VfxTypeData is not VfxTypeData vfxTypeData)
            throw new InvalidDataException($"Oh dear. {path}");

        if (vfxTypeData.PointerDataLength > 0)
            return true;
        if (vfxTypeData.VectorDataLength > 0)
            return true;

        return false;
    }

    public static void Test()
    {
        bool[] fields = Enumerable.Repeat(false, 7).ToArray();

        string effectsDir = @"H:/FFXV Debug Build/6575095/datas/effects/";
        IEnumerable<string> files = Directory.EnumerateFiles(
            effectsDir,
            "*.vfx",
            SearchOption.AllDirectories
        );
        Console.WriteLine($"Total VFX files: {files.Count()}");
        object mutex = new object();
        ConcurrentBag<string> pain = new ConcurrentBag<string>();

        Parallel.ForEach(
            files,
            file =>
            {
                var result = ValueTableCheck(file);
                if (result)
                {
                    pain.Add(file);
                }
            }
        );

        Console.WriteLine($"Pain: {pain.Count}/{files.Count()}");
    }

    public static int Main(string[] args)
    {
        Option<FileInfo> inFileOption = new("--in", "-i")
        {
            Description = "Input file path",
            Required = true,
            Recursive = true,
        };
        Option<FileInfo> outFileOption = new("--out", "-o")
        {
            Description = "Output file path",
            Required = false,
            Recursive = true,
        };

        RootCommand rootCommand = new("Final Fantasy XV VFX Graph Converter");
        rootCommand.Options.Add(inFileOption);
        rootCommand.Options.Add(outFileOption);

        Command vfxToXmlCommand = new("vfx-xml", "Convert .vfx to XML");
        Command xmlToVfxCommand = new("xml-vfx", "Convert XML to .vfx");
        Command testCommand = new("test", "Run flatbuffer check on all .vfx files");

        rootCommand.Subcommands.Add(vfxToXmlCommand);
        rootCommand.Subcommands.Add(xmlToVfxCommand);
        rootCommand.Subcommands.Add(testCommand);
        testCommand.SetAction(parseResult =>
        {
            Test();
        });

        vfxToXmlCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(inFileOption);
            if (input is not FileInfo file)
                throw new Exception();
            Console.WriteLine($"Converting {input.Name} to .xml...");

            string name;
            if (parseResult.GetValue(outFileOption) is FileInfo output)
                name = output.FullName;
            else
                name = $"{Path.GetFileNameWithoutExtension(input.Name)}.xml";

            byte[] data;
            using (var stream = new FileStream(input.FullName, FileMode.Open))
            {
                data = new byte[stream.Length];
                stream.ReadExactly(data);
            }
            ByteBuffer buffer = new ByteBuffer(data, 16);
            VfxGraphBinary vfxData = VfxGraphBinary.GetRootAsVfxGraphBinary(buffer);

            XElement root = VfxConverter.ToXml(vfxData, Path.GetFileNameWithoutExtension(name));

            root.Save(name);
        });

        xmlToVfxCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(inFileOption);
            if (input is not FileInfo file)
                throw new Exception();
            Console.WriteLine($"Converting {input.Name} to .vfx...");
            string name;
            if (parseResult.GetValue(outFileOption) is FileInfo output)
                name = output.FullName;
            else
                name = $"{Path.GetFileNameWithoutExtension(input.Name)}.xml";
            FlatBufferBuilder builder = new FlatBufferBuilder(1024);
            XElement root = XElement.Load(input.FullName);
            //ToBinary(root, builder);

            using (var stream = new FileStream(name, FileMode.Create))
            {
                byte[] array = builder.SizedByteArray();
                stream.Write(array, 0, array.Length);
            }
        });

        rootCommand.Parse(args).Invoke();
        return 0;
    }
}
