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
using System;
namespace jp.nyatla.nyartoolkit.cs.core
{


    /**
     * このクラスは、色調テーブルフィルタです。
     * トーンテーブルの内容に従って、画素を置換します。
     * </p>
     */
    public interface INyARGsToneTableFilter
    {
        /**
         * トーンテーブルに、点0,0を通過する、傾きi_aの直線を定義して、フィルタを適応します。
         * <p>設定例-
         * i_aの値をvとしたとき、以下のようになります。
         * <ul>
         * <li>v<=0		黒色
         * <li>0<v<1	暗くする。
         * <li>v=0		変化しない
         * <li>1<v<255	明るくする。
         * <li>255<=v	白色
         * </ul>
         * </p>
         * @param i_a
         * 直線の傾きです。
         */
        void line(double i_a, INyARGrayscaleRaster i_output);
        /**
         * トーンテーブルに、点x,yを通過する、傾きi_aの直線を定義して、フィルタを適応します。
         * @param i_x
         * 直線の通過点
         * @param i_y
         * 直線の通過点
         * @param i_a
         * 直線の傾きです。
         */
        void line(int i_x, int i_y, double i_a, INyARGrayscaleRaster i_output);
        /**
         * 点 i_x,i_yを中心とする、ゲインi_gainのシグモイド関数を定義して、フィルタを適応します。
         * @param i_x
         * 直線の通過点
         * @param i_y
         * 直線の通過点
         * @param i_gain
         * シグモイド関数のゲイン値
         */
        void sigmoid(int i_x, int i_y, double i_gain, INyARGrayscaleRaster i_output);
        /**
         * ガンマ補正値を定義して、フィルタを適応します。
         * @param i_gamma
         * ガンマ値
         */
        void gamma(double i_gamma, INyARGrayscaleRaster i_output);
    }


    class NyARGsToneTableFilter : INyARGsToneTableFilter
    {
        private int[] _table = new int[256];
        private INyARGsCustomToneTableFilter _tone_filter;

        public NyARGsToneTableFilter(INyARGrayscaleRaster i_raster)
        {
            this._tone_filter = NyARGsFilterFactory.createCustomToneTableFilter(i_raster);
        }

        public void line(int i_x, int i_y, double i_a, INyARGrayscaleRaster i_output)
        {
            if (i_a == 0)
            {
                int i;
                for (i = 0; i <= i_x; i++)
                {
                    this._table[i] = 0;
                }
                for (i = 0; i < 256; i++)
                {
                    this._table[i] = 255;
                }
            }
            else
            {
                int b = i_y - (int)(i_a * i_x);
                for (int i = 0; i < 256; i++)
                {
                    int v = (int)(i_a * i) + b;
                    this._table[i] = v < 0 ? 0 : v > 255 ? 255 : v;
                }
            }
            this._tone_filter.doFilter(this._table, i_output);
        }

        public void line(double i_a, INyARGrayscaleRaster i_output)
        {
            this.line(0, 0, i_a, i_output);
        }

        public void sigmoid(int i_x, int i_y, double i_gain, INyARGrayscaleRaster i_output)
        {
            for (int i = 0; i < 256; i++)
            {
                int v = 255 * (int)(1 / (1 + Math.Exp(i_gain * (i - i_x))) - 0.5) + i_y;
                this._table[i] = v < 0 ? 0 : v > 255 ? 255 : v;
            }
            this._tone_filter.doFilter(this._table, i_output);
        }

        public void gamma(double i_gamma, INyARGrayscaleRaster i_output)
        {
            for (int i = 0; i < 256; i++)
            {
                this._table[i] = (int)(Math.Pow((double)i / 255.0, i_gamma) * 255.0);
            }
            this._tone_filter.doFilter(this._table, i_output);
        }
    }
}
