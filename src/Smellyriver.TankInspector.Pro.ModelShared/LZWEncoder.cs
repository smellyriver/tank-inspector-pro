using System;
using System.IO;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    class LZWEncoder
    {

        private static readonly int s_eof = -1;

        private int _imgW, _imgH;
        private byte[] _pixels;
        private int _initCodeSize;
        private int _remaining;
        private int _currentPixel;

        // GIFCOMPR.C       - GIF Image compression routines
        //
        // Lempel-Ziv compression based on 'compress'.  GIF modifications by
        // David Rowley (mgardi@watdcsu.waterloo.edu)

        // General DEFINEs

        static readonly int BITS = 12;

        static readonly int HSIZE = 5003; // 80% occupancy

        // GIF Image compression - modified 'compress'
        //
        // Based on: compress.c - File compression ala IEEE Computer, June 1984.
        //
        // By Authors:  Spencer W. Thomas      (decvax!harpo!utah-cs!utah-gr!thomas)
        //              Jim McKie              (decvax!mcvax!jim)
        //              Steve Davies           (decvax!vax135!petsd!peora!srd)
        //              Ken Turkowski          (decvax!decwrl!turtlevax!ken)
        //              James A. Woods         (decvax!ihnp4!ames!jaw)
        //              Joe Orost              (decvax!vax135!petsd!joe)

        int _nBits; // number of bits/code
        int _maxBits = BITS; // user settable max # bits/code
        int _maxCode; // maximum code, given n_bits
        int _maxMaxCode = 1 << BITS; // should NEVER generate this code

        int[] _hTab = new int[HSIZE];
        int[] _codeTab = new int[HSIZE];

        int _hSize = HSIZE; // for dynamic table sizing

        int _freeEntry = 0; // first unused entry

        // block compression parameters -- after all codes are used up,
        // and compression rate changes, start over.
        bool _clearFlag = false;

        // Algorithm:  use open addressing double hashing (no chaining) on the
        // prefix code / next character combination.  We do a variant of Knuth's
        // algorithm D (vol. 3, sec. 6.4) along with G. Knott's relatively-prime
        // secondary probe.  Here, the modular division first probe is gives way
        // to a faster exclusive-or manipulation.  Also do block compression with
        // an adaptive reset, whereby the code table is cleared when the compression
        // ratio decreases, but after the table fills.  The variable-length output
        // codes are re-sized at this point, and a special CLEAR code is generated
        // for the decompressor.  Late addition:  construct the table according to
        // file size for noticeable speed improvement on small files.  Please direct
        // questions about this implementation to ames!jaw.

        int _gInitBits;

        int _clearCode;
        int _eofCode;

        // output
        //
        // Output the given code.
        // Inputs:
        //      code:   A n_bits-bit integer.  If == -1, then EOF.  This assumes
        //              that n_bits =< wordsize - 1.
        // Outputs:
        //      Outputs code to the file.
        // Assumptions:
        //      Chars are 8 bits long.
        // Algorithm:
        //      Maintain a BITS character long buffer (so that 8 codes will
        // fit in it exactly).  Use the VAX insv instruction to insert each
        // code in turn.  When the buffer fills up empty it and start over.

        int _currentAccum = 0;
        int _currentBits = 0;

        int[] _masks =
		{
			0x0000,
			0x0001,
			0x0003,
			0x0007,
			0x000F,
			0x001F,
			0x003F,
			0x007F,
			0x00FF,
			0x01FF,
			0x03FF,
			0x07FF,
			0x0FFF,
			0x1FFF,
			0x3FFF,
			0x7FFF,
			0xFFFF 
        };

        // Number of characters so far in this 'packet'
        int _aCount;

        // Define the storage for the packet accumulator
        byte[] _accum = new byte[256];

        //----------------------------------------------------------------------------
        public LZWEncoder(int width, int height, byte[] pixels, int color_depth)
        {
            _imgW = width;
            _imgH = height;
            _pixels = pixels;
            _initCodeSize = Math.Max(2, color_depth);
        }

        // Add a character to the end of the current packet, and if it is 254
        // characters, flush the packet to disk.
        void Add(byte c, Stream outs)
        {
            _accum[_aCount++] = c;
            if (_aCount >= 254)
                Flush(outs);
        }

        // Clear out the hash table

        // table clear for block compress
        void ClearTable(Stream outs)
        {
            ResetCodeTable(_hSize);
            _freeEntry = _clearCode + 2;
            _clearFlag = true;

            Output(_clearCode, outs);
        }

        // reset code table
        void ResetCodeTable(int hsize)
        {
            for (int i = 0; i < hsize; ++i)
                _hTab[i] = -1;
        }

        void Compress(int init_bits, Stream outs)
        {
            int fcode;
            int i /* = 0 */;
            int c;
            int ent;
            int disp;
            int hsize_reg;
            int hshift;

            // Set up the globals:  g_init_bits - initial number of bits
            _gInitBits = init_bits;

            // Set up the necessary values
            _clearFlag = false;
            _nBits = _gInitBits;
            _maxCode = MaxCode(_nBits);

            _clearCode = 1 << (init_bits - 1);
            _eofCode = _clearCode + 1;
            _freeEntry = _clearCode + 2;

            _aCount = 0; // clear packet

            ent = NextPixel();

            hshift = 0;
            for (fcode = _hSize; fcode < 65536; fcode *= 2)
                ++hshift;
            hshift = 8 - hshift; // set hash code range bound

            hsize_reg = _hSize;
            ResetCodeTable(hsize_reg); // clear hash table

            Output(_clearCode, outs);

        outer_loop: while ((c = NextPixel()) != s_eof)
            {
                fcode = (c << _maxBits) + ent;
                i = (c << hshift) ^ ent; // xor hashing

                if (_hTab[i] == fcode)
                {
                    ent = _codeTab[i];
                    continue;
                }
                else if (_hTab[i] >= 0) // non-empty slot
                {
                    disp = hsize_reg - i; // secondary hash (after G. Knott)
                    if (i == 0)
                        disp = 1;
                    do
                    {
                        if ((i -= disp) < 0)
                            i += hsize_reg;

                        if (_hTab[i] == fcode)
                        {
                            ent = _codeTab[i];
                            goto outer_loop;
                        }
                    } while (_hTab[i] >= 0);
                }
                Output(ent, outs);
                ent = c;
                if (_freeEntry < _maxMaxCode)
                {
                    _codeTab[i] = _freeEntry++; // code -> hashtable
                    _hTab[i] = fcode;
                }
                else
                    ClearTable(outs);
            }
            // Put out the final code.
            Output(ent, outs);
            Output(_eofCode, outs);
        }

        //----------------------------------------------------------------------------
        public void Encode(Stream os)
        {
            os.WriteByte(Convert.ToByte(_initCodeSize)); // write "initial code size" byte

            _remaining = _imgW * _imgH; // reset navigation variables
            _currentPixel = 0;

            Compress(_initCodeSize + 1, os); // compress and write the pixel data

            os.WriteByte(0); // write block terminator
        }

        // Flush the packet to disk, and reset the accumulator
        void Flush(Stream outs)
        {
            if (_aCount > 0)
            {
                outs.WriteByte(Convert.ToByte(_aCount));
                outs.Write(_accum, 0, _aCount);
                _aCount = 0;
            }
        }

        int MaxCode(int n_bits)
        {
            return (1 << n_bits) - 1;
        }

        //----------------------------------------------------------------------------
        // Return the next pixel from the image
        //----------------------------------------------------------------------------
        private int NextPixel()
        {
            if (_remaining == 0)
                return s_eof;

            --_remaining;

            int temp = _currentPixel + 1;
            if (temp < _pixels.GetUpperBound(0))
            {
                byte pix = _pixels[_currentPixel++];

                return pix & 0xff;
            }
            return 0xff;
        }

        void Output(int code, Stream outs)
        {
            _currentAccum &= _masks[_currentBits];

            if (_currentBits > 0)
                _currentAccum |= (code << _currentBits);
            else
                _currentAccum = code;

            _currentBits += _nBits;

            while (_currentBits >= 8)
            {
                Add((byte)(_currentAccum & 0xff), outs);
                _currentAccum >>= 8;
                _currentBits -= 8;
            }

            // If the next entry is going to be too big for the code size,
            // then increase it, if possible.
            if (_freeEntry > _maxCode || _clearFlag)
            {
                if (_clearFlag)
                {
                    _maxCode = MaxCode(_nBits = _gInitBits);
                    _clearFlag = false;
                }
                else
                {
                    ++_nBits;
                    if (_nBits == _maxBits)
                        _maxCode = _maxMaxCode;
                    else
                        _maxCode = MaxCode(_nBits);
                }
            }

            if (code == _eofCode)
            {
                // At EOF, write the rest of the buffer.
                while (_currentBits > 0)
                {
                    Add((byte)(_currentAccum & 0xff), outs);
                    _currentAccum >>= 8;
                    _currentBits -= 8;
                }

                Flush(outs);
            }
        }
    }
}