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
     * このクラスは、{@link NyARMatchPatt_BlackWhite}や{@link NyARMatchPatt_Color_WITHOUT_PCA}のevaluate
     * 関数の戻り値を格納します。
     */
    public class NyARMatchPattResult
    {
        /** {@link #direction}の初期値。方位不明である事を表します。*/
        public const int DIRECTION_UNKNOWN = -1;
        /** パターンの一致率。0から1.0までの数値です。高い方が、一致率が高いことを示します。*/
        public double confidence;
        /** ARToolKit準拠の方位定数です。
         *  画像の右上位置が、0=1象限、1=2象限、、2=3象限、、3=4象限の位置にあることを示します。
         */
        public int direction;
    }
}
