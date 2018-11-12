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
     * このクラスは、NyARToolkitの計算式で、二次元矩形から３次元位置姿勢を推定します。
     */
    public class NyARTransMat : INyARTransMat
    {
        private NyARPerspectiveProjectionMatrix _ref_projection_mat;
        protected NyARRotMatrix _rotmatrix;
        protected NyARTransportVectorSolver _transsolver;
        protected NyARPartialDifferentiationOptimize _mat_optimize;


        private INyARCameraDistortionFactor _ref_dist_factor;


        /**
         * コンストラクタです。
         * 座標計算に必要なオブジェクトの参照値を元に、インスタンスを生成します。
         * @param i_ref_distfactor
         * 樽型歪み矯正オブジェクトの参照値です。歪み矯正が不要な時は、nullを指定します。
         * @param i_ref_projmat
         * 射影変換オブジェクトの参照値です。
         * @
         */
        public NyARTransMat(INyARCameraDistortionFactor i_distfactor, NyARPerspectiveProjectionMatrix i_projmat)
        {
            this._transsolver = new NyARTransportVectorSolver(i_projmat, 4);
            //互換性が重要な時は、NyARRotMatrix_ARToolKitを使うこと。
            //理屈はNyARRotMatrix_NyARToolKitもNyARRotMatrix_ARToolKitも同じだけど、少しだけ値がずれる。
            this._rotmatrix = new NyARRotMatrix(i_projmat);
            this._mat_optimize = new NyARPartialDifferentiationOptimize(i_projmat);
            this._ref_dist_factor = i_distfactor;
            this._ref_projection_mat = i_projmat;
            return;
        }

        /**
         * コンストラクタです。
         * 座標計算に必要なカメラパラメータの参照値を元に、インスタンスを生成します。
         * @param i_param
         * ARToolKit形式のカメラパラメータです。
         * インスタンスは、この中から樽型歪み矯正オブジェクト、射影変換オブジェクトを参照します。
         * @
         */
        public NyARTransMat(NyARParam i_param)
            : this(i_param.getDistortionFactor(), i_param.getPerspectiveProjectionMatrix())
        {
            return;
        }

        private readonly NyARDoublePoint2d[] __transMat_vertex_2d = NyARDoublePoint2d.createArray(4);
        private readonly NyARDoublePoint3d[] __transMat_vertex_3d = NyARDoublePoint3d.createArray(4);
        private readonly NyARDoublePoint3d __transMat_trans = new NyARDoublePoint3d();

        /**
         * 頂点情報を元に、エラー閾値を計算します。
         * @param i_vertex
         */
        private double makeErrThreshold(NyARDoublePoint2d[] i_vertex)
        {
            double a, b, l1, l2;
            a = i_vertex[0].x - i_vertex[2].x;
            b = i_vertex[0].y - i_vertex[2].y;
            l1 = a * a + b * b;
            a = i_vertex[1].x - i_vertex[3].x;
            b = i_vertex[1].y - i_vertex[3].y;
            l2 = a * a + b * b;
            return (Math.Sqrt(l1 > l2 ? l1 : l2)) / 200;
        }

        /**
         * この関数は、理想座標系の四角系を元に、位置姿勢変換行列を求めます。
         * ARToolKitのarGetTransMatに該当します。
         * @see INyARTransMat#transMatContinue
         */
	    public bool transMat(NyARSquare i_square,NyARRectOffset i_offset, NyARDoubleMatrix44 o_result,NyARTransMatResultParam o_param)
        {
            NyARDoublePoint3d trans = this.__transMat_trans;
            double err_threshold = makeErrThreshold(i_square.sqvertex);

            NyARDoublePoint2d[] vertex_2d;
            if (this._ref_dist_factor != null)
            {
                //歪み復元必要
                vertex_2d = this.__transMat_vertex_2d;
                this._ref_dist_factor.ideal2ObservBatch(i_square.sqvertex, vertex_2d, 4);
            }
            else
            {
                //歪み復元は不要
                vertex_2d = i_square.sqvertex;
            }
            //平行移動量計算機に、2D座標系をセット
            this._transsolver.set2dVertex(vertex_2d, 4);

            //回転行列を計算
            if (!this._rotmatrix.initRotBySquare(i_square.line, i_square.sqvertex))
            {
                return false;
            }

            //回転後の3D座標系から、平行移動量を計算
            NyARDoublePoint3d[] vertex_3d = this.__transMat_vertex_3d;
            this._rotmatrix.getPoint3dBatch(i_offset.vertex, vertex_3d, 4);
            this._transsolver.solveTransportVector(vertex_3d, trans);

            //計算結果の最適化(平行移動量と回転行列の最適化)
            double err = this.optimize(this._rotmatrix, trans, this._transsolver, i_offset.vertex, vertex_2d, err_threshold, o_result);
            //必要なら計算パラメータを返却
            if (o_param != null)
            {
                o_param.last_error = err;
            } 
            return true;
        }

        /**
         * この関数は、理想座標系の四角系を元に、位置姿勢変換行列を求めます。
         * 計算に過去の履歴を使う点が、{@link #transMat}と異なります。
         * @see INyARTransMat#transMatContinue
         */
 	    public bool transMatContinue(NyARSquare i_square,NyARRectOffset i_offset, NyARDoubleMatrix44 i_prev_result,double i_prev_err,NyARDoubleMatrix44 o_result,NyARTransMatResultParam o_param)
        {
            NyARDoublePoint3d trans = this.__transMat_trans;

            //最適化計算の閾値を決定
            double err_threshold = makeErrThreshold(i_square.sqvertex);


            //平行移動量計算機に、2D座標系をセット
            NyARDoublePoint2d[] vertex_2d;
            if (this._ref_dist_factor != null)
            {
                vertex_2d = this.__transMat_vertex_2d;
                this._ref_dist_factor.ideal2ObservBatch(i_square.sqvertex, vertex_2d, 4);
            }
            else
            {
                vertex_2d = i_square.sqvertex;
            }
            this._transsolver.set2dVertex(vertex_2d, 4);

            //回転行列を計算
            NyARRotMatrix rot = this._rotmatrix;
            rot.initRotByPrevResult(i_prev_result);

            //回転後の3D座標系から、平行移動量を計算
            NyARDoublePoint3d[] vertex_3d = this.__transMat_vertex_3d;
            rot.getPoint3dBatch(i_offset.vertex, vertex_3d, 4);
            this._transsolver.solveTransportVector(vertex_3d, trans);

            //現在のエラーレートを計算
            double min_err = errRate(rot, trans, i_offset.vertex, vertex_2d, 4, vertex_3d);
            
            //エラーレートの判定
		    if(min_err<i_prev_err+err_threshold){
			    //save initial result
			    o_result.setValue(rot,trans);
                //			System.out.println("TR:ok");
                //最適化してみる。
                for (int i = 0; i < 5; i++)
                {
                    //変換行列の最適化
                    this._mat_optimize.modifyMatrix(rot, trans, i_offset.vertex, vertex_2d, 4);
                    double err = errRate(rot, trans, i_offset.vertex, vertex_2d, 4, vertex_3d);
                    //System.out.println("E:"+err);
                    if (min_err - err < err_threshold / 2)
                    {
                        //System.out.println("BREAK");
                        break;
                    }
                    this._transsolver.solveTransportVector(vertex_3d, trans);
                    o_result.setValue(rot, trans);
                    min_err = err;
                }
                //継続計算成功
                if (o_param != null)
                {
                    o_param.last_error = min_err;
                }
                return true;
            }
            //継続計算失敗
            return false;
        }

        /**
         * 
         * @param iw_rotmat
         * @param iw_transvec
         * @param i_solver
         * @param i_offset_3d
         * @param i_2d_vertex
         * @param i_err_threshold
         * @param o_result
         * @return
         * @
         */
	    private double optimize(NyARRotMatrix iw_rotmat,NyARDoublePoint3d iw_transvec,INyARTransportVectorSolver i_solver,NyARDoublePoint3d[] i_offset_3d,NyARDoublePoint2d[] i_2d_vertex,double i_err_threshold,NyARDoubleMatrix44 o_result)
        {
            //System.out.println("START");
            NyARDoublePoint3d[] vertex_3d = this.__transMat_vertex_3d;
            //初期のエラー値を計算
            double min_err = errRate(iw_rotmat, iw_transvec, i_offset_3d, i_2d_vertex, 4, vertex_3d);
            o_result.setValue(iw_rotmat, iw_transvec);

            for (int i = 0; i < 5; i++)
            {
                //変換行列の最適化
                this._mat_optimize.modifyMatrix(iw_rotmat, iw_transvec, i_offset_3d, i_2d_vertex, 4);
                double err = errRate(iw_rotmat, iw_transvec, i_offset_3d, i_2d_vertex, 4, vertex_3d);
                //System.out.println("E:"+err);
                if (min_err - err < i_err_threshold)
                {
                    //System.out.println("BREAK");
                    break;
                }
                i_solver.solveTransportVector(vertex_3d, iw_transvec);
                o_result.setValue(iw_rotmat, iw_transvec);
                min_err = err;
            }
            //System.out.println("END");
            return min_err;
        }

        /**
         * この関数は、姿勢行列のエラーレートを計算します。
         * エラーレートは、回転行列、平行移動量、オフセット、観察座標から計算します。
         * 通常、ユーザが使うことはありません。
         * @param i_rot
         * 回転行列
         * @param i_trans
         * 平行移動量
         * @param i_vertex3d
         * オフセット位置
         * @param i_vertex2d
         * 理想座標
         * @param i_number_of_vertex
         * 評価する頂点数
         * @param o_rot_vertex
         * 計算過程で得られた、各頂点の三次元座標
         * @return
         * エラーレート(Σ(理想座標と計算座標の距離[n]^2))
         * @
         */
        public double errRate(NyARDoubleMatrix33 i_rot, NyARDoublePoint3d i_trans, NyARDoublePoint3d[] i_vertex3d, NyARDoublePoint2d[] i_vertex2d, int i_number_of_vertex, NyARDoublePoint3d[] o_rot_vertex)
        {
            NyARPerspectiveProjectionMatrix cp = this._ref_projection_mat;
            double cp00 = cp.m00;
            double cp01 = cp.m01;
            double cp02 = cp.m02;
            double cp11 = cp.m11;
            double cp12 = cp.m12;

            double err = 0;
            for (int i = 0; i < i_number_of_vertex; i++)
            {
                double x3d, y3d, z3d;
                o_rot_vertex[i].x = x3d = i_rot.m00 * i_vertex3d[i].x + i_rot.m01 * i_vertex3d[i].y + i_rot.m02 * i_vertex3d[i].z;
                o_rot_vertex[i].y = y3d = i_rot.m10 * i_vertex3d[i].x + i_rot.m11 * i_vertex3d[i].y + i_rot.m12 * i_vertex3d[i].z;
                o_rot_vertex[i].z = z3d = i_rot.m20 * i_vertex3d[i].x + i_rot.m21 * i_vertex3d[i].y + i_rot.m22 * i_vertex3d[i].z;
                x3d += i_trans.x;
                y3d += i_trans.y;
                z3d += i_trans.z;

                //射影変換
                double x2d = x3d * cp00 + y3d * cp01 + z3d * cp02;
                double y2d = y3d * cp11 + z3d * cp12;
                double h2d = z3d;

                //エラーレート計算
                double t1 = i_vertex2d[i].x - x2d / h2d;
                double t2 = i_vertex2d[i].y - y2d / h2d;
                err += t1 * t1 + t2 * t2;

            }
            return err / i_number_of_vertex;
        }
    }
}
