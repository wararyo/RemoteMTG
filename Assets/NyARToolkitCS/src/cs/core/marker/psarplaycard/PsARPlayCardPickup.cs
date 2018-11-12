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
using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.nyidmarker;
using System.Diagnostics;

namespace jp.nyatla.nyartoolkit.cs.psarplaycard
{



    /**
     * このクラスは、ラスタ画像に定義したの任意矩形から、PsARPlayCardパターンのデータを読み取ります。
     *
     */
    public class PsARPlayCardPickup
    {
        public class PsArIdParam
        {
            public int id;
            public int direction;
        }
        private PerspectivePixelReader _perspective_reader;
        private MarkerPattDecoder _decoder = new MarkerPattDecoder();

        /**
         * コンストラクタです。インスタンスを生成します。
         * @throws NyARException 
         */
        public PsARPlayCardPickup()
        {
            this._perspective_reader = new PerspectivePixelReader();
            return;
        }

        /**
         * この関数は、ラスタドライバから画像を読み出します。
         * @param i_pix_drv
         * @param i_size
         * @param i_vertex
         * @param o_data
         * @param o_param
         * @return
         * @throws NyARException
         */
        public bool getARPlayCardId(INyARGrayscaleRaster i_raster, NyARIntPoint2d[] i_vertex, PsArIdParam i_result)
        {
            if (!this._perspective_reader.setSourceSquare(i_vertex))
            {
                return false;
            }
            return this._pickFromRaster(i_raster, i_result);
        }
        /**
         * この関数は、ラスタドライバから画像を読み出します。
         * @param i_pix_drv
         * @param i_size
         * @param i_vertex
         * @param o_data
         * @param o_param
         * @return
         * @throws NyARException
         */
        public bool getARPlayCardId(INyARGrayscaleRaster i_raster, NyARDoublePoint2d[] i_vertex, PsArIdParam i_result)
        {
            if (!this._perspective_reader.setSourceSquare(i_vertex))
            {
                return false;
            }
            return this._pickFromRaster(i_raster, i_result);
        }
        /**
         * i_imageから、idマーカを読みだします。
         * o_dataにはマーカデータ、o_paramにはマーカのパラメータを返却します。
         * @param image
         * @param i_vertex
         * @param o_data
         * @param o_param
         * @return
         * @throws NyARException
         */
        private bool _pickFromRaster(INyARGrayscaleRaster i_raster, PsArIdParam i_result)
        {
            if (!this._perspective_reader.readDataBits(i_raster, this._decoder))
            {
                return false;
            }
            //敷居値検索
            return this._decoder.decodePatt(i_result);
        }
    }



    /**
     * PSARIdを100x100画像として読み出す。
     */
    sealed class PerspectivePixelReader
    {
        private static int READ_RESOLUTION = 100;
        private NyARPerspectiveParamGenerator _param_gen = new NyARPerspectiveParamGenerator_O1(1, 1);
        private double[] _cparam = new double[8];

        /**
         * コンストラクタです。
         */
        public PerspectivePixelReader()
        {
            return;
        }
        /**
         * この関数は、マーカ四角形をインスタンスにセットします。
         * @param i_vertex
         * セットする四角形頂点座標。4要素である必要があります。
         * @return
         * 成功するとtrueです。
         * @throws NyARException
         */
        public bool setSourceSquare(NyARIntPoint2d[] i_vertex)
        {
            return this._param_gen.getParam(READ_RESOLUTION, READ_RESOLUTION, i_vertex, this._cparam);
        }
        /**
         * この関数は、マーカ四角形をインスタンスにセットします。
         * @param i_vertex
         * セットする四角形頂点座標。4要素である必要があります。
         * @return
         * 成功するとtrueです。
         * @throws NyARException
         */
        public bool setSourceSquare(NyARDoublePoint2d[] i_vertex)
        {
            return this._param_gen.getParam(READ_RESOLUTION, READ_RESOLUTION, i_vertex, this._cparam);
        }


        //タイミングパターン用のパラメタ(FRQ_POINTS*FRQ_STEPが100を超えないようにすること)

        private const int MAX_FREQ = 10;
        private const int MAX_DATA_BITS = MAX_FREQ + MAX_FREQ - 1;

        private int[] _ref_x = new int[108];
        private int[] _ref_y = new int[108];
        //(model+1)*4とTHRESHOLD_PIXELのどちらか大きい方
        private int[] _pixcel_temp = new int[108];


        private void detectDataBitsIndex(double[] o_index_row, double[] o_index_col)
        {
            for (int i = 0; i < 3; i++)
            {
                o_index_row[i * 2] = 25 + i * 20;
                o_index_row[i * 2 + 1] = 35 + i * 20;
                o_index_col[i * 2] = 25 + i * 20;
                o_index_col[i * 2 + 1] = 35 + i * 20;
            }
        }
        private double[] __readDataBits_index_bit_x = new double[MAX_DATA_BITS * 2];
        private double[] __readDataBits_index_bit_y = new double[MAX_DATA_BITS * 2];
        /**
         * この関数は、マーカパターンからデータを読み取ります。
         * @param i_reader
         * ラスタリーダ
         * @param i_raster_size
         * ラスタのサイズ
         * @param o_bitbuffer
         * データビットの出力先
         * @return
         * 成功するとtrue
         * @throws NyARException
         */
        public bool readDataBits(INyARGrayscaleRaster i_raster, MarkerPattDecoder o_bitbuffer)
        {
            int raster_width = i_raster.getWidth();
            int raster_height = i_raster.getHeight();

            double[] index_x = this.__readDataBits_index_bit_x;
            double[] index_y = this.__readDataBits_index_bit_y;


            //読み出し位置を取得
            detectDataBitsIndex(index_x, index_y);
            int resolution = 3;

            double[] cpara = this._cparam;
            int[] ref_x = this._ref_x;
            int[] ref_y = this._ref_y;
            int[] pixcel_temp = this._pixcel_temp;

            double cpara_0 = cpara[0];
            double cpara_1 = cpara[1];
            double cpara_3 = cpara[3];
            double cpara_6 = cpara[6];


            int p = 0;
            for (int i = 0; i < resolution; i++)
            {
                //1列分のピクセルのインデックス値を計算する。
                double cy0 = 1 + index_y[i * 2 + 0];
                double cy1 = 1 + index_y[i * 2 + 1];
                double cpy0_12 = cpara_1 * cy0 + cpara[2];
                double cpy0_45 = cpara[4] * cy0 + cpara[5];
                double cpy0_7 = cpara[7] * cy0 + 1.0;
                double cpy1_12 = cpara_1 * cy1 + cpara[2];
                double cpy1_45 = cpara[4] * cy1 + cpara[5];
                double cpy1_7 = cpara[7] * cy1 + 1.0;

                int pt = 0;
                for (int i2 = 0; i2 < resolution; i2++)
                {
                    int xx, yy;
                    double d;
                    double cx0 = 1 + index_x[i2 * 2 + 0];
                    double cx1 = 1 + index_x[i2 * 2 + 1];

                    double cp6_0 = cpara_6 * cx0;
                    double cpx0_0 = cpara_0 * cx0;
                    double cpx3_0 = cpara_3 * cx0;

                    double cp6_1 = cpara_6 * cx1;
                    double cpx0_1 = cpara_0 * cx1;
                    double cpx3_1 = cpara_3 * cx1;

                    d = cp6_0 + cpy0_7;
                    ref_x[pt] = xx = (int)((cpx0_0 + cpy0_12) / d);
                    ref_y[pt] = yy = (int)((cpx3_0 + cpy0_45) / d);
                    if (xx < 0 || xx >= raster_width || yy < 0 || yy >= raster_height)
                    {
                        ref_x[pt] = xx < 0 ? 0 : (xx >= raster_width ? raster_width - 1 : xx);
                        ref_y[pt] = yy < 0 ? 0 : (yy >= raster_height ? raster_height - 1 : yy);
                    }
                    pt++;

                    d = cp6_0 + cpy1_7;
                    ref_x[pt] = xx = (int)((cpx0_0 + cpy1_12) / d);
                    ref_y[pt] = yy = (int)((cpx3_0 + cpy1_45) / d);
                    if (xx < 0 || xx >= raster_width || yy < 0 || yy >= raster_height)
                    {
                        ref_x[pt] = xx < 0 ? 0 : (xx >= raster_width ? raster_width - 1 : xx);
                        ref_y[pt] = yy < 0 ? 0 : (yy >= raster_height ? raster_height - 1 : yy);
                    }
                    pt++;

                    d = cp6_1 + cpy0_7;
                    ref_x[pt] = xx = (int)((cpx0_1 + cpy0_12) / d);
                    ref_y[pt] = yy = (int)((cpx3_1 + cpy0_45) / d);
                    if (xx < 0 || xx >= raster_width || yy < 0 || yy >= raster_height)
                    {
                        ref_x[pt] = xx < 0 ? 0 : (xx >= raster_width ? raster_width - 1 : xx);
                        ref_y[pt] = yy < 0 ? 0 : (yy >= raster_height ? raster_height - 1 : yy);
                    }
                    pt++;

                    d = cp6_1 + cpy1_7;
                    ref_x[pt] = xx = (int)((cpx0_1 + cpy1_12) / d);
                    ref_y[pt] = yy = (int)((cpx3_1 + cpy1_45) / d);
                    if (xx < 0 || xx >= raster_width || yy < 0 || yy >= raster_height)
                    {
                        ref_x[pt] = xx < 0 ? 0 : (xx >= raster_width ? raster_width - 1 : xx);
                        ref_y[pt] = yy < 0 ? 0 : (yy >= raster_height ? raster_height - 1 : yy);
                    }
                    pt++;
                }
                //1行分のピクセルを取得(場合によっては専用アクセサを書いた方がいい)
                i_raster.getPixelSet(ref_x, ref_y, resolution * 4, pixcel_temp, 0);
                //グレースケールにしながら、line→mapへの転写
                for (int i2 = 0; i2 < resolution; i2++)
                {
                    int index = i2 * 4;
                    o_bitbuffer.setBit(p, (pixcel_temp[index + 0] + pixcel_temp[index + 1] + pixcel_temp[index + 2] + pixcel_temp[index + 3]) / 4);
                    p++;
                }
            }
            return true;
        }


    }


    /**
     * ARPlayCard patt decoder
     */
    class MarkerPattDecoder
    {
        /**
         * ビット位置のテーブル(0の位置が1-4象限で反時計回り)
         */
        private static int[][] _bit_index ={
		   new int[]{	6,3,0,
			    7,4,1,
			    8,5,2},
		    new int[]{	0,1,2,
			    3,4,5,
			    6,7,8},
		    new int[]{	2,5,8,
			    1,4,7,
			    0,3,6},
		    new int[]{	8,7,6,
			    5,4,3,
			    2,1,0}
	    };
        /**
         * マーカパターン
         */
        private static int[][] _mk_patt ={
		    new int[]{	0,0,1,
			    1,0,1,
			    1,1,0},
		    new int[]{	0,0,0,
			    1,1,1,
			    0,0,1},
		    new int[]{	0,0,0,
			    1,0,1,
			    0,1,0},
		    new int[]{	1,0,1,
			    1,1,1,
			    1,1,1},
		    new int[]{	1,0,0,
			    1,1,1,
			    1,0,1},
		    new int[]{	0,0,1,
			    1,0,0,
			    1,0,1}
	    };
        private int[] _bits = new int[9];
        /**
         * この関数は、ビットイメージ{@link #_bits}のnビット目に、値をセットします。
         * @param i_bit_no
         * ビットイメージのインデクス
         * @param i_value
         * セットする値。
         */
        public void setBit(int i_bit_no, int i_value)
        {
            this._bits[i_bit_no] = i_value;
            return;
        }
        private static bool isMatchBits(int[] i_in_bits, int[] i_bit_map, int[] i_bit_index, int i_th)
        {
            for (int i = 8; i >= 0; i--)
            {

                if (((i_in_bits[i] > i_th) ? 1 : 0) != i_bit_map[i_bit_index[i]])
                {
                    return false;
                }
            }
            return true;
        }
        private static int getThreshold(int[] i_in_bits)
        {
            int min = i_in_bits[0];
            int max = i_in_bits[0];
            for (int i = i_in_bits.Length - 1; i > 0; i--)
            {
                if (min > i_in_bits[i])
                {
                    min = i_in_bits[i];
                }
                else if (max < i_in_bits[i])
                {
                    max = i_in_bits[i];
                }
            }
            return (max + min) / 2;
        }
        public bool decodePatt(PsARPlayCardPickup.PsArIdParam i_result)
        {
            int th = getThreshold(this._bits);
            for (int i = _mk_patt.Length - 1; i >= 0; i--)
            {
                for (int i2 = _bit_index.Length - 1; i2 >= 0; i2--)
                {
                    if (isMatchBits(this._bits, _mk_patt[i], _bit_index[i2], th))
                    {
                        i_result.direction = i2;
                        i_result.id = i + 1;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
