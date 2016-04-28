using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    public class AnimatedGifEncoder
    {
        protected int _width; // image size
        protected int _height;
        protected Color? _transparent = null; // transparent color if given
        protected int _transIndex; // transparent index in color table
        protected int _repeat = -1; // no repeat
        protected int _delay = 0; // frame delay (hundredths)
        protected bool _isStarted = false; // ready to output frames
        //	protected BinaryWriter bw;
        protected FileStream _fileStream;

        protected BitmapSource _currentFrame; // current frame
        protected byte[] _pixels; // BGR byte array from frame
        protected byte[] _indexedPixels; // converted frame indexed to palette
        protected int _colorDepth; // number of bit planes
        protected byte[] _colorTab; // RGB palette
        protected bool[] _usedEntry = new bool[256]; // active palette entries
        protected int _paletteSize = 7; // color table size (bits-1)
        protected int _disposalCode = -1; // disposal code (-1 = use default)
        protected bool _closeStreamRequested = false; // close stream when finished
        protected bool _isFirstFrame = true;
        protected bool _isSizeSet = false; // if false, get size from first frame
        protected int _sample = 10; // default sample interval for quantizer

        /**
         * Sets the delay time between each frame, or changes it
         * for subsequent frames (applies to last frame added).
         *
         * @param ms int delay time in milliseconds
         */
        public void SetDelay(int ms)
        {
            _delay = (int)Math.Round(ms / 10.0f);
        }

        /**
         * Sets the GIF frame disposal code for the last added frame
         * and any subsequent frames.  Default is 0 if no transparent
         * color has been set, otherwise 2.
         * @param code int disposal code.
         */
        public void SetDispose(int code)
        {
            if (code >= 0)
            {
                _disposalCode = code;
            }
        }

        /**
         * Sets the number of times the set of GIF frames
         * should be played.  Default is 1; 0 means play
         * indefinitely.  Must be invoked before the first
         * image is added.
         *
         * @param iter int number of iterations.
         * @return
         */
        public void SetRepeat(int iter)
        {
            if (iter >= 0)
            {
                _repeat = iter;
            }
        }

        /**
         * Sets the transparent color for the last added frame
         * and any subsequent frames.
         * Since all colors are subject to modification
         * in the quantization process, the color in the final
         * palette for each frame closest to the given color
         * becomes the transparent color for that frame.
         * May be set to null to indicate no transparent color.
         *
         * @param c Color to be treated as transparent on display.
         */
        public void SetTransparentColor(Color c)
        {
            _transparent = c;
        }

        /**
         * Adds next GIF frame.  The frame is not written immediately, but is
         * actually deferred until the next frame is received so that timing
         * data can be inserted.  Invoking <code>finish()</code> flushes all
         * frames.  If <code>setSize</code> was not invoked, the size of the
         * first image is used for all subsequent frames.
         *
         * @param im BufferedImage containing frame to write.
         * @return true if successful.
         */
        public bool AddFrame(BitmapSource source)
        {
            if ((source == null) || !_isStarted)
                return false;

            var succeed = true;

            try
            {
                if (!_isSizeSet)
                {
                    // use first frame's size
                    this.SetSize((int)source.Width, (int)source.Height);
                }


                _currentFrame = source;
                this.GetImagePixels(); // convert to correct format if necessary
                this.AnalyzePixels(); // build color table & map pixels
                if (_isFirstFrame)
                {
                    this.WriteLocalScreenDescriptor(); // logical screen descriptior
                    this.WritePalette(); // global color table
                    if (_repeat >= 0)
                    {
                        // use NS app extension to indicate reps
                        this.WriteNetscapeExtension();
                    }
                }
                this.WriteGraphicControlExtension(); // write graphic control extension
                this.WriteImageDescriptor(); // image descriptor
                if (!_isFirstFrame)
                {
                    this.WritePalette(); // local color table
                }
                this.WritePixels(); // encode and write pixel data
                _isFirstFrame = false;
            }
            catch (IOException)
            {
                succeed = false;
            }

            return succeed;
        }

        /**
         * Flushes any pending data and closes output file.
         * If writing to an OutputStream, the stream is not
         * closed.
         */
        public bool Finish()
        {
            if (!_isStarted)
                return false;

            var succeed = true;
            _isStarted = false;

            try
            {
                _fileStream.WriteByte(0x3b); // gif trailer
                _fileStream.Flush();

                if (_closeStreamRequested)
                    _fileStream.Close();

            }
            catch (IOException)
            {
                succeed = false;
            }

            // reset for subsequent use
            _transIndex = 0;
            _fileStream = null;
            _currentFrame = null;
            _pixels = null;
            _indexedPixels = null;
            _colorTab = null;
            _closeStreamRequested = false;
            _isFirstFrame = true;

            return succeed;
        }

        /**
         * Sets frame rate in frames per second.  Equivalent to
         * <code>setDelay(1000/fps)</code>.
         *
         * @param fps float frame rate (frames per second)
         */
        public void SetFrameRate(double fps)
        {
            if (fps != 0)
            {
                _delay = (int)Math.Round(100.0 / fps);
            }
        }

        /**
         * Sets quality of color quantization (conversion of images
         * to the maximum 256 colors allowed by the GIF specification).
         * Lower values (minimum = 1) produce better colors, but slow
         * processing significantly.  10 is the default, and produces
         * good color mapping at reasonable speeds.  Values greater
         * than 20 do not yield significant improvements in speed.
         *
         * @param quality int greater than 0.
         * @return
         */
        public void SetQuality(int quality)
        {
            if (quality < 1) quality = 1;
            _sample = quality;
        }

        /**
         * Sets the GIF frame size.  The default size is the
         * size of the first frame added if this method is
         * not invoked.
         *
         * @param w int frame width.
         * @param h int frame width.
         */
        public void SetSize(int w, int h)
        {
            if (_isStarted && !_isFirstFrame)
                return;

            _width = w;
            _height = h;
            if (_width < 1)
                _width = 320;

            if (_height < 1)
                _height = 240;

            _isSizeSet = true;
        }

        /**
         * Initiates GIF file creation on the given stream.  The stream
         * is not closed automatically.
         *
         * @param os OutputStream on which GIF images are written.
         * @return false if initial write failed.
         */
        public bool Start(FileStream fileStream)
        {
            if (fileStream == null)
                return false;

            var succeed = true;

            _closeStreamRequested = false;
            _fileStream = fileStream;

            try
            {
                WriteString("GIF89a"); // header
            }
            catch (IOException)
            {
                succeed = false;
            }

            return _isStarted = succeed;
        }

        /**
         * Initiates writing of a GIF file with the specified name.
         *
         * @param file String containing output file name.
         * @return false if open or initial write failed.
         */
        public bool Start(String file)
        {
            var succeed = true;
            try
            {
                _fileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                succeed = Start(_fileStream);
                _closeStreamRequested = true;
            }
            catch (IOException)
            {
                succeed = false;
            }
            return _isStarted = succeed;
        }

        /**
         * Analyzes image colors and creates color map.
         */
        protected void AnalyzePixels()
        {
            int len = _pixels.Length;
            int nPix = len / 3;
            _indexedPixels = new byte[nPix];
            NeuQuant nq = new NeuQuant(_pixels, len, _sample);
            // initialize quantizer
            var nqColorTab = nq.Process(); // create reduced palette

            // NeuQuant may change the transparent color in picture
            // thus we only give 255 slots to it and keep the last
            // slot for the transparent color
            _colorTab = new byte[256 * 3];
            Array.Copy(nqColorTab, _colorTab, nqColorTab.Length);

            int transparent_r = -1, transparent_g = -1, transparent_b = -1;

            if (_transparent != null)
            {
                _colorTab[256 * 3 - 3] = _transparent.Value.R;
                _colorTab[256 * 3 - 2] = _transparent.Value.G;
                _colorTab[256 * 3 - 1] = _transparent.Value.B;
                _transIndex = 255;
                transparent_r = _transparent.Value.R;
                transparent_g = _transparent.Value.G;
                transparent_b = _transparent.Value.B;
            }

            int k = 0;
            for (int i = 0; i < nPix; i++)
            {
                var r = _pixels[k++] & 0xff;
                var g = _pixels[k++] & 0xff;
                var b = _pixels[k++] & 0xff;

                int index;
                if (r == transparent_r && g == transparent_g && b == transparent_b)
                    index = 255;
                else
                    index = nq.Map(r, g, b);

                _usedEntry[index] = true;
                _indexedPixels[i] = (byte)index;
            }
            _pixels = null;
            _colorDepth = 8;
            _paletteSize = 7;
        }

        /**
         * Returns index of palette color closest to c
         *
         */
        protected int FindClosest(Color c)
        {
            if (_colorTab == null) return -1;
            int r = c.R;
            int g = c.G;
            int b = c.B;
            int minpos = 0;
            int dmin = 256 * 256 * 256;
            int len = _colorTab.Length;
            for (int i = 0; i < len; )
            {
                int dr = r - (_colorTab[i++] & 0xff);
                int dg = g - (_colorTab[i++] & 0xff);
                int db = b - (_colorTab[i] & 0xff);
                int d = dr * dr + dg * dg + db * db;
                int index = i / 3;
                if (_usedEntry[index] && (d < dmin))
                {
                    dmin = d;
                    minpos = index;
                }
                i++;
            }
            return minpos;
        }

        /**
         * Extracts image pixels into byte array "pixels"
         */
        protected void GetImagePixels()
        {
            var rgb24Bitmap = new FormatConvertedBitmap();
            rgb24Bitmap.BeginInit();
            rgb24Bitmap.Source = _currentFrame;
            rgb24Bitmap.DestinationFormat = PixelFormats.Rgb24;
            rgb24Bitmap.EndInit();

            var stride = rgb24Bitmap.Format.BitsPerPixel * rgb24Bitmap.PixelWidth / 8;
            _pixels = new byte[stride * rgb24Bitmap.PixelHeight];
            rgb24Bitmap.CopyPixels(_pixels, stride, 0);
        }

        /**
         * Writes Graphic Control Extension
         */
        protected void WriteGraphicControlExtension()
        {
            _fileStream.WriteByte(0x21); // extension introducer
            _fileStream.WriteByte(0xf9); // GCE label
            _fileStream.WriteByte(4); // data block size
            int transp, disp;
            if (_transparent == null)
            {
                transp = 0;
                disp = 0; // dispose = no action
            }
            else
            {
                transp = 1;
                disp = 2; // force clear if using transparent color
            }
            if (_disposalCode >= 0)
            {
                disp = _disposalCode & 7; // user override
            }
            disp <<= 2;

            // packed fields
            _fileStream.WriteByte(Convert.ToByte(0 | // 1:3 reserved
                disp | // 4:6 disposal
                0 | // 7   user input - 0 = none
                transp)); // 8   transparency flag

            WriteShort(_delay); // delay x 1/100 sec
            _fileStream.WriteByte(Convert.ToByte(_transIndex)); // transparent color index
            _fileStream.WriteByte(0); // block terminator
        }

        /**
         * Writes Image Descriptor
         */
        protected void WriteImageDescriptor()
        {
            _fileStream.WriteByte(0x2c); // image separator
            WriteShort(0); // image position x,y = 0,0
            WriteShort(0);
            WriteShort(_width); // image size
            WriteShort(_height);
            // packed fields
            if (_isFirstFrame)
            {
                // no LCT  - GCT is used for first (or only) frame
                _fileStream.WriteByte(0);
            }
            else
            {
                // specify normal LCT
                _fileStream.WriteByte(Convert.ToByte(0x80 | // 1 local color table  1=yes
                    0 | // 2 interlace - 0=no
                    0 | // 3 sorted - 0=no
                    0 | // 4-5 reserved
                    _paletteSize)); // 6-8 size of color table
            }
        }

        /**
         * Writes Logical Screen Descriptor
         */
        protected void WriteLocalScreenDescriptor()
        {
            // logical screen size
            WriteShort(_width);
            WriteShort(_height);
            // packed fields
            _fileStream.WriteByte(Convert.ToByte(0x80 | // 1   : global color table flag = 1 (gct used)
                0x70 | // 2-4 : color resolution = 7
                0x00 | // 5   : gct sort flag = 0
                _paletteSize)); // 6-8 : gct size

            _fileStream.WriteByte(0); // background color index
            _fileStream.WriteByte(0); // pixel aspect ratio - assume 1:1
        }

        /**
         * Writes Netscape application extension to define
         * repeat count.
         */
        protected void WriteNetscapeExtension()
        {
            _fileStream.WriteByte(0x21); // extension introducer
            _fileStream.WriteByte(0xff); // app extension label
            _fileStream.WriteByte(11); // block size
            WriteString("NETSCAPE" + "2.0"); // app id + auth code
            _fileStream.WriteByte(3); // sub-block size
            _fileStream.WriteByte(1); // loop sub-block id
            WriteShort(_repeat); // loop count (extra iterations, 0=repeat forever)
            _fileStream.WriteByte(0); // block terminator
        }

        /**
         * Writes color table
         */
        protected void WritePalette()
        {
            _fileStream.Write(_colorTab, 0, _colorTab.Length);
            int n = (3 * 256) - _colorTab.Length;
            for (int i = 0; i < n; i++)
            {
                _fileStream.WriteByte(0);
            }
        }

        /**
         * Encodes and writes pixel data
         */
        protected void WritePixels()
        {
            LZWEncoder encoder =
                new LZWEncoder(_width, _height, _indexedPixels, _colorDepth);
            encoder.Encode(_fileStream);
        }

        /**
         *    Write 16-bit value to output stream, LSB first
         */
        protected void WriteShort(int value)
        {
            _fileStream.WriteByte(Convert.ToByte(value & 0xff));
            _fileStream.WriteByte(Convert.ToByte((value >> 8) & 0xff));
        }

        /**
         * Writes string to output stream
         */
        protected void WriteString(String s)
        {
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                _fileStream.WriteByte((byte)chars[i]);
            }
        }
    }

}