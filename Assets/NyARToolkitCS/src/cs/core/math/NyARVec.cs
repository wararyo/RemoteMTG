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
     * このクラスは、ARToolKit由来のベクトル値計算関数を提供します。
     * <p>memo:
     * このクラスは、今後統合・削除する可能性があります。
     * </p>
     */
    public class NyARVec
    {
        /** ベクトルの列数*/
        private int clm;
        /** ベクトル値を格納する配列*/
        private double[] v;


        /**
         * コンストラクタです。
         * 列数を指定して、インスタンスを生成します。
         * @param i_clm
         * 列数です。
         */
        public NyARVec(int i_clm)
        {
            v = new double[i_clm];
            clm = i_clm;
        }

        /**
         * ベクトルのバッファサイズを、i_clmに十分になるように変更します。
         * 実行後、列の各値は不定になります。
         * @param i_clm
         * 新しい列数
         */
        public void realloc(int i_clm)
        {
            if (i_clm <= this.v.Length)
            {
                // 十分な配列があれば何もしない。
            }
            else
            {
                // 不十分なら取り直す。
                v = new double[i_clm];
            }
            this.clm = i_clm;
        }

        /**
         * ベクトルの列数を返します。
         * @return
         * ベクトルの列数
         */
        public int getClm()
        {
            return clm;
        }

        /**
         * ベクトル値を格納した配列の参照値を返します。
         * @return
         * 配列の参照値
         */
        public double[] getArray()
        {
            return v;
        }

        /**
         * arVecInnerproduct関数の同等品です。
         * この関数は動作チェックをしておらず、機能しません。
         * 詳細は不明です。
         * @param y
         * 不明。
         * @param i_start
         * 演算開始列(よくわからないけどarVecTridiagonalizeの呼び出し元でなんかしてる)
         * @return
         * 不明。
         * @
         */
        public double vecInnerproduct(NyARVec y, int i_start)
        {
            NyARRuntimeException.trap("この関数は動作確認できていません。");
            double result = 0.0;
            // double[] x_array=x.v;.getArray();
            // double[] y_array=y.getArray();

            if (this.clm != y.clm)
            {
                throw new NyARRuntimeException();// exit();
            }
            for (int i = i_start; i < this.clm; i++)
            {
                NyARRuntimeException.trap("未チェックのパス");
                result += this.v[i] * y.v[i];// result += x->v[i] * y->v[i];
            }
            return result;
        }

        /**
         * arVecHousehold関数の同等品です。
         * 詳細は不明です。
         * @param i_start
         * 演算開始列(よくわからないけどarVecTridiagonalizeの呼び出し元でなんかしてる)
         * @return
         * 不明。
         * @
         */
        public double vecHousehold(int i_start)
        {
            NyARRuntimeException.trap("この関数は動作確認できていません。");
            double s, t;
            s = Math.Sqrt(this.vecInnerproduct(this, i_start));
            // double[] x_array=x.getArray();
            if (s != 0.0)
            {
                NyARRuntimeException.trap("未チェックのパス");
                if (this.v[i_start] < 0)
                {
                    s = -s;
                }
                NyARRuntimeException.trap("未チェックのパス");
                {
                    this.v[i_start] += s;// x->v[0] += s;
                    t = 1 / Math.Sqrt(this.v[i_start] * s);// t = 1 / sqrt(x->v[0] * s);
                }
                for (int i = i_start; i < this.clm; i++)
                {
                    NyARRuntimeException.trap("未チェックのパス");
                    this.v[i] *= t;// x->v[i] *= t;
                }
            }
            return -s;
        }

        /**
         * 現在ラップしている配列を取り外して、新しい配列と、列数をセットします。
         * @param i_array
         * 新しく設定する配列です。この配列は、thisが所有します。
         * @param i_clm
         * 新しいVectorの列数です。
         */
        public void setNewArray(double[] i_array, int i_clm)
        {
            this.v = i_array;
            this.clm = i_clm;
        }
    }
}
