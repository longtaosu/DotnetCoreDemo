using Microsoft.Extensions.DependencyInjection;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib
{
    public class CaptchaService1 : ICaptchaService
    {
        private int _width;
        private int _height;

        private readonly Color[] Colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Brown, Color.Purple };
        private readonly char[] Chars = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public CaptchaService1(CaptchaOptions options)
        {
            _width = options.Width;
            _height = options.Height;
        }
     

        public byte[] GetCaptcha(string text)
        {
            var code = text;
            var r = new Random();

            using var image = new Image<Rgba32>(_width, _height);
            var font = SystemFonts.CreateFont(SystemFonts.Families.First().Name, 25, FontStyle.Bold);

            image.Mutate(ctx =>
            {
                ctx.Fill(Color.White);

                for (int i = 0; i < code.Length; i++)
                {
                    ctx.DrawText(code[i].ToString(), font, Colors[r.Next(Colors.Length)], 
                        new PointF((this._width-10) * i /code.Length  + 5,r.Next(this._height / 5 ,this._height / 4 ))
                        );
                }

                for (int i = 0; i < 5; i++)
                {
                    var pen = new Pen(Colors[r.Next(Colors.Length)], 1);
                    var p1 = new PointF(r.Next(_width), r.Next(_height));
                    var p2 = new PointF(r.Next(_width), r.Next(_height));

                    ctx.DrawLines(pen, p1, p2);
                }

                for (int i = 0; i < 20; i++)
                {
                    var pen = new Pen(Colors[r.Next(Colors.Length)], 1);
                    var p1 = new PointF(r.Next(_width), r.Next(_height));
                    var p2 = new PointF(p1.X + 1f, p1.Y + 1f);

                    ctx.DrawLines(pen, p1, p2);
                }
            });

            using var ms = new System.IO.MemoryStream();
            image.SaveAsPng(ms);
            return ms.ToArray();
        }

        public string GetRandomText(int length)
        {
            var code = string.Empty;
            var r = new Random();

            for (int i = 0; i < length; i++)
            {
                code += Chars[r.Next(Chars.Length)].ToString();
            }

            return code;
        }

        public void SetOptions(CaptchaOptions option)
        {
            this._width = option.Width;
            this._height = option.Height;
        }
    }

    public static class CaptchaExtension
    {
        public static IServiceCollection AddCaptchaService(this IServiceCollection services)
        {
            services.AddScoped<ICaptchaService>(x => new CaptchaService1(
                new CaptchaOptions() {
                Height = 35,
                Width = 90
            }));
            return services;
        }
    }
}
