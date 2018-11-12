﻿/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 *
 * The NyARToolkitCS is C# edition NyARToolKit class library.
 * Copyright (C)2008-2012 Ryo Iizuka
 *
 * This work is based on the ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as publishe
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp> or <nyatla(at)nyatla.jp>
 * 
 */
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{


    /**
     * このインタフェイスは、Gaussianフィルタで画像を平滑化します。
     * カーネルサイズは3x3です。
     * <p>対応している画素形式は以下の通りです。
     * <li>{@link NyARBufferType#INT1D_GRAY_8}
     * </p>
     */
    public interface INyARGsGaussianSmoothFilter
    {
        void doFilter(INyARGrayscaleRaster i_output);
    }

    /**
     * 制限事項
     * このクラスは、in/out共にGS8の時のみ動作します。
     */
    class NyARGsGaussianSmoothFilter_GS8 : INyARGsGaussianSmoothFilter
    {
        private INyARGrayscaleRaster _raster;
        public NyARGsGaussianSmoothFilter_GS8(INyARGrayscaleRaster i_raster)
        {
            Debug.Assert(i_raster.isEqualBufferType(NyARBufferType.INT1D_GRAY_8));
            this._raster = i_raster;
        }

        public void doFilter(INyARGrayscaleRaster i_output)
        {
            int[] in_ptr = (int[])this._raster.getBuffer();
            int width = this._raster.getWidth();
            int height = this._raster.getHeight();
            int col0, col1, col2;
            int bptr = 0;
            switch (i_output.getBufferType())
            {
                case NyARBufferType.INT1D_GRAY_8:
                    int[] out_ptr = (int[])i_output.getBuffer();
                    bptr = 0;
                    //1行目
                    col1 = in_ptr[bptr] * 2 + in_ptr[bptr + width];
                    col2 = in_ptr[bptr + 1] * 2 + in_ptr[bptr + width + 1];
                    out_ptr[bptr] = (col1 * 2 + col2) / 9;
                    bptr++;
                    for (int x = 0; x < width - 2; x++)
                    {
                        col0 = col1;
                        col1 = col2;
                        col2 = in_ptr[bptr + 1] * 2 + in_ptr[bptr + width + 1];
                        out_ptr[bptr] = (col0 + col1 * 2 + col2) / 12;
                        bptr++;
                    }
                    out_ptr[bptr] = (col1 + col2) / 9;
                    bptr++;
                    //2行目-末行-1
                    for (int y = 0; y < height - 2; y++)
                    {
                        //左端
                        col1 = in_ptr[bptr] * 2 + in_ptr[bptr - width] + in_ptr[bptr + width];
                        col2 = in_ptr[bptr + 1] * 2 + in_ptr[bptr - width + 1] + in_ptr[bptr + width + 1];
                        out_ptr[bptr] = (col1 + col2) / 12;
                        bptr++;
                        for (int x = 0; x < width - 2; x++)
                        {
                            col0 = col1;
                            col1 = col2;
                            col2 = in_ptr[bptr + 1] * 2 + in_ptr[bptr - width + 1] + in_ptr[bptr + width + 1];
                            out_ptr[bptr] = (col0 + col1 * 2 + col2) / 16;
                            bptr++;
                        }
                        //右端
                        out_ptr[bptr] = (col1 * 2 + col2) / 12;
                        bptr++;
                    }
                    //末行目
                    col1 = in_ptr[bptr] * 2 + in_ptr[bptr - width];
                    col2 = in_ptr[bptr + 1] * 2 + in_ptr[bptr - width + 1];
                    out_ptr[bptr] = (col1 + col2) / 9;
                    bptr++;
                    for (int x = 0; x < width - 2; x++)
                    {
                        col0 = col1;
                        col1 = col2;
                        col2 = in_ptr[bptr + 1] * 2 + in_ptr[bptr - width + 1];
                        out_ptr[bptr] = (col0 + col1 * 2 + col2) / 12;
                        bptr++;
                    }
                    out_ptr[bptr] = (col1 * 2 + col2) / 9;
                    bptr++;
                    return;
                default:
                    throw new NyARRuntimeException();
            }
        }
    }
}
