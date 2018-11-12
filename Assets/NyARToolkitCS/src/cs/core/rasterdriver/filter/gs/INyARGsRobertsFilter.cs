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
     * このインタフェイスは、Roberts法で勾配画像を作ります。
     * 右端と左端の1ピクセルは、常に0が入ります。
     * <p>対応している画素形式は以下の通りです。
     * <li>{@link NyARBufferType#INT1D_GRAY_8}
     * </p>
     * <pre>
     * X=|-1, 0|  Y=|0,-1|
     *   | 0, 1|    |1, 0|
     * V=sqrt(X^2+Y+2)/2
     * </pre>
     */
    public interface INyARGsRobertsFilter
    {
        void doFilter(INyARGrayscaleRaster i_output);
    }

    class NyARGsRobertsFilter_GS8 : INyARGsRobertsFilter
    {
        private INyARGrayscaleRaster _raster;
        public NyARGsRobertsFilter_GS8(INyARGrayscaleRaster i_raster)
        {
            this._raster = i_raster;
        }
        public void doFilter(INyARGrayscaleRaster i_output)
        {
            Debug.Assert(i_output.isEqualBufferType(NyARBufferType.INT1D_GRAY_8));
            NyARIntSize size = this._raster.getSize();
            int[] in_ptr = (int[])this._raster.getBuffer();
            switch (this._raster.getBufferType())
            {
                case NyARBufferType.INT1D_GRAY_8:
                    int[] out_ptr = (int[])i_output.getBuffer();
                    int width = size.w;
                    int idx = 0;
                    int idx2 = width;
                    int fx, fy;
                    int mod_p = (width - 2) - (width - 2) % 8;
                    for (int y = size.h - 2; y >= 0; y--)
                    {
                        int p00 = in_ptr[idx++];
                        int p10 = in_ptr[idx2++];
                        int p01, p11;
                        int x = width - 2;
                        for (; x >= mod_p; x--)
                        {
                            p01 = in_ptr[idx++]; p11 = in_ptr[idx2++];
                            fx = p11 - p00; fy = p10 - p01;
                            out_ptr[idx - 2] = ((fx < 0 ? -fx : fx) + (fy < 0 ? -fy : fy)) >> 1;
                            p00 = p01;
                            p10 = p11;
                        }
                        for (; x >= 0; x -= 4)
                        {
                            p01 = in_ptr[idx++]; p11 = in_ptr[idx2++];
                            fx = p11 - p00;
                            fy = p10 - p01;
                            out_ptr[idx - 2] = ((fx < 0 ? -fx : fx) + (fy < 0 ? -fy : fy)) >> 1;
                            p00 = p01; p10 = p11;

                            p01 = in_ptr[idx++]; p11 = in_ptr[idx2++];
                            fx = p11 - p00;
                            fy = p10 - p01;
                            out_ptr[idx - 2] = ((fx < 0 ? -fx : fx) + (fy < 0 ? -fy : fy)) >> 1;
                            p00 = p01; p10 = p11;
                            p01 = in_ptr[idx++]; p11 = in_ptr[idx2++];

                            fx = p11 - p00;
                            fy = p10 - p01;
                            out_ptr[idx - 2] = ((fx < 0 ? -fx : fx) + (fy < 0 ? -fy : fy)) >> 1;
                            p00 = p01; p10 = p11;

                            p01 = in_ptr[idx++]; p11 = in_ptr[idx2++];
                            fx = p11 - p00;
                            fy = p10 - p01;
                            out_ptr[idx - 2] = ((fx < 0 ? -fx : fx) + (fy < 0 ? -fy : fy)) >> 1;
                            p00 = p01; p10 = p11;

                        }
                        out_ptr[idx - 1] = 0;
                    }
                    for (int x = width - 1; x >= 0; x--)
                    {
                        out_ptr[idx++] = 0;
                    }
                    return;
                default:
                    //ANY未対応
                    throw new NyARRuntimeException();
            }
        }
    }
}
