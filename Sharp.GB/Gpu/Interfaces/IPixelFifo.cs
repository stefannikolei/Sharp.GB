namespace Sharp.GB.Gpu
{
    public interface IPixelFifo
    {
        int getLength();

        void putPixelToScreen();

        void dropPixel();

        void enqueue8Pixels(int[] pixels, TileAttributes tileAttributes);

        void setOverlay(int[] pixelLine, int offset, TileAttributes flags, int oamIndex);

        void clear();
    }
}
