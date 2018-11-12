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
     * このクラスは、{@link NyARContourPickup}に、{@link NyARLabelingImage}の処理機能を追加したクラスです。
     * <p>@bug
     * この輪郭線抽出は、特定のパターンで1ドット幅の輪郭を正しく抽出できません。ARToolKit互換の画像処理では
     * 問題になることは少ないので、大きな影響はありませんが、必要に応じて{@link NyARContourPickup}を参考に直してください。
     * </p>
     */
    public class NyARContourPickup_ARToolKit : NyARContourPickup
    {
        /** 8方位探索の座標マップ*/
        protected readonly static int[] _getContour_xdir = { 0, 1, 1, 1, 0, -1, -1, -1, 0, 1, 1, 1, 0, -1, -1 };
        /** 8方位探索の座標マップ*/
        protected readonly static int[] _getContour_ydir = { -1, -1, 0, 1, 1, 1, 0, -1, -1, -1, 0, 1, 1, 1, 0 };

        public NyARContourPickup_ARToolKit():base()
        {
        }
        /**
         * この関数は、ラスタの指定点を基点に、輪郭線を抽出します。
         * 開始点は、輪郭の一部である必要があります。
         * 通常は、ラべリングの結果の上辺クリップとX軸エントリポイントを開始点として入力します。
         * @param i_raster
         * 輪郭線を抽出するラスタを指定します。
         * @param i_entry_x
         * 輪郭抽出の開始点です。
         * @param i_entry_y
         * 輪郭抽出の開始点です。
         * @param o_coord
         * 輪郭点を格納するオブジェクトを指定します。
         * @return
         * 輪郭線がo_coordの長さを超えた場合、falseを返します。
         * @
         */
        public bool getContour(NyARLabelingImage i_raster, int i_entry_x, int i_entry_y, NyARIntCoordinates o_coord)
        {
            int[] xdir = _getContour_xdir;// static int xdir[8] = { 0, 1, 1, 1, 0,-1,-1,-1};
            int[] ydir = _getContour_ydir;// static int ydir[8] = {-1,-1, 0, 1, 1, 1, 0,-1};

            int[] i_buf = (int[])i_raster.getBuffer();
            int width = i_raster.getWidth();
            int height = i_raster.getHeight();
            NyARIntPoint2d[] coord = o_coord.items;
            int i_array_size = o_coord.items.Length;
            //クリップ領域の上端に接しているポイントを得る。
            int sx = i_entry_x;
            int sy = i_entry_y;

            int coord_num = 1;
            coord[0].x = sx;
            coord[0].y = sy;
            int dir = 5;

            int c = coord[0].x;
            int r = coord[0].y;
            for (; ; )
            {
                dir = (dir + 5) % 8;//dirの正規化
                //ここは頑張ればもっと最適化できると思うよ。
                //4隅以外の境界接地の場合に、境界チェックを省略するとかね。
                if (c >= 1 && c < width - 1 && r >= 1 && r < height - 1)
                {
                    for (; ; )
                    {//gotoのエミュレート用のfor文
                        //境界に接していないとき
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        //8方向全て調べたけどラベルが無いよ？
                        throw new NyARRuntimeException();
                    }
                }
                else
                {
                    //境界に接しているとき
                    int i;
                    for (i = 0; i < 8; i++)
                    {
                        int x = c + xdir[dir];
                        int y = r + ydir[dir];
                        //境界チェック
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            if (i_buf[(y) * width + (x)] > 0)
                            {
                                break;
                            }
                        }
                        dir++;//倍長テーブルを参照するので問題なし
                    }
                    if (i == 8)
                    {
                        //8方向全て調べたけどラベルが無いよ？
                        throw new NyARRuntimeException();// return(-1);
                    }
                }

                dir = dir % 8;//dirの正規化

                // xcoordとycoordをc,rにも保存
                c = c + xdir[dir];
                r = r + ydir[dir];
                coord[coord_num].x = c;
                coord[coord_num].y = r;
                // 終了条件判定
                if (c == sx && r == sy)
                {
                    coord_num++;
                    break;
                }
                coord_num++;
                if (coord_num == i_array_size)
                {
                    //輪郭が末端に達した
                    return false;
                }
            }
            o_coord.length = coord_num;
            return true;
        }
    }
}
