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
     * このクラスは、ARToolKitと同じ手順で主成分分析を行います。
     */
    public class NyARPca2d_MatrixPCA : INyARPca2d
    {
        private readonly NyARMatPca __pca_input = new NyARMatPca(1, 2);
        private readonly NyARMat __pca_evec = new NyARMat(2, 2);
        private readonly NyARVec __pca_ev = new NyARVec(2);
        private readonly NyARVec __pca_mean = new NyARVec(2);
        //override
        public void pca(double[] i_v1, double[] i_v2, int i_number_of_point, NyARDoubleMatrix22 o_evec, double[] o_ev, double[] o_mean)
        {
            NyARMatPca input = this.__pca_input;// 次処理で初期化される。		
            // pcaの準備
            input.realloc(i_number_of_point, 2);
            double[][] input_array = input.getArray();
            for (int i = 0; i < i_number_of_point; i++)
            {
                input_array[i][0] = i_v1[i];
                input_array[i][1] = i_v2[i];
            }
            // 主成分分析
            input.pca(this.__pca_evec, this.__pca_ev, this.__pca_mean);
            double[] mean_array = this.__pca_mean.getArray();
            double[][] evec_array = this.__pca_evec.getArray();
            double[] ev_array = this.__pca_ev.getArray();
            o_evec.m00 = evec_array[0][0];
            o_evec.m01 = evec_array[0][1];
            o_evec.m10 = evec_array[1][0];
            o_evec.m11 = evec_array[1][1];
            o_ev[0] = ev_array[0];
            o_ev[1] = ev_array[1];
            o_mean[0] = mean_array[0];
            o_mean[1] = mean_array[1];
            return;
        }
    }
}
