using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace FCP.Util
{
    /// <summary>
    /// 验证码图片
    /// </summary>
    public class ValidateCodeImage
    {
        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="validateCode">验证码</param>
        /// <returns></returns>
        public static byte[] createValidateImage(string validateCode, ValidateCodeImageSetting codeImageSetting = null)
        {
            validateCode = (validateCode ?? string.Empty).Trim();            
            if (validateCode.Length < 1)
            {
                return null;
            }
            codeImageSetting = codeImageSetting ?? new ValidateCodeImageSetting();

            Bitmap codeImage = null;
            try
            {
                codeImage = createValidateImageInternal(validateCode, codeImageSetting);
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.FormatLogMessage());
                return null;
            }            
            return getImageBytes(codeImage, codeImageSetting.CodeImageFormat);
        }

        /// <summary>
        /// 创建验证码图片 核心方法
        /// </summary>
        /// <param name="validateCode">验证码</param>
        /// <param name="codeImageSetting">验证码图片配置</param>
        /// <returns></returns>
        private static Bitmap createValidateImageInternal(string validateCode, ValidateCodeImageSetting codeImageSetting)
        {
            int imageWidth = (int)(validateCode.Length * codeImageSetting.FontWidth) + 4 + codeImageSetting.Padding * 2;
            Bitmap codeImage = new Bitmap(imageWidth, codeImageSetting.ImageHeight);

            using (Graphics graphics = Graphics.FromImage(codeImage))
            {
                graphics.Clear(codeImageSetting.BackgroundColor); //设置背景色
            }
            drawNoisePixel(codeImage, validateCode.Length, codeImageSetting);   //绘制背景噪点
            drawValidateCode(codeImage, validateCode, codeImageSetting);    //绘制验证码
            drawNoiseLine(codeImage, validateCode.Length, codeImageSetting);   //绘制前景噪线

            return codeImage;
        }

        /// <summary>
        /// 绘制背景噪点
        /// </summary>
        /// <param name="codeImage">验证码图片</param>
        /// <param name="validateCodeLength">验证码数量</param>
        /// <param name="codeImageSetting">验证码图片配置</param>
        private static void drawNoisePixel(Bitmap codeImage, int validateCodeLength, ValidateCodeImageSetting codeImageSetting)
        {            
            if (codeImageSetting.IsNoisePixel)
            {
                Pen pen = new Pen(codeImageSetting.NoisePixelColor, 0);
                int pixelNum = validateCodeLength * 10;  //计算噪点数量

                Random rand = new Random();
                using (Graphics graphics = Graphics.FromImage(codeImage))
                {
                    for (int i = 0; i < pixelNum; i++)
                    {
                        int x = rand.Next(codeImage.Width);
                        int y = rand.Next(codeImage.Height);
                        graphics.DrawRectangle(pen, x, y, 1, 1);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制前景噪线
        /// </summary>
        /// <param name="codeImage">验证码图片</param>
        /// <param name="validateCodeLength">验证码数量</param>
        /// <param name="codeImageSetting">验证码图片配置</param>
        private static void drawNoiseLine(Bitmap codeImage, int validateCodeLength, ValidateCodeImageSetting codeImageSetting)
        {
            if (codeImageSetting.IsNoiseLine)
            {
                Pen pen = new Pen(codeImageSetting.NoiseLineColor, codeImageSetting.NoiseLineWidth);
                int lineNum = validateCodeLength * 2;  //计算噪线数量

                Random rand = new Random();
                using (Graphics graphics = Graphics.FromImage(codeImage))
                {
                    for (int i = 0; i < lineNum; i++)
                    {
                        Point startPoint = new Point(rand.Next(codeImage.Width), rand.Next(codeImage.Height));
                        Point endPoint = new Point(rand.Next(codeImage.Width), rand.Next(codeImage.Height));
                        graphics.DrawLine(pen, startPoint, endPoint);
                    }                    
                }                
            }
        }

        /// <summary>
        /// 绘制验证码
        /// </summary>
        /// <param name="codeImage">验证码图片</param>
        /// <param name="validateCode">验证码</param>
        /// <param name="codeImageSetting">验证码图片配置</param>
        private static void drawValidateCode(Bitmap codeImage, string validateCode, ValidateCodeImageSetting codeImageSetting)
        {
            char[] validateCodeCharArr = validateCode.ToCharArray();
            Font font = new Font(codeImageSetting.Fonts[0], codeImageSetting.FontSize, FontStyle.Regular);
            Brush brush = new SolidBrush(codeImageSetting.BrushColors[0]);
            Random rand = new Random();            
            int eighthImageHeight = codeImageSetting.ImageHeight / 8;
            using (Graphics graphics = Graphics.FromImage(codeImage))
            {
                if (codeImageSetting.IsFontTextAntiAlias)
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;  //抗锯齿
                else
                    graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                for (int i = 0; i < validateCodeCharArr.Length; i++)
                {
                    if (codeImageSetting.IsRandomFont)  //随机字体
                    {
                        int fontIndex = rand.Next(codeImageSetting.Fonts.Length);
                        font = new Font(codeImageSetting.Fonts[fontIndex], codeImageSetting.FontSize, FontStyle.Regular);
                    }
                    if (codeImageSetting.IsRandomColor)  //随机画刷颜色
                    {
                        brush = new SolidBrush(codeImageSetting.BrushColors[rand.Next(codeImageSetting.BrushColors.Length)]);
                    }
                    int x = i * codeImageSetting.FontWidth + rand.Next(codeImageSetting.Padding);
                    int y = eighthImageHeight + rand.Next(eighthImageHeight);   //在1/8高度附近随机，保证起始高度在1/8到1/4之间                    
                    graphics.DrawString(validateCodeCharArr[i].ToString(), font, brush, new Point(x, y));
                }
            }
        }

        /// <summary>
        /// 获取验证码图片字节数组
        /// </summary>
        /// <param name="codeImage">验证码图片</param>
        /// <returns></returns>
        private static byte[] getImageBytes(Bitmap codeImage, ImageFormat imageFormat)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                codeImage.Save(ms, imageFormat);
                return ms.GetBuffer();
            }
        }
    }

    /// <summary>
    /// 验证码图片配置
    /// </summary>
    public class ValidateCodeImageSetting
    {
        #region 边框间隔
        int padding = 2;
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        #endregion

        #region 是否输出噪点(默认输出)
        bool isNoisePixel = true;
        public bool IsNoisePixel
        {
            get { return isNoisePixel; }
            set { isNoisePixel = value; }
        }
        #endregion

        #region 输出噪点的颜色(默认灰色)
        Color noisePixelColor = Color.LightGray;
        public Color NoisePixelColor
        {
            get { return noisePixelColor; }
            set { noisePixelColor = value; }
        }
        #endregion

        #region 是否输出噪线
        bool isNoiseLine = false;
        public bool IsNoiseLine
        {
            get { return isNoiseLine; }
            set { isNoiseLine = value; }
        }
        #endregion

        #region 输出噪线的颜色(默认灰色)
        Color noiseLineColor = Color.LightGray;
        public Color NoiseLineColor
        {
            get { return noiseLineColor; }
            set { noiseLineColor = value; }
        }
        #endregion

        #region 输出噪线的宽度
        float noiseLineWidth = 1;
        public float NoiseLineWidth
        {
            get { return noiseLineWidth; }
            set { noiseLineWidth = value; }
        }
        #endregion

        #region 自定义背景色(默认白色)
        Color backgroundColor = Color.White;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        #endregion

        #region 自定义随机颜色数组
        Color[] brushcolors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
        public Color[] BrushColors
        {
            get { return brushcolors; }
            set { brushcolors = value; }
        }
        #endregion

        #region 是否随机颜色
        bool isRandomColor = true;
        public bool IsRandomColor
        {
            get { return isRandomColor; }
            set { isRandomColor = value; }
        }
        #endregion

        #region 自定义字体数组
        string[] fonts = { "Arial", "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "宋体" };
        public string[] Fonts
        {
            get { return fonts; }
            set { fonts = value; }
        }
        #endregion

        #region 是否随机字体
        bool isRandomFont = true;
        public bool IsRandomFont
        {
            get { return isRandomFont; }
            set { isRandomFont = value; }
        }
        #endregion

        #region 验证码字体大小数组(为了显示扭曲效果，默认40像素，可以自行修改)
        int fontSize = 16;
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }
        #endregion

        #region 是否字体抗锯齿
        bool isFontTextAntiAlias = true;
        public bool IsFontTextAntiAlias
        {
            get { return isFontTextAntiAlias; }
            set { isFontTextAntiAlias = value; }
        }
        #endregion

        #region 生成图片格式
        ImageFormat codeImageFormat = ImageFormat.Png;
        public ImageFormat CodeImageFormat
        {
            get { return codeImageFormat; }
            set { codeImageFormat = value; }
        }
        #endregion

        #region 字体宽度
        public int FontWidth
        {
            get
            {
                return FontSize + Padding;
            }
        }
        #endregion

        #region 图片高度
        public int ImageHeight
        {
            get
            {
                return (FontSize + Padding) * 2;
            }
        }
        #endregion
    }
}
