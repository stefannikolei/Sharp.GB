namespace Sharp.GB.Gpu
{
    public interface IDisplay
    {
        void putDmgPixel(int color);

        void putColorPixel(int gbcRgb);

        void requestRefresh();

        void waitForRefresh();

        void enableLcd();

        void disableLcd();
    }
}
