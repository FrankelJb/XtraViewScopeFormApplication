
namespace XtraViewScopeFormApplication.Models.Enums
{
    public struct XmpPacketTransmissionStatus
    {
        public const string Passed = "XMP Transmission was normal";
        public const string FailedAdd2Packets = "XMP Transmission had malformed add2 packets";
        public const string FailedNibbles = "XMP Transmission had malformed nibbles";
    }
}
