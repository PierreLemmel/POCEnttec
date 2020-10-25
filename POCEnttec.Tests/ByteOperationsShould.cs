using NFluent;
using NUnit.Framework;
using System;

namespace POCEnttec.Tests
{
    public class ByteOperationsShould
    {
        [Test]
        [TestCase(0x00000000, 0x00)]
        [TestCase(0x0000b500, 0x00)]
        [TestCase(0x1aaaaaaa, 0xaa)]
        [TestCase(0x12345600, 0x00)]
        [TestCase(0x00120000, 0x00)]
        [TestCase(0x00000012, 0x12)]
        public void LSB_Should_Return_Expected_Result(int input, byte expected)
        {
            byte lsb = input.LSB();

            Check.That(lsb)
                .IsEqualTo(expected);
        }

        [Test]
        [TestCase(0x00000000, 0x00)]
        [TestCase(0x0000b500, 0xb5)]
        [TestCase(0x1aaabbaa, 0xbb)]
        [TestCase(0x12345678, 0x56)]
        [TestCase(0x00012000, 0x20)]
        [TestCase(0x00000112, 0x01)]
        public void MSB_Should_Return_Expected_Result(int input, byte expected)
        {
            byte msb = input.MSB();

            Check.That(msb)
                .IsEqualTo(expected);
        }
    }
}