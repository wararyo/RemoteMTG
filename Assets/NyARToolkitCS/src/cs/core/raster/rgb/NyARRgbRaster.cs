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
     * このクラスは、指定形式のバッファを持つRGBラスタです。
     * 外部参照バッファ、内部バッファの両方に対応します。
     * コンストラクタは無効です。{@link #createInstance}を使ってください。
     * <p>
     * 対応しているバッファタイプ-
     * <ul>{@link NyARBufferType#INT1D_X8R8G8B8_32}
     * <li>{@link NyARBufferType#BYTE1D_B8G8R8X8_32}
     * <li>{@link NyARBufferType#BYTE1D_R8G8B8_24}
     * <li>{@link NyARBufferType#BYTE1D_B8G8R8_24}
     * <li>{@link NyARBufferType#BYTE1D_X8R8G8B8_32}
     * <li>{@link NyARBufferType#WORD1D_R5G6B5_16LE}
     * </ul>
     * </p>
     */
    public abstract class NyARRgbRaster : INyARRgbRaster
    {
        /** バッファオブジェクトがアタッチされていればtrue*/
        readonly protected bool _is_attached_buffer;
        readonly protected NyARIntSize _size;
        /**
         * コンストラクタです。
         * 画像のサイズパラメータとバッファ形式を指定して、インスタンスを生成します。
         * @param i_width
         * ラスタのサイズ
         * @param i_height
         * ラスタのサイズ
         * @param i_raster_type
         * ラスタのバッファ形式。
         * {@link NyARBufferType}に定義された定数値を指定してください。
         * 指定できる値は、クラスの説明を見てください。
         * @param i_is_alloc
         * バッファを外部参照にするかのフラグ値。
         * trueなら内部バッファ、falseなら外部バッファを使用します。
         * falseの場合、初期のバッファはnullになります。インスタンスを生成したのちに、{@link #wrapBuffer}を使って割り当ててください。
         * @throws NyARRuntimeException
         */
        public static INyARRgbRaster createInstance(int i_width, int i_height, int i_buffer_type, bool i_is_alloc)
        {
            switch (i_buffer_type)
            {
                case NyARBufferType.INT1D_X8R8G8B8_32:
                    return new NyARRgbRaster_INT1D_X8R8G8B8_32(i_width, i_height, i_is_alloc);
                case NyARBufferType.BYTE1D_B8G8R8X8_32:
                    return new NyARRgbRaster_BYTE1D_B8G8R8X8_32(i_width, i_height, i_is_alloc);
                case NyARBufferType.BYTE1D_X8R8G8B8_32:
                    return new NyARRgbRaster_BYTE1D_X8R8G8B8_32(i_width, i_height, i_is_alloc);
                case NyARBufferType.BYTE1D_X8B8G8R8_32:
                    return new NyARRgbRaster_BYTE1D_X8B8G8R8_32(i_width, i_height, i_is_alloc);
                case NyARBufferType.BYTE1D_R8G8B8_24:
                    return new NyARRgbRaster_BYTE1D_R8G8B8_24(i_width, i_height, i_is_alloc);
                case NyARBufferType.BYTE1D_B8G8R8_24:
                    return new NyARRgbRaster_BYTE1D_B8G8R8_24(i_width, i_height, i_is_alloc);
                case NyARBufferType.WORD1D_R5G6B5_16LE:
                    return new NyARRgbRaster_WORD1D_R5G6B5_16LE(i_width, i_height, i_is_alloc);
                default:
                    throw new NyARRuntimeException();
            }
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
         * {@link NyARBufferType}に定義された定数値を指定してください。
         * 指定できる値は、クラスの説明を見てください。
         * @throws NyARRuntimeException
         */
        public static INyARRgbRaster createInstance(int i_width, int i_height, int i_raster_type)
        {
            return createInstance(i_width, i_height, i_raster_type, true);
        }
        /**
         * コンストラクタです。
         * 画像サイズを指定してインスタンスを生成します。
         * @param i_width
         * ラスタのサイズ
         * @param i_height
         * ラスタのサイズ
         * @throws NyARRuntimeException
         */
        public static INyARRgbRaster createInstance(int i_width, int i_height)
        {
            return createInstance(i_width, i_height, NyARBufferType.INT1D_X8R8G8B8_32);
        }

        /**
         * 継承クラス呼び出すコンストラクタです。
         */
        protected NyARRgbRaster(int i_width, int i_height, bool i_is_alloc)
        {
            this._is_attached_buffer = i_is_alloc;
            this._size = new NyARIntSize(i_width, i_height);
        }


        /**
         * インスタンスがバッファを所有するかを返します。
         * コンストラクタでi_is_allocをfalseにしてラスタを作成した場合、
         * バッファにアクセスするまえに、バッファの有無をこの関数でチェックしてください。
         * @return
         * インスタンスがバッファを所有すれば、trueです。
         */
        public bool hasBuffer()
        {
            return this.getBuffer() != null;
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
         * この関数は、ラスタの幅を返します。
         */
        public bool isEqualBufferType(int i_type_value)
        {
            return this.getBufferType() == i_type_value;
        }
        /**
         * サポートしているインタフェイスは以下の通りです。
         * <ul>
         * <li>{@link INyARRgbPixelDriver}
         * <li>{@link INyARPerspectiveCopy}
         * <li>{@link INyARPerspectiveCopy}
         * <li>{@link NyARMatchPattDeviationColorData.IRasterDriver}
         * <li>{@link INyARRgb2GsFilter}
         * <li>{@link INyARRgb2GsFilterRgbAve}
         * <li>{@link INyARRgb2GsFilterRgbCube}
         * <li>{@link INyARRgb2GsFilterYCbCr}
         * <li>{@link INyARRgb2GsFilterArtkTh}
         * </ul>
         */
        virtual public Object createInterface(Type iIid)
        {
            if (iIid == typeof(INyARPerspectiveCopy))
            {
                return NyARPerspectiveCopyFactory.createDriver(this);
            }
            if (iIid == typeof(NyARMatchPattDeviationColorData.IRasterDriver))
            {
                return NyARMatchPattDeviationColorData.RasterDriverFactory.createDriver(this);
            }
            //継承を考慮してる。
            if (iIid == typeof(INyARRgb2GsFilter))
            {
                return NyARRgb2GsFilterFactory.createRgbAveDriver(this);
            }
            else if (iIid == typeof(INyARRgb2GsFilterRgbAve))
            {
                return NyARRgb2GsFilterFactory.createRgbAveDriver(this);
            }
            else if (iIid == typeof(INyARRgb2GsFilterRgbCube))
            {
                return NyARRgb2GsFilterFactory.createRgbCubeDriver(this);
            }
            else if (iIid == typeof(INyARRgb2GsFilterYCbCr))
            {
                return NyARRgb2GsFilterFactory.createYCbCrDriver(this);
            }
            if (iIid == typeof(INyARRgb2GsFilterArtkTh))
            {
                return NyARRgb2GsFilterArtkThFactory.createDriver(this);
            }
            //クラスが見つからない
            throw new NyARRuntimeException("Interface not found!");
            //		return null;
        }
        public static void main(String[] args)
        {
            //INyARRgbRaster n = NyARRgbRaster.createInstance(640, 480);
            //long s = System.currentTimeMillis();
            //for (int i = 0; i < 100000; i++)
            //{
            //    n.createInterface(null);
            //}
            ////		System.out.println(System.currentTimeMillis()-s);
        }
        public abstract Object getBuffer();
        public abstract int getBufferType();
        public abstract void wrapBuffer(Object i_ref_buf);
        public abstract int[] getPixel(int i_x, int i_y, int[] i_rgb);
        public abstract int[] getPixelSet(int[] i_x, int[] i_y, int i_num, int[] i_intrgb);
        public abstract void setPixel(int i_x, int i_y, int i_r, int i_g, int i_b);
        public abstract void setPixel(int i_x, int i_y, int[] i_rgb);
        public abstract void setPixels(int[] i_x, int[] i_y, int i_num, int[] i_intrgb);
    }
}
