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
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.markersystem
{
    public class NyARSingleCameraSystem
    {
        protected NyARSingleCameraSystem(NyARSingleCameraView i_ref_view)
	    {
            this._view = i_ref_view;
            this._view.setClipping(
                NyARSingleCameraView.FRUSTUM_DEFAULT_NEAR_CLIP,
                NyARSingleCameraView.FRUSTUM_DEFAULT_FAR_CLIP);
    		
	    }
        readonly protected NyARSingleCameraView _view;
	    /**
	     * [readonly]
	     * 現在のフラスタムオブジェクトを返します。
	     * @return
	     */
	    public NyARFrustum getFrustum()
	    {
            return this._view.getFrustum();
	    }
        /**
         * [readonly]
         * 現在のカメラパラメータオブジェクトを返します。
         * @return
         */
        public NyARParam getARParam()
        {
            return this._view.getARParam();
        }
        public NyARSingleCameraView getSingleView()
        {
            return this._view;
        }
	    /**
	     * 視錐台パラメータを設定します。
	     * この関数は、値を更新後、登録済の{@link IObserver}オブジェクトへ、{@link #EV_UPDATE}通知を送信します。
	     * @param i_near
	     * 新しいNEARパラメータ
	     * @param i_far
	     * 新しいFARパラメータ
	     */
	    public virtual void setProjectionMatrixClipping(double i_near,double i_far)
	    {
            this._view.setClipping(i_near, i_far);
	    }	

    }
}
