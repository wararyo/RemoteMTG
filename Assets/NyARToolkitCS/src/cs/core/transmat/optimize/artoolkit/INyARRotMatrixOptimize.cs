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
     * このクラスは、ARToolKit由来の回転行列最適化関数を定義します。
     *
     */
    public interface INyARRotMatrixOptimize
    {
        /**
         * この関数は、回転行列を最適化します。
         * 実装クラスでは、ARToolKitと互換性のある値を返す、回転行列の最適化処理を書きます。
         * @param io_rot
         * 最適化する回転行列を指定します。
         * @param i_trans
         * 平行移動量
         * @param i_vertex3d
         * 三次元オフセット座標
         * @param i_vertex2d
         * 理想座標系の頂点座標
         * @return
         * エラーレート
         * @
         */
        double modifyMatrix(NyARRotMatrix_ARToolKit io_rot, NyARDoublePoint3d i_trans, NyARDoublePoint3d[] i_vertex3d, NyARDoublePoint2d[] i_vertex2d);

    }
}
