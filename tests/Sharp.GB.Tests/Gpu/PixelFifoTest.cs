using System.Collections.Generic;
using Sharp.GB.Gpu;
using Sharp.GB.Memory;
using Xunit;

public class PixelFifoTest
{
    private readonly DmgPixelFifo _fifo;

    public PixelFifoTest()
    {
        MemoryRegisters r = new MemoryRegisters(GpuRegister.All);
        r.Put(GpuRegister.Bgp, 0b11100100);
        _fifo = new DmgPixelFifo(NullDisplay.Instance, new Lcdc(), r);
    }

    [Fact]
    public void TestEnqueue()
    {
        _fifo.Enqueue8Pixels(Zip(0b11001001, 0b11110000, false), TileAttributes.Empty);
        Assert.Equal([3, 3, 2, 2, 1, 0, 0, 1], ArrayQueueAsList(_fifo.GetPixels()));
    }

    [Fact]
    public void TestDequeue()
    {
        _fifo.Enqueue8Pixels(Zip(0b11001001, 0b11110000, false), TileAttributes.Empty);
        _fifo.Enqueue8Pixels(Zip(0b10101011, 0b11100111, false), TileAttributes.Empty);
        Assert.Equal(0b11, _fifo.DequeuePixel());
        Assert.Equal(0b11, _fifo.DequeuePixel());
        Assert.Equal(0b10, _fifo.DequeuePixel());
        Assert.Equal(0b10, _fifo.DequeuePixel());
        Assert.Equal(0b01, _fifo.DequeuePixel());
    }

    [Fact]
    public void TestZip()
    {
        Assert.Equal(new int[] { 3, 3, 2, 2, 1, 0, 0, 1 }, Zip(0b11001001, 0b11110000, false));
        Assert.Equal(new int[] { 1, 0, 0, 1, 2, 2, 3, 3 }, Zip(0b11001001, 0b11110000, true));
    }

    private int[] Zip(int data1, int data2, bool reverse)
    {
        return Fetcher.Zip(data1, data2, reverse, new int[8]);
    }

    private static List<int> ArrayQueueAsList(IntQueue queue)
    {
        List<int> l = new List<int>();
        for (int i = 0; i < queue.Size; i++)
        {
            l.Add(queue.Get(i));
        }
        return l;
    }
}
