import React from 'react'
import { useSelector, useDispatch } from 'react-redux';
import { selectScreen } from './store/screenSlice'

// Screen'de bir Popup yardımıyla aşağıdaki işlemlerin düzenlenmesine izin ver:
// - Kolon sıralamaları
// - Kolon genişlikleri
// - Kolon gizle/göstger
// - Kolon veri formatı
// - diğer...

const Configuration = React.memo((props) => {
	const { currentScreen, row, loading } = useSelector(selectScreen)


})

export default Configuration