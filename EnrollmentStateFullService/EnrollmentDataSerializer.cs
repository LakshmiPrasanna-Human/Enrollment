using Microsoft.ServiceFabric.Data;
using System.IO;
using Bond;
using Bond.Protocols;
using Bond.IO.Unsafe;


namespace EnrollmentStateFullService
{
    // Custom serializer for the  structure.
    public sealed class EnrollmentDataSerializer : IStateSerializer<EnrollmentData>
    {
        private readonly static Serializer<CompactBinaryWriter<OutputBuffer>> Serializer;
        private readonly static Deserializer<CompactBinaryReader<InputBuffer>> Deserializer;

        static EnrollmentDataSerializer()
        {
            // Create the serializers and deserializers for FileMetadata.
            Serializer = new Serializer<CompactBinaryWriter<OutputBuffer>>(typeof(EnrollmentData));
            Deserializer = new Deserializer<CompactBinaryReader<InputBuffer>>(typeof(EnrollmentData));
        }

        public EnrollmentDataSerializer()
        {

        }

        public EnrollmentData Read(BinaryReader binaryReader)
        {
            int count = binaryReader.ReadInt32();
            byte[] bytes = binaryReader.ReadBytes(count);

            var input = new InputBuffer(bytes);
            var reader = new CompactBinaryReader<InputBuffer>(input);
            return Deserializer.Deserialize<EnrollmentData>(reader);
        }

        public EnrollmentData Read(EnrollmentData baseValue, BinaryReader binaryReader)
        {
            return Read(binaryReader);
        }

        public void Write(EnrollmentData value, BinaryWriter binaryWriter)
        {
            var output = new OutputBuffer();
            var writer = new CompactBinaryWriter<OutputBuffer>(output);
            Serializer.Serialize(value, writer);

            binaryWriter.Write(output.Data.Count);
            binaryWriter.Write(output.Data.Array, output.Data.Offset, output.Data.Count);
        }

        public void Write(EnrollmentData baseValue, EnrollmentData targetValue, BinaryWriter binaryWriter)
        {
            Write(targetValue, binaryWriter);
        }
    }

}
