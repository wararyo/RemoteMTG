﻿/* 
 * PROJECT: NyARToolkit
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *  Copyright 2015 Daqri, LLC.
 *  Copyright 2006-2015 ARToolworks, Inc.
 *
 *  Author(s): Hirokazu Kato, Philip Lamb
 *
 * The NyARToolkit is Java edition ARToolKit class library.
 * Copyright (C)2008-2016 Ryo Iizuka
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
using jp.nyatla.nyartoolkit.cs.cs4;
namespace jp.nyatla.nyartoolkit.cs.core
{
/**
 * ARToolKitV4のisetファイルを読み出します。
 * <pre>
 * //ファイル形式
 * int imageset_num;
 * AR2ImageT{
 * 		int           xsize;
 *		int           ysize;
 *		float         dpi;
 * 		byte          ARUint8[xsize*ysize];
 * }[imageset_num];
 * </pre>
 */
    public class IsetFileDataParserV4
    {
        public class AR2ImageT
        {
            readonly public double dpi;
            readonly public int width;
            readonly public int height;
            readonly public byte[] img;
            public AR2ImageT(int i_w, int i_h, double i_dpi, byte[] i_img)
            {
                this.width = i_w;
                this.height = i_h;
                this.dpi = i_dpi;
                this.img = i_img;
            }
        }
        readonly public AR2ImageT[] ar2image;
        public IsetFileDataParserV4(byte[] i_src)
        {
            BinaryReader br = new BinaryReader(i_src, BinaryReader.ENDIAN_LITTLE);
            int n = br.getInt();
            this.ar2image = new AR2ImageT[n];
            for (int i = 0; i < n; i++)
            {
                int w = br.getInt();
                int h = br.getInt();
                double dpi = br.getDouble();
                byte[] d = br.getByteArray(w * h);
                this.ar2image[i] = new AR2ImageT(w, h, dpi, d);
            }
        }
    }
}
