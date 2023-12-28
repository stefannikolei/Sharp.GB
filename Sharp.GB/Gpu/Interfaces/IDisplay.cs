using Sharp.GB.Gpu;

namespace Sharp.GB.Gpu
{
    public interface IDisplay
    {
        void PutDmgPixel(int color);

        void PutColorPixel(int gbcRgb);

        void RequestRefresh();

        void WaitForRefresh();

        void EnableLcd();

        void DisableLcd();
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
}
