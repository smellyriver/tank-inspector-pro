// ReSharper disable All

#pragma warning disable 0168

using System;

namespace Smellyriver.TankInspector.Pro.ModelShared
{
    class NeuQuant
    {
        protected static readonly int s_netsize = 255; /* number of colours used */
        /* four primes near 500 - assume no image has a length so large */
        /* that it is divisible by all four primes */
        protected static readonly int s_prime1 = 499;
        protected static readonly int s_prime2 = 491;
        protected static readonly int s_prime3 = 487;
        protected static readonly int s_prime4 = 503;
        protected static readonly int s_minPictureBytes = (3 * s_prime4);
        /* minimum size for input image */
        /* Program Skeleton
           ----------------
           [select samplefac in range 1..30]
           [read image from input file]
           pic = (unsigned char*) malloc(3*width*height);
           initnet(pic,3*width*height,samplefac);
           learn();
           unbiasnet();
           [write output image header, using writecolourmap(f)]
           inxbuild();
           write output image using inxsearch(b,g,r)      */

        /* Network Definitions
           ------------------- */
        protected static readonly int s_maxNetPos = (s_netsize - 1);
        protected static readonly int s_netBiasShift = 4; /* bias for colour values */
        protected static readonly int s_nCycles = 100; /* no. of learning cycles */

        /* defs for freq and bias */
        protected static readonly int s_intBiasShift = 16; /* bias for fractions */
        protected static readonly int s_intBias = (((int)1) << s_intBiasShift);
        protected static readonly int s_gammaShift = 10; /* gamma = 1024 */
        protected static readonly int s_gamma = (((int)1) << s_gammaShift);
        protected static readonly int s_betaShift = 10;
        protected static readonly int s_beta = (s_intBias >> s_betaShift); /* beta = 1/1024 */
        protected static readonly int s_betaGamma = (s_intBias << (s_gammaShift - s_betaShift));

        /* defs for decreasing radius factor */
        protected static readonly int s_initRad = (s_netsize >> 3); /* for 256 cols, radius starts */
        protected static readonly int s_radiusBiasShift = 6; /* at 32.0 biased by 6 bits */
        protected static readonly int s_radiusBias = (((int)1) << s_radiusBiasShift);
        protected static readonly int s_initRadius = (s_initRad * s_radiusBias); /* and decreases by a */
        protected static readonly int s_radiusDec = 30; /* factor of 1/30 each cycle */

        /* defs for decreasing alpha factor */
        protected static readonly int s_alphaBiasShift = 10; /* alpha starts at 1.0 */
        protected static readonly int s_initAlpha = (((int)1) << s_alphaBiasShift);

        protected int alphadec; /* biased by 10 bits */

        /* radbias and alpharadbias used for radpower calculation */
        protected static readonly int s_radBiasShift = 8;
        protected static readonly int s_radBias = (((int)1) << s_radBiasShift);
        protected static readonly int s_alphaRadBiasShift = (s_alphaBiasShift + s_radBiasShift);
        protected static readonly int s_alphaRadBias = (((int)1) << s_alphaRadBiasShift);

        /* Types and Global Variables
        -------------------------- */

        protected byte[] _imageData; /* the input image itself */
        protected int _lengthCount; /* lengthcount = H*W*3 */

        protected int _sampleFac; /* sampling factor 1..30 */

        //   typedef int pixel[4];                /* BGRc */
        protected int[][] _network; /* the network itself - [netsize][4] */

        protected int[] _netIndex = new int[256];
        /* for network lookup - really 256 */

        protected int[] _bias = new int[s_netsize];
        /* bias and freq arrays for learning */
        protected int[] _freq = new int[s_netsize];
        protected int[] _radPower = new int[s_initRad];
        /* radpower for precomputation */

        /* Initialise network in range (0,0,0) to (255,255,255) and set parameters
           ----------------------------------------------------------------------- */
        public NeuQuant(byte[] thepic, int len, int sample)
        {

            int i;
            int[] p;

            _imageData = thepic;
            _lengthCount = len;
            _sampleFac = sample;

            _network = new int[s_netsize][];
            for (i = 0; i < s_netsize; i++)
            {
                _network[i] = new int[4];
                p = _network[i];
                p[0] = p[1] = p[2] = (i << (s_netBiasShift + 8)) / s_netsize;
                _freq[i] = s_intBias / s_netsize; /* 1/netsize */
                _bias[i] = 0;
            }
        }

        public byte[] ColorMap()
        {
            byte[] map = new byte[3 * s_netsize];
            int[] index = new int[s_netsize];
            for (int i = 0; i < s_netsize; i++)
                index[_network[i][3]] = i;
            int k = 0;
            for (int i = 0; i < s_netsize; i++)
            {
                int j = index[i];
                map[k++] = (byte)(_network[j][0]);
                map[k++] = (byte)(_network[j][1]);
                map[k++] = (byte)(_network[j][2]);
            }
            return map;
        }

        /* Insertion sort of network and building of netindex[0..255] (to do after unbias)
           ------------------------------------------------------------------------------- */
        public void Inxbuild()
        {

            int i, j, smallpos, smallval;
            int[] p;
            int[] q;
            int previouscol, startpos;

            previouscol = 0;
            startpos = 0;
            for (i = 0; i < s_netsize; i++)
            {
                p = _network[i];
                smallpos = i;
                smallval = p[1]; /* index on g */
                /* find smallest in i..netsize-1 */
                for (j = i + 1; j < s_netsize; j++)
                {
                    q = _network[j];
                    if (q[1] < smallval)
                    { /* index on g */
                        smallpos = j;
                        smallval = q[1]; /* index on g */
                    }
                }
                q = _network[smallpos];
                /* swap p (i) and q (smallpos) entries */
                if (i != smallpos)
                {
                    j = q[0];
                    q[0] = p[0];
                    p[0] = j;
                    j = q[1];
                    q[1] = p[1];
                    p[1] = j;
                    j = q[2];
                    q[2] = p[2];
                    p[2] = j;
                    j = q[3];
                    q[3] = p[3];
                    p[3] = j;
                }
                /* smallval entry is now in position i */
                if (smallval != previouscol)
                {
                    _netIndex[previouscol] = (startpos + i) >> 1;
                    for (j = previouscol + 1; j < smallval; j++)
                        _netIndex[j] = i;
                    previouscol = smallval;
                    startpos = i;
                }
            }
            _netIndex[previouscol] = (startpos + s_maxNetPos) >> 1;
            for (j = previouscol + 1; j < 256; j++)
                _netIndex[j] = s_maxNetPos; /* really 256 */
        }

        /* Main Learning Loop
           ------------------ */
        public void Learn()
        {

            int i, j, b, g, r;
            int radius, rad, alpha, step, delta, samplepixels;
            byte[] p;
            int pix, lim;

            if (_lengthCount < s_minPictureBytes)
                _sampleFac = 1;
            alphadec = 30 + ((_sampleFac - 1) / 3);
            p = _imageData;
            pix = 0;
            lim = _lengthCount;
            samplepixels = _lengthCount / (3 * _sampleFac);
            delta = samplepixels / s_nCycles;
            alpha = s_initAlpha;
            radius = s_initRadius;

            rad = radius >> s_radiusBiasShift;
            if (rad <= 1)
                rad = 0;
            for (i = 0; i < rad; i++)
                _radPower[i] =
                    alpha * (((rad * rad - i * i) * s_radBias) / (rad * rad));

            //fprintf(stderr,"beginning 1D learning: initial radius=%d\n", rad);

            if (_lengthCount < s_minPictureBytes)
                step = 3;
            else if ((_lengthCount % s_prime1) != 0)
                step = 3 * s_prime1;
            else
            {
                if ((_lengthCount % s_prime2) != 0)
                    step = 3 * s_prime2;
                else
                {
                    if ((_lengthCount % s_prime3) != 0)
                        step = 3 * s_prime3;
                    else
                        step = 3 * s_prime4;
                }
            }

            i = 0;
            while (i < samplepixels)
            {
                b = (p[pix + 0] & 0xff) << s_netBiasShift;
                g = (p[pix + 1] & 0xff) << s_netBiasShift;
                r = (p[pix + 2] & 0xff) << s_netBiasShift;
                j = Contest(b, g, r);

                Altersingle(alpha, j, b, g, r);
                if (rad != 0)
                    Alterneigh(rad, j, b, g, r); /* alter neighbours */

                pix += step;
                if (pix >= lim)
                    pix -= _lengthCount;

                i++;
                if (delta == 0)
                    delta = 1;
                if (i % delta == 0)
                {
                    alpha -= alpha / alphadec;
                    radius -= radius / s_radiusDec;
                    rad = radius >> s_radiusBiasShift;
                    if (rad <= 1)
                        rad = 0;
                    for (j = 0; j < rad; j++)
                        _radPower[j] =
                            alpha * (((rad * rad - j * j) * s_radBias) / (rad * rad));
                }
            }
            //fprintf(stderr,"finished 1D learning: readonly alpha=%f !\n",((float)alpha)/initalpha);
        }

        /* Search for BGR values 0..255 (after net is unbiased) and return colour index
           ---------------------------------------------------------------------------- */
        public int Map(int b, int g, int r)
        {

            int i, j, dist, a, bestd;
            int[] p;
            int best;

            bestd = 1000; /* biggest possible dist is 256*3 */
            best = -1;
            i = _netIndex[g]; /* index on g */
            j = i - 1; /* start at netindex[g] and work outwards */

            while ((i < s_netsize) || (j >= 0))
            {
                if (i < s_netsize)
                {
                    p = _network[i];
                    dist = p[1] - g; /* inx key */
                    if (dist >= bestd)
                        i = s_netsize; /* stop iter */
                    else
                    {
                        i++;
                        if (dist < 0)
                            dist = -dist;
                        a = p[0] - b;
                        if (a < 0)
                            a = -a;
                        dist += a;
                        if (dist < bestd)
                        {
                            a = p[2] - r;
                            if (a < 0)
                                a = -a;
                            dist += a;
                            if (dist < bestd)
                            {
                                bestd = dist;
                                best = p[3];
                            }
                        }
                    }
                }
                if (j >= 0)
                {
                    p = _network[j];
                    dist = g - p[1]; /* inx key - reverse dif */
                    if (dist >= bestd)
                        j = -1; /* stop iter */
                    else
                    {
                        j--;
                        if (dist < 0)
                            dist = -dist;
                        a = p[0] - b;
                        if (a < 0)
                            a = -a;
                        dist += a;
                        if (dist < bestd)
                        {
                            a = p[2] - r;
                            if (a < 0)
                                a = -a;
                            dist += a;
                            if (dist < bestd)
                            {
                                bestd = dist;
                                best = p[3];
                            }
                        }
                    }
                }
            }
            return (best);
        }
        public byte[] Process()
        {
            Learn();
            Unbiasnet();
            Inxbuild();
            return ColorMap();
        }

        /* Unbias network to give byte values 0..255 and record position i to prepare for sort
           ----------------------------------------------------------------------------------- */
        public void Unbiasnet()
        {

            int i, j;

            for (i = 0; i < s_netsize; i++)
            {
                _network[i][0] >>= s_netBiasShift;
                _network[i][1] >>= s_netBiasShift;
                _network[i][2] >>= s_netBiasShift;
                _network[i][3] = i; /* record colour no */
            }
        }

        /* Move adjacent neurons by precomputed alpha*(1-((i-j)^2/[r]^2)) in radpower[|i-j|]
           --------------------------------------------------------------------------------- */
        protected void Alterneigh(int rad, int i, int b, int g, int r)
        {

            int j, k, lo, hi, a, m;
            int[] p;

            lo = i - rad;
            if (lo < -1)
                lo = -1;
            hi = i + rad;
            if (hi > s_netsize)
                hi = s_netsize;

            j = i + 1;
            k = i - 1;
            m = 1;
            while ((j < hi) || (k > lo))
            {
                a = _radPower[m++];
                if (j < hi)
                {
                    p = _network[j++];
                    try
                    {
                        p[0] -= (a * (p[0] - b)) / s_alphaRadBias;
                        p[1] -= (a * (p[1] - g)) / s_alphaRadBias;
                        p[2] -= (a * (p[2] - r)) / s_alphaRadBias;
                    }
                    catch (Exception e)
                    {
                    } // prevents 1.3 miscompilation
                }
                if (k > lo)
                {
                    p = _network[k--];
                    try
                    {
                        p[0] -= (a * (p[0] - b)) / s_alphaRadBias;
                        p[1] -= (a * (p[1] - g)) / s_alphaRadBias;
                        p[2] -= (a * (p[2] - r)) / s_alphaRadBias;
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        /* Move neuron i towards biased (b,g,r) by factor alpha
           ---------------------------------------------------- */
        protected void Altersingle(int alpha, int i, int b, int g, int r)
        {

            /* alter hit neuron */
            int[] n = _network[i];
            n[0] -= (alpha * (n[0] - b)) / s_initAlpha;
            n[1] -= (alpha * (n[1] - g)) / s_initAlpha;
            n[2] -= (alpha * (n[2] - r)) / s_initAlpha;
        }

        /* Search for biased BGR values
           ---------------------------- */
        protected int Contest(int b, int g, int r)
        {

            /* finds closest neuron (min dist) and updates freq */
            /* finds best neuron (min dist-bias) and returns position */
            /* for frequently chosen neurons, freq[i] is high and bias[i] is negative */
            /* bias[i] = gamma*((1/netsize)-freq[i]) */

            int i, dist, a, biasdist, betafreq;
            int bestpos, bestbiaspos, bestd, bestbiasd;
            int[] n;

            bestd = ~(((int)1) << 31);
            bestbiasd = bestd;
            bestpos = -1;
            bestbiaspos = bestpos;

            for (i = 0; i < s_netsize; i++)
            {
                n = _network[i];
                dist = n[0] - b;
                if (dist < 0)
                    dist = -dist;
                a = n[1] - g;
                if (a < 0)
                    a = -a;
                dist += a;
                a = n[2] - r;
                if (a < 0)
                    a = -a;
                dist += a;
                if (dist < bestd)
                {
                    bestd = dist;
                    bestpos = i;
                }
                biasdist = dist - ((_bias[i]) >> (s_intBiasShift - s_netBiasShift));
                if (biasdist < bestbiasd)
                {
                    bestbiasd = biasdist;
                    bestbiaspos = i;
                }
                betafreq = (_freq[i] >> s_betaShift);
                _freq[i] -= betafreq;
                _bias[i] += (betafreq << s_gammaShift);
            }
            _freq[bestpos] += s_beta;
            _bias[bestpos] -= s_betaGamma;
            return (bestbiaspos);
        }
    }
}