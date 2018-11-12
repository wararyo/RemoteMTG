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
     * このクラスは、ヒストグラムの平滑化フィルタです。
     */
    public interface INyARGsEqualizeHistFilter
    {
        void doFilter(int i_hist_interval, INyARGrayscaleRaster i_output);
    }




    class NyARGsEqualizeHistFilter_Any : INyARGsEqualizeHistFilter
    {
        private INyARGsCustomToneTableFilter _tone_table;
        private INyARHistogramFromRaster _histdrv;
        private NyARHistogram _histogram = new NyARHistogram(256);
        private readonly int[] _hist = new int[256];

        public NyARGsEqualizeHistFilter_Any(INyARGrayscaleRaster i_raster)
        {
            this._tone_table = NyARGsFilterFactory.createCustomToneTableFilter(i_raster);
            this._histdrv = (INyARHistogramFromRaster)i_raster.createInterface(typeof(INyARHistogramFromRaster));
        }
        public void doFilter(int i_hist_interval, INyARGrayscaleRaster i_output)
        {
            //ヒストグラムを得る
            NyARHistogram hist = this._histogram;
            this._histdrv.createHistogram(i_hist_interval, hist);
            //変換テーブルを作成
            int hist_total = this._histogram.total_of_data;
            int min = hist.getMinData();
            int hist_size = this._histogram.length;
            int sum = 0;
            for (int i = 0; i < hist_size; i++)
            {
                sum += hist.data[i];
                this._hist[i] = (int)((sum - min) * (hist_size - 1) / ((hist_total - min)));
            }
            //変換
            this._tone_table.doFilter(this._hist, i_output);
            return;
        }
    }
}
