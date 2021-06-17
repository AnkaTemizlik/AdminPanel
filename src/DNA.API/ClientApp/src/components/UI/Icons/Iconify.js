import React, { useRef, useEffect, useState } from 'react'
import { Icon as IconifyIcon, getIcon } from '@iconify/react-with-api'
import SvgIcon from "@material-ui/core/SvgIcon";
import Icon from "@material-ui/core/Icon";

const Iconify = ({ icon, fontSize, children, style, ...rest }) => {
	//const spanRef = useRef(null);

	//const [data, setData] = useState(null)

	// useEffect(() => {
	// 	let name = icon.replace('_', '-')
	// 	if (name.indexOf(':') > -1) {
	// 		setData(name)
	// 	}
	// 	else {
	// 		name = "ic:" + name
	// 		setData(null)
	// 	}
	// }, [icon]);

	return (icon || children).indexOf(':') > -1
		? <IconifyIcon
			icon={(icon || children).replace('_', '-')}
			{...rest}
			style={{ ...style, fontSize: fontSize || '2em' }}
		/>
		: <Icon style={{ ...style, fontSize: fontSize }} {...rest}>{(icon || children)}</Icon>
}

export default Iconify