namespace POCEnttec
{
    public static class ByteOperations
    {
        public static byte LSB(this int input) => (byte) (input & 0xff);
        public static byte MSB(this int input) => (byte) ((input >> 8) & 255);
    }
}