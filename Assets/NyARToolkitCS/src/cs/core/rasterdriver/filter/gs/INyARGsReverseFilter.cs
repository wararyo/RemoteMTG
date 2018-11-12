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
namespace jp.nyatla.nyartoolkit.cs.core
{



    /**
     * このインタフェイスは、ネガポジ反転機能を提供します。
     * <p>対応している画素形式は以下の通りです。
     * <li>{@link NyARBufferType#INT1D_GRAY_8}
     * </p>
     */
    public interface INyARGsReverseFilter
    {
        void doFilter(INyARGrayscaleRaster i_output);
    }
    class NyARGsReverseFilter_Any : INyARGsReverseFilter
    {
        private INyARGrayscaleRaster _raster;
        public NyARGsReverseFilter_Any(INyARGrayscaleRaster i_raster)
        {
            this._raster = i_raster;
        }
        public void doFilter(INyARGrayscaleRaster i_output)
        {
            INyARGrayscaleRaster ind = this._raster;
            INyARGrayscaleRaster outd = i_output;
            NyARIntSize s = this._raster.getSize();
            for (int y = s.h - 1; y >= 0; y--)
            {
                for (int x = s.w - 1; x >= 0; x--)
                {
                    outd.setPixel(x, y, 255 - ind.getPixel(x, y));
                }
            }
        }
    }
}
