﻿/* 
 * PROJECT: NyARToolkit(Extension)
 * --------------------------------------------------------------------------------
 *
 * The NyARToolkit is Java edition ARToolKit class library.
 * Copyright (C)2008-2012 Ryo Iizuka
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
     * このクラスは、グレースケース画像を格納するラスタクラスです。
     * 外部バッファ、内部バッファの両方に対応します。
     */
    public abstract class NyARGrayscaleRaster : INyARGrayscaleRaster
    {
        readonly protected NyARIntSize _size;
        /** バッファオブジェクトがアタッチされていればtrue*/
        protected readonly bool _is_attached_buffer;

        public static INyARGrayscaleRaster createInstance(int i_w, int i_h, int i_raster_type, Object i_src)
        {
            INyARGrayscaleRaster ret = createInstance(i_w, i_h, i_raster_type, false);
            ret.wrapBuffer(i_src);
            return ret;
        }


        /**
         * コンストラクタです。
         * 画像のサイズパラメータとバッファ形式を指定して、インスタンスを生成します。
         * @param i_width
         * ラスタのサイズ
         * @param i_height
         * ラスタのサイズ
         * @param i_raster_type
         * ラスタのバッファ形式。
         * {@link NyARBufferType}に定義された定数値を指定してください。指定できる値は、以下の通りです。
         * <ul>
         * <li>{@link NyARBufferType#INT1D_GRAY_8}
         * <ul>
         * @param i_is_alloc
         * バッファを外部参照にするかのフラグ値。
         * trueなら内部バッファ、falseなら外部バッファを使用します。
         * falseの場合、初期のバッファはnullになります。インスタンスを生成したのちに、{@link #wrapBuffer}を使って割り当ててください。
         * @throws NyARRuntimeException
         */
        public static INyARGrayscaleRaster createInstance(int i_width, int i_height, int i_raster_type, bool i_is_alloc)
        {
            switch (i_raster_type)
            {
                case NyARBufferType.INT1D_GRAY_8:
                    return new NyARGsRaster_INT1D_GRAY_8(i_width, i_height, i_is_alloc);
            }
            throw new NyARRuntimeException();
        }
        /**
         * 内部参照のバッファ（{@link NyARBufferType#INT1D_GRAY_8}形式）を持つインスタンスを生成します。
         * @param i_width
         * ラスタのサイズ
         * @param i_height
         * ラスタのサイズ
         * @throws NyARRuntimeException
         */
        public static INyARGrayscaleRaster createInstance(int i_width, int i_height)
        {
            return NyARGrayscaleRaster.createInstance(i_width, i_height, true);
        }

        public static INyARGrayscaleRaster createInstance(NyARIntSize i_size)
        {
            return NyARGrayscaleRaster.createInstance(i_size.w, i_size.h, true);
        }
        /**
         * 画像のサイズパラメータとバッファ参照方式を指定して、インスタンスを生成します。
         * バッファの形式は、{@link NyARBufferType#INT1D_GRAY_8}です。
         * @param i_width
         * ラスタのサイズ
         * @param i_height
         * ラスタのサイズ
         * @param i_is_alloc
         * バッファを外部参照にするかのフラグ値。
         * trueなら内部バッファ、falseなら外部バッファを使用します。
         * falseの場合、初期のバッファはnullになります。インスタンスを生成したのちに、{@link #wrapBuffer}を使って割り当ててください。
         * @throws NyARRuntimeException
         */
        public static INyARGrayscaleRaster createInstance(int i_width, int i_height, bool i_is_alloc)
        {
            return NyARGrayscaleRaster.createInstance(i_width, i_height, NyARBufferType.INT1D_GRAY_8, i_is_alloc);
        }
        /**
         * この関数は、ラスタの幅を返します。
         */
        public int getWidth()
        {
            return this._size.w;
        }
        /**
         * この関数は、ラスタの高さを返します。
         */
        public int getHeight()
        {
            return this._size.h;
        }
        /**
         * この関数は、ラスタのサイズを格納したオブジェクトを返します。
         */
        public NyARIntSize getSize()
        {
            return this._size;
        }
        /**
         * この関数は、ラスタのバッファへの参照値を返します。
         * バッファの形式は、コンストラクタに指定した形式と同じです。
         */

        /**
         * この関数は、ラスタの幅を返します。
         */
        public bool isEqualBufferType(int i_type_value)
        {
            return i_type_value == this.getBufferType();
        }
        /**
         * この関数は、インスタンスがバッファを所有するかを返します。
         * 内部参照バッファの場合は、常にtrueです。
         * 外部参照バッファの場合は、バッファにアクセスする前に、このパラメタを確認してください。
         */
        public bool hasBuffer()
        {
            return this.getBuffer() != null;
        }


        /**
         * 継承クラスから使うコンストラクタです。
         * 引数に有効な値を設定してください。
         * @param i_size
         * @param i_is_alloc
         */
        protected NyARGrayscaleRaster(int i_width, int i_height, bool i_is_alloc)
        {
            this._size = new NyARIntSize(i_width, i_height);
            this._is_attached_buffer = i_is_alloc;
        }
        public Object createInterface(Type i_iid)
        {
            if (i_iid == typeof(NyARLabeling_Rle.IRasterDriver))
            {
                return NyARLabeling_Rle.RasterDriverFactory.createDriver(this);
            }
            if (i_iid == typeof(NyARContourPickup.IRasterDriver))
            {
                return NyARContourPickup.ImageDriverFactory.createDriver(this);
            }
            if (i_iid == typeof(INyARHistogramFromRaster))
            {
                return NyARHistogramFromRasterFactory.createInstance(this);
            }
            throw new NyARRuntimeException();
        }
        abstract public Object getBuffer();
        abstract public int getBufferType();
        abstract public void wrapBuffer(Object i_ref_buf);
        abstract public int[] getPixelSet(int[] i_x, int[] i_y, int i_n, int[] o_buf, int i_st_buf);
        abstract public int getPixel(int i_x, int i_y);
        abstract public void setPixel(int i_x, int i_y, int i_gs);
        abstract public void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intgs);
    }
}
