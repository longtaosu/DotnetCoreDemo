using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib
{
    public interface ICaptchaService
    {
        void SetOptions(CaptchaOptions option);

        byte[] GetCaptcha(string text);

        string GetRandomText(int length);
    }

    public class CaptchaOptions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
