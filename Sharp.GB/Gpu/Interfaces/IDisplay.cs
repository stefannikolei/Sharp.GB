using Sharp.GB.Gpu;

namespace Sharp.GB.Gpu
{
    public interface IDisplay
    {
        static int DisplayWidth = 160;

        static int DisplayHeight = 144;

        void PutDmgPixel(int color);

        void PutColorPixel(int gbcRgb);

        void FrameIsReady();

        void EnableLcd();

        void DisableLcd();

        static int TranslateGbcRgb(int gbcRgb)
        {
            int r = (gbcRgb >> 0) & 0x1f;
            int g = (gbcRgb >> 5) & 0x1f;
            int b = (gbcRgb >> 10) & 0x1f;
            int result = (r * 8) << 16;
            result |= (g * 8) << 8;
            result |= (b * 8) << 0;
            return result;
        }
    }
}

public class NullDisplay : IDisplay
{
    public static IDisplay Instance => new NullDisplay();

    public void PutDmgPixel(int color) { }

    public void PutColorPixel(int gbcRgb) { }

    public void RequestRefresh() { }

    public void WaitForRefresh() { }

    public void EnableLcd() { }

    public void DisableLcd() { }

    public void FrameIsReady() { }
}
