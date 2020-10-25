using System;

namespace POCEnttec
{
    /// <summary>
    /// For more information, see : https://dol2kh495zr52.cloudfront.net/pdf/misc/dmx_usb_pro_api_spec.pdf
    /// </summary>
    public static class Enttec
    {
        public const byte StartOfMessage = 0x7e;
        public const byte EndOfMessage = 0xe7;

        public const byte StartCode = 0x00;

        public const byte SendDMXPacket = 6;
    }
}