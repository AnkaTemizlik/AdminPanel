import React from 'react'
import { Icon as IconifyIcon, getIcon } from '@iconify/react-with-api'
import Icon from "@material-ui/core/Icon";

const Iconify = ({ icon, fontSize, children, style, ...rest }) => {
	let name = icon || children
	let fs = fontSize || '1.285rem'
	//console.info("Iconify", name)
	return name && name.indexOf(':') > -1
		? <IconifyIcon
			icon={name.replace('_', '-')}
			{...rest}
			style={{ ...style, fontSize: fs }}
		/>
		: <Icon style={{ ...style, fontSize: fs }} {...rest}>{name || 'account'}</Icon>
}

export default Iconify