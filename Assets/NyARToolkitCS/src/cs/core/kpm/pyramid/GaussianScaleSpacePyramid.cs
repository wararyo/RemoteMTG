﻿/* 
 * PROJECT: NyARToolkit
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *  Copyright 2013-2015 Daqri, LLC.
 *  Author(s): Chris Broaddus
 *
 * The NyARToolkit is Java edition ARToolKit class library.
 *  Copyright (C)2016 Ryo Iizuka
 * 
 * NyARToolkit is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as publishe
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * NyARToolkit is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * As a special exception, the copyright holders of this library give you
 * permission to link this library with independent modules to produce an
 * executable, regardless of the license terms of these independent modules, and to
 * copy and distribute the resulting executable under terms of your choice,
 * provided that you also meet, for each linked independent module, the terms and
 * conditions of the license of that module. An independent module is a module
 * which is neither derived from nor based on this library. If you modify this
 * library, you may extend this exception to your version of the library, but you
 * are not obligated to do so. If you do not wish to do so, delete this exception
 * statement from your version.
 * 
 */
using System;
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{

    public class GaussianScaleSpacePyramid
    {
        /**
         * Number of octaves
         */
        readonly protected int mNumOctaves;
        readonly protected int mNumScalesPerOctave;
        /**
         * 2^(1/(mNumScalesPerOctave-1))
         */
        readonly private double mK;
        /**
         * 1/log(k) precomputed for efficiency
         */
        readonly private double mOneOverLogK;

        public GaussianScaleSpacePyramid(int i_num_octaves, int i_num_scales_per_octaves)
        {
            this.mNumOctaves = i_num_octaves;
            this.mNumScalesPerOctave = i_num_scales_per_octaves;
            this.mK = Math.Pow(2.0, 1.0 / (this.mNumScalesPerOctave - 1));
            this.mOneOverLogK = 1.0 / Math.Log(this.mK);
            return;
        }
        public int numOctaves()
        {
            return mNumOctaves;
        }
        /**
         * Get the number of octaves and scales.
         */
        public int numScalesPerOctave()
        {
            return mNumScalesPerOctave;
        }
        /**
         * Get the constant k-factor.
         */
        public double kfactor()
        {
            return mK;
        }

        /**
         * Get the effective sigma given the octave and sub-pixel scale.
         */
        public double effectiveSigma(int octave, double scale)
        {
            Debug.Assert(scale >= 0);// "Scale must be positive";
            Debug.Assert(scale < this.mNumScalesPerOctave);// "Scale must be less than number of scale per octave";
            return Math.Pow(this.mK, scale) * (1 << octave);
        }

        sealed public class LocateResult
        {
            public int octave;
            public int scale;
        };

        /**
         * Locate a SIGMA on the pyramid.
         */
        public void locate(double sigma, LocateResult result)
        {
            // octave = floor(log2(s))
            int octave = (int)Math.Floor(Math.Log(sigma) / Math.Log(2));
            // scale = logk(s/2^octave)
            // Here we ROUND the scale to an integer value
            double fscale = Math.Log(sigma / (double)(1 << octave))
                    * this.mOneOverLogK;
            int scale = (int)Math.Floor((fscale + 0.5));

            // The last scale in an octave has the same sigma as the first scale
            // of the next octave. We prefer coarser octaves for efficiency.
            if (scale == this.mNumScalesPerOctave - 1)
            {
                // If this octave is out of range, then it will be clipped to
                // be in range on the next step below.
                octave = octave + 1;
                scale = 0;
            }

            // Clip the octave/scale to be in range of the pyramid
            if (octave < 0)
            {
                result.octave = 0;
                result.scale = 0;
            }
            else if (octave >= this.mNumOctaves)
            {
                result.octave = this.mNumOctaves - 1;
                result.scale = this.mNumScalesPerOctave - 1;
            }
            else
            {
                result.octave = octave;
                result.scale = scale;
            }
            Debug.Assert(result.octave >= 0);// , "Octave must be positive");
            Debug.Assert(result.octave < this.mNumOctaves);//  "Octave must be less than number of octaves");
            Debug.Assert(result.scale >= 0);// , "Scale must be positive");
            Debug.Assert(result.scale < this.mNumScalesPerOctave);// "Scale must be less than number of scale per octave");
        }

        protected KpmImage[] mPyramid;

        /**
         * @return Get the vector of images.
         */
        public KpmImage[] images()
        {
            return this.mPyramid;
        }

        /**
         * Get a pyramid image.
         */
        public KpmImage image(int octave, int scale)
        {
            Debug.Assert(octave < mNumOctaves);// "Octave out of range");
            Debug.Assert(scale < mNumScalesPerOctave);// , "Scale out of range");
            return this.mPyramid[octave * mNumScalesPerOctave + scale];
        }

        public int size()
        {
            return this.mPyramid.Length;
        }

        public KpmImage get(int octave, int scale)
        {
            // ASSERT(octave < mNumOctaves, "Octave out of range");
            // ASSERT(scale < mNumScalesPerOctave, "Scale out of range");
            return this.mPyramid[octave * this.mNumScalesPerOctave + scale];
        }


        /**
         * Use this function to downsample a point to an octave that was found from
         * a bilinear downsampled pyramid.
         *
         * @param[out] xp Downsampled x location
         * @param[out] yp Downsampled y location
         * @param[in] x X location on fine image
         * @param[in] y Y location on fine image
         * @param[in] octave The octave to downsample (x,y) to
         */
        public static void bilinear_downsample_point(NyARDoublePoint2d ret,
                                              double x,
                                              double y,
                                              int octave)
        {
            double a, b;
            a = 1.0f / (1 << octave);
            b = 0.5f * a - 0.5f;
            ret.x = x * a + b;
            ret.y = y * a + b;
        }
    }
}
