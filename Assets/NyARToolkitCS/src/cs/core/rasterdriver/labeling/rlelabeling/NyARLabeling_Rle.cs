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
using System.Diagnostics;
namespace jp.nyatla.nyartoolkit.cs.core
{








    /**
     * このクラスは、RLE圧縮を利用した輪郭線エントリポイント検出用のラべリングクラスです。
     * ラべリング画像を生成せずに、ラベル輪郭へのエントリーポイントの一覧を作ることに注意してください。
     * <p>コールバック関数
     * このクラスはいくつかの自己コールバック関数を持つ抽象クラスです。継承クラスでコールバック関数を実装して使います。
     * <ul>
     * <li>{@link #onLabelFound}- {@link #labeling}関数が検出したラベルを通知するコールバック関数です。
     * ここに、発見したラベルを処理するコードを書きます。
     * </ul>
     * </p>
     * <p>ラベル輪郭のエントリーポイント -
     * このエントリポイントは、ラベルを構成する塊の輪郭を指す１点です。ここから８方位輪郭検出を実行すると、
     * ラベルの輪郭を一周することができます。
     * </p>
     * <p>入力できる画素形式 -
     * <p>{@link NyARBinRaster}を入力する場合
     * <ul>
     * <li>{@link NyARBufferType#INT1D_BIN_8}
     * </ul>
     * </p>
     * <p>{@link NyARGrayscaleRaster}を入力する場合
     * <ul>
     * <li>{@link NyARBufferType#INT1D_GRAY_8}
     * </ul>
     * </p>
     * </p>
     */
    public abstract class NyARLabeling_Rle
    {
        /**
         * NyARLabeling_Rleクラスが使用するピクセルドライブインタフェイスです。
         */
        public interface IRasterDriver
        {
            /**
             * 現在のラスタの指定点(x,y)から、幅i_lenの領域をRLE圧縮して返します。
             * @param i_x
             * @param i_y
             * @param i_len
             * @param i_out
             * @return
             */
            int xLineToRle(int i_x, int i_y, int i_len, int i_th, RleElement[] i_out);
        }
        /**
         * Labeling用の画像ドライバを構築します。
         */
        public class RasterDriverFactory
        {
            /**
             * この関数はラスタから呼ばれる。
             * @param i_raster
             * @return
             */
            public static NyARLabeling_Rle.IRasterDriver createDriver(INyARGrayscaleRaster i_raster)
            {
                switch (i_raster.getBufferType())
                {
                    case NyARBufferType.INT1D_GRAY_8:
                    case NyARBufferType.INT1D_BIN_8:
                        return new NyARRlePixelDriver_BIN_GS8(i_raster);
                    default:
                        if (i_raster is INyARGrayscaleRaster)
                        {
                            return new NyARRlePixelDriver_GSReader((INyARGrayscaleRaster)i_raster);
                        }
                        throw new NyARRuntimeException();
                }
            }
        }
        /**
         * このクラスは、{@link RleInfoStack}の要素です。
         * RLEフラグメントのパラメータを保持します。
         * ユーザが使うことはありません。
         */
        public class RleElement
        {
            public int l;
            public int r;
            public int fid;
            public static RleElement[] createArray(int i_length)
            {
                RleElement[] ret = new RleElement[i_length];
                for (int i = 0; i < i_length; i++)
                {
                    ret[i] = new RleElement();
                }
                return ret;
            }
        }
        private const int AR_AREA_MAX = 100000;// #define AR_AREA_MAX 100000
        private const int AR_AREA_MIN = 70;// #define AR_AREA_MIN 70
        private RleInfoStack _rlestack;
        private RleElement[] _rle1;
        private RleElement[] _rle2;
        private int _max_area;
        private int _min_area;
        /** 入力ラスタのサイズ*/
        protected NyARIntSize _raster_size = new NyARIntSize();
        /**
         * コンストラクタです。{@link #labeling}に入力するラスタのサイズを指定して、インスタンスを生成します。
         * @param i_width
         * 入力画像の幅
         * @param i_height
         * 入力画像の高さ
         * @
         */
        public NyARLabeling_Rle(int i_width, int i_height)
        {
            this.initInstance(i_width, i_height);
        }

        protected void initInstance(int i_width, int i_height)
        {
            this._raster_size.setValue(i_width, i_height);
            //120KB/QVGA +4K
            long t = (long)i_width * i_height * 3000 / (320 * 240) + 100;//full HD support
            this._rlestack = new RleInfoStack((int)t);
            this._rle1 = RleElement.createArray(i_width / 2 + 1);
            this._rle2 = RleElement.createArray(i_width / 2 + 1);
            this._max_area = AR_AREA_MAX;
            this._min_area = AR_AREA_MIN;
            return;
        }
        /**
         * 検出するラベルのエリア（画素数）範囲を設定します。
         * この範囲にあるラベルのみが、結果に返されます。
         * 初期値は、{@link #AR_AREA_MAX},{@link #AR_AREA_MIN}です。
         * @param i_max
         * エリアの最大値を指定します。
         * @param i_min
         * エリアの最小値を指定します。
         */
        public void setAreaRange(int i_max, int i_min)
        {
            Debug.Assert(i_min > 0 && i_max > i_min);
            this._max_area = i_max;
            this._min_area = i_min;
            return;
        }
        /**
         * フラグメントをRLEスタックへ追加する。
         * @param i_rel_img
         * @param i_nof
         * @param i_row_index
         * @param o_stack
         * @return
         * @
         */
        private bool addFragment(RleElement i_rel_img, int i_nof, int i_row_index, RleInfoStack o_stack)
        {
            int l = i_rel_img.l;
            int len = i_rel_img.r - l;
            i_rel_img.fid = i_nof;// REL毎の固有ID
            NyARRleLabelFragmentInfo v = o_stack.prePush();
            if (v == null)
            {
                return false;
            }
            v.entry_x = l;
            v.area = len;
            v.clip_l = l;
            v.clip_r = i_rel_img.r - 1;
            v.clip_t = i_row_index;
            v.clip_b = i_row_index;
            v.pos_x = (len * (2 * l + (len - 1))) / 2;
            v.pos_y = i_row_index * len;

            return true;
        }
        /**
         * この関数は、ラスタを敷居値i_thで2値化して、ラベリングします。
         * 検出したラベルは、自己コールバック関数{@link #onLabelFound}で通知します。
         * @param i_bin_raster
         * 入力画像。対応する形式は、クラスの説明を参照してください。
         * @param i_th
         * 敷居値を指定します。2値画像の場合は、0を指定してください。
         * @
         */
        public virtual bool labeling(INyARGrayscaleRaster i_raster, int i_th)
        {
            NyARIntSize size = i_raster.getSize();
            return this.imple_labeling(i_raster, i_th, 0, 0, size.w, size.h);
        }
        /**
         * この関数は、ラスタを敷居値i_thで2値化して、ラベリングします。
         * 検出したラベルは、自己コールバック関数{@link #onLabelFound}で通知します。
         * @param i_bin_raster
         * 入力画像。対応する形式は、クラスの説明を参照してください。
         * @param i_area
         * ラべリングする画像内の範囲
         * @param i_th
         * 敷居値
         * @
         */
        public virtual bool labeling(INyARGrayscaleRaster i_raster, NyARIntRect i_area, int i_th)
        {
            return this.imple_labeling(i_raster, 0, i_area.x, i_area.y, i_area.w, i_area.h);
        }
        private INyARRaster _last_input_raster = null;
        private IRasterDriver _image_driver;

 	    /**
	     * 
	     * ラベリングの実体。
	     * @return
	     * ラベル数が上限に達したときはfalse
	     */
	    private bool imple_labeling(INyARRaster i_raster,int i_th,int i_left,int i_top,int i_width, int i_height)
	    {
		//ラスタのサイズを確認
		Debug.Assert(i_raster.getSize().isEqualSize(this._raster_size));
		//ラスタドライバのチェック
		if(_last_input_raster!=i_raster){
			this._image_driver=(IRasterDriver)i_raster.createInterface(typeof(IRasterDriver));
		}
		IRasterDriver pixdrv=this._image_driver;
		RleElement[] rle_prev = this._rle1;
		RleElement[] rle_current = this._rle2;
		// リセット処理
		 RleInfoStack rlestack=this._rlestack;
		rlestack.clear();

		//
		int len_prev = 0;
		int len_current = 0;
		int bottom=i_top+i_height;
		int id_max = 0;
		int ypos=i_top;
		// 初段登録
		len_prev = pixdrv.xLineToRle(i_left,ypos,i_width,i_th,rle_prev);
		for (int i = 0; i < len_prev; i++) {
			// フラグメントID=フラグメント初期値、POS=Y値、RELインデクス=行
            if (addFragment(rle_prev[i], id_max, ypos, rlestack))
            {
                id_max++;
			}else{
				return false;
            }
		}
		NyARRleLabelFragmentInfo[] f_array = rlestack.getArray();
		// 次段結合
		for (int y = i_top + 1; y < bottom; y++) {
			// カレント行の読込
			
			ypos++;
			len_current = pixdrv.xLineToRle(i_left,ypos,i_width,i_th, rle_current);
			int index_prev = 0;

			for (int i = 0; i < len_current; i++) {
				// index_prev,len_prevの位置を調整する
				int id = -1;
				// チェックすべきprevがあれば確認
				while (index_prev < len_prev) {
					if (rle_current[i].l - rle_prev[index_prev].r > 0) {// 0なら8方位ラベリング
						// prevがcurの左方にある→次のフラグメントを探索
						index_prev++;
						continue;
					} else if (rle_prev[index_prev].l - rle_current[i].r > 0) {// 0なら8方位ラベリングになる
						// prevがcur右方にある→独立フラグメント
						if(addFragment(rle_current[i], id_max, y,rlestack)){
							id_max++;
                        }else{
                            return false;
                        }
						// 次のindexをしらべる
						goto SCAN_CUR;
					}
					id=rle_prev[index_prev].fid;//ルートフラグメントid
					NyARRleLabelFragmentInfo id_ptr = f_array[id];
					//結合対象(初回)->prevのIDをコピーして、ルートフラグメントの情報を更新
					rle_current[i].fid = id;//フラグメントIDを保存
					//
					 int l= rle_current[i].l;
					 int r= rle_current[i].r;
					 int len=r-l;
					//結合先フラグメントの情報を更新する。
					id_ptr.area += len;
					//tとentry_xは、結合先のを使うので更新しない。
					id_ptr.clip_l=l<id_ptr.clip_l?l:id_ptr.clip_l;
					id_ptr.clip_r=r>id_ptr.clip_r?r-1:id_ptr.clip_r;
					id_ptr.clip_b=y;
					id_ptr.pos_x+=(len*(2*l+(len-1)))/2;
					id_ptr.pos_y+=y*len;
					//多重結合の確認（２個目以降）
					index_prev++;
					while (index_prev < len_prev) {
						if (rle_current[i].l - rle_prev[index_prev].r > 0) {// 0なら8方位ラベリング
							// prevがcurの左方にある→prevはcurに連結していない。
							goto SCAN_PREV;
						} else if (rle_prev[index_prev].l - rle_current[i].r > 0) {// 0なら8方位ラベリングになる
							// prevがcurの右方にある→prevはcurに連結していない。
							index_prev--;
							goto SCAN_CUR;
						}
						// prevとcurは連結している→ルートフラグメントの統合
						
						//結合するルートフラグメントを取得
						 int prev_id =rle_prev[index_prev].fid;
						NyARRleLabelFragmentInfo prev_ptr = f_array[prev_id];
						if (id != prev_id){
							//prevとcurrentのフラグメントidを書き換える。
							for(int i2=index_prev;i2<len_prev;i2++){
								//prevは現在のidから最後まで
								if(rle_prev[i2].fid==prev_id){
									rle_prev[i2].fid=id;
								}
							}
							for(int i2=0;i2<i;i2++){
								//currentは0から現在-1まで
								if(rle_current[i2].fid==prev_id){
									rle_current[i2].fid=id;
								}
							}
							
							//現在のルートフラグメントに情報を集約
							id_ptr.area +=prev_ptr.area;
							id_ptr.pos_x+=prev_ptr.pos_x;
							id_ptr.pos_y+=prev_ptr.pos_y;
							//tとentry_xの決定
							if (id_ptr.clip_t > prev_ptr.clip_t) {
								// 現在の方が下にある。
								id_ptr.clip_t = prev_ptr.clip_t;
								id_ptr.entry_x = prev_ptr.entry_x;
							}else if (id_ptr.clip_t < prev_ptr.clip_t) {
								// 現在の方が上にある。prevにフィードバック
							} else {
								// 水平方向で小さい方がエントリポイント。
								if (id_ptr.entry_x > prev_ptr.entry_x) {
									id_ptr.entry_x = prev_ptr.entry_x;
								}else{
								}
							}
							//lの決定
							if (id_ptr.clip_l > prev_ptr.clip_l) {
								id_ptr.clip_l=prev_ptr.clip_l;
							}else{
							}
							//rの決定
							if (id_ptr.clip_r < prev_ptr.clip_r) {
								id_ptr.clip_r=prev_ptr.clip_r;
							}else{
							}
							//bの決定

							//結合済のルートフラグメントを無効化する。
							prev_ptr.area=0;
						}


						index_prev++;
					}
					index_prev--;
					break;
                SCAN_PREV: ;
				}
				// curにidが割り当てられたかを確認
				// 右端独立フラグメントを追加
				if (id < 0){
					if(addFragment(rle_current[i], id_max, y,rlestack)){
						id_max++;
                    }else{
                        return false;
                    }
				}
            SCAN_CUR: ;
            }
			// prevとrelの交換
			RleElement[] tmp = rle_prev;
			rle_prev = rle_current;
			len_prev = len_current;
			rle_current = tmp;
		}
		//対象のラベルだけを追記
		 int max=this._max_area;
		 int min=this._min_area;
		for(int i=id_max-1;i>=0;i--){
			 NyARRleLabelFragmentInfo src_info=f_array[i];
			 int area=src_info.area;
			if(area<min || area>max){//対象外のエリア0のもminではじく
				continue;
			}
			//値を相対位置に補正
			src_info.clip_l+=i_left;
			src_info.clip_r+=i_left;
			src_info.entry_x+=i_left;
			src_info.pos_x/=area;
			src_info.pos_y/=area;
			//コールバック関数コール
			this.onLabelFound(src_info);
		}
        return true;
	}
        /**
         * この仮想関数は自己コールバック関数です。
         * {@link #labeling}関数が、検出したラベルを通知するために使います。
         * @param i_ref_label
         * 検出したラベルを格納したオブジェクト。値の有効期間は、次の{@link #labeling}が実行されるまでです。
         * (注)この仕様は変わるかもしれません。
         * @
         */

        protected abstract void onLabelFound(NyARRleLabelFragmentInfo i_ref_label);

        /**
         * クラスの仕様確認フラグです。ラベル配列の参照アクセスが可能かを返します。
         * このクラスでは、true固定です。
         */
        public const bool _sf_label_array_safe_reference = true;
    }

    /**
     * このクラスは、{@link NyARLabeling_Rle_old}が内部的に使うRLEスタックです。
     * ユーザが使うことはありません。
     */
    class RleInfoStack : NyARObjectStack<NyARRleLabelFragmentInfo>
    {
        public RleInfoStack(int i_length)
            : base(i_length)
        {
            return;
        }

        protected override NyARRleLabelFragmentInfo createElement()
        {
            return new NyARRleLabelFragmentInfo();
        }
    }

    //
    //画像ドライバ
    //

    class NyARRlePixelDriver_BIN_GS8 : NyARLabeling_Rle.IRasterDriver
    {
        private INyARRaster _ref_raster;
        public NyARRlePixelDriver_BIN_GS8(INyARRaster i_ref_raster)
        {
            this._ref_raster = i_ref_raster;
        }
        public int xLineToRle(int i_x, int i_y, int i_len, int i_th, NyARLabeling_Rle.RleElement[] i_out)
        {
            int[] buf = (int[])this._ref_raster.getBuffer();
            int current = 0;
            int r = -1;
            // 行確定開始
            int st = i_x + this._ref_raster.getWidth() * i_y;
            int x = st;
            int right_edge = st + i_len - 1;
            while (x < right_edge)
            {
                // 暗点(0)スキャン
                if (buf[x] > i_th)
                {
                    x++;//明点
                    continue;
                }
                // 暗点発見→暗点長を調べる
                r = (x - st);
                i_out[current].l = r;
                r++;// 暗点+1
                x++;
                while (x < right_edge)
                {
                    if (buf[x] > i_th)
                    {
                        // 明点(1)→暗点(0)配列終了>登録
                        i_out[current].r = r;
                        current++;
                        x++;// 次点の確認。
                        r = -1;// 右端の位置を0に。
                        break;
                    }
                    else
                    {
                        // 暗点(0)長追加
                        r++;
                        x++;
                    }
                }
            }
            // 最後の1点だけ判定方法が少し違うの。
            if (buf[x] > i_th)
            {
                // 明点→rカウント中なら暗点配列終了>登録
                if (r >= 0)
                {
                    i_out[current].r = r;
                    current++;
                }
            }
            else
            {
                // 暗点→カウント中でなければl1で追加
                if (r >= 0)
                {
                    i_out[current].r = (r + 1);
                }
                else
                {
                    // 最後の1点の場合
                    i_out[current].l = (i_len - 1);
                    i_out[current].r = (i_len);
                }
                current++;
            }
            // 行確定
            return current;
        }
    }

    /**
     * GSPixelDriverを使ったクラス
     */
    class NyARRlePixelDriver_GSReader : NyARLabeling_Rle.IRasterDriver
    {
        readonly private INyARGrayscaleRaster _ref_raster;
        public NyARRlePixelDriver_GSReader(INyARGrayscaleRaster i_raster)
        {
            this._ref_raster = i_raster;
        }
        public int xLineToRle(int i_x, int i_y, int i_len, int i_th, NyARLabeling_Rle.RleElement[] i_out)
        {
            int current = 0;
            int r = -1;
            // 行確定開始
            int st = i_x;
            int x = st;
            int right_edge = st + i_len - 1;
            while (x < right_edge)
            {
                // 暗点(0)スキャン
                if (this._ref_raster.getPixel(x, i_y) > i_th)
                {
                    x++;//明点
                    continue;
                }
                // 暗点発見→暗点長を調べる
                r = (x - st);
                i_out[current].l = r;
                r++;// 暗点+1
                x++;
                while (x < right_edge)
                {
                    if (this._ref_raster.getPixel(x, i_y) > i_th)
                    {
                        // 明点(1)→暗点(0)配列終了>登録
                        i_out[current].r = r;
                        current++;
                        x++;// 次点の確認。
                        r = -1;// 右端の位置を0に。
                        break;
                    }
                    else
                    {
                        // 暗点(0)長追加
                        r++;
                        x++;
                    }
                }
            }
            // 最後の1点だけ判定方法が少し違うの。
            if (this._ref_raster.getPixel(x, i_y) > i_th)
            {
                // 明点→rカウント中なら暗点配列終了>登録
                if (r >= 0)
                {
                    i_out[current].r = r;
                    current++;
                }
            }
            else
            {
                // 暗点→カウント中でなければl1で追加
                if (r >= 0)
                {
                    i_out[current].r = (r + 1);
                }
                else
                {
                    // 最後の1点の場合
                    i_out[current].l = (i_len - 1);
                    i_out[current].r = (i_len);
                }
                current++;
            }
            // 行確定
            return current;
        }
    }
}
