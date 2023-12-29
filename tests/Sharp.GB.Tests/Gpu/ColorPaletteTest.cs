using FluentAssertions;
using Sharp.GB.Gpu;
using Xunit;

public class ColorPaletteTest
{
    [Fact]
    public void TestAutoIncrement()
    {
        ColorPalette p = new ColorPalette(0xff68);
        p.SetByte(0xff68, 0x80);
        p.SetByte(0xff69, 0x00);
        p.SetByte(0xff69, 0xaa);
        p.SetByte(0xff69, 0x11);
        p.SetByte(0xff69, 0xbb);
        p.SetByte(0xff69, 0x22);
        p.SetByte(0xff69, 0xcc);
        p.SetByte(0xff69, 0x33);
        p.SetByte(0xff69, 0xdd);
        p.SetByte(0xff69, 0x44);
        p.SetByte(0xff69, 0xee);
        p.SetByte(0xff69, 0x55);
        p.SetByte(0xff69, 0xff);

        int[] a = [0xaa00, 0xbb11, 0xcc22, 0xdd33];
        p.GetPalette(0).Should().BeEquivalentTo(a);

        new[] { 0xee44, 0xff55, 0x0000, 0x0000 }.Should().BeEquivalentTo(p.GetPalette(1));
    }
}
