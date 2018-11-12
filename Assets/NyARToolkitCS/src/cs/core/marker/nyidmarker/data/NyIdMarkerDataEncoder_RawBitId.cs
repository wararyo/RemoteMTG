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
namespace jp.nyatla.nyartoolkit.cs.nyidmarker
{

    /**
     * このクラスは、RawbitドメインのIdマーカをLongのシリアル値にするエンコーダです。
     * Rawbitマーカのパケットを、[0][1]...[n]の順に並べて、64bitのId値を作ります。
     * コントロールドメイン0、マスク0、Model5未満のマーカを対象にします。
     */
    public class NyIdMarkerDataEncoder_RawBitId : NyIdMarkerDataEncoder_RawBit
    {
        private NyIdMarkerData_RawBit _tmp = new NyIdMarkerData_RawBit();
        public override bool encode(NyIdMarkerPattern i_data, INyIdMarkerData o_dest)
        {
            //対象か調べるん
            if (i_data.ctrl_domain != 0)
            {
                return false;
            }
            //受け入れられるMaskは0のみ
            if (i_data.ctrl_mask != 0)
            {
                return false;
            }
            //受け入れられるModelは5未満
            if (i_data.model >= 5)
            {
                return false;
            }
            //エンコードしてみる
            if (!base.encode(i_data, this._tmp))
            {
                return false;
            }
            //SerialIDの再構成
            ulong s = 0;
            //最大4バイト繋げて１個のint値に変換
            for (int i = 0; i < this._tmp.length; i++)
            {
                s = (s << 8) | (uint)this._tmp.packet[i];
            }
            ((NyIdMarkerData_RawBitId)o_dest).marker_id = (long)s;
            return true;
        }
        /**
         * この関数は、{@link NyIdMarkerData_RawBitId}型のオブジェクトを生成して返します。
         */
        public override INyIdMarkerData createDataInstance()
        {
            return new NyIdMarkerData_RawBitId();
        }
    }
}
