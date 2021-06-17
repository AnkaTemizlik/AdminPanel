import React from 'react'
import * as Treasury from "@mui-treasury/layout";
import AccountCircleTwoTone from '@material-ui/icons/AccountCircleTwoTone';
import { Avatar } from '@material-ui/core';
import { useSelector } from 'react-redux';

const SidebarTrigger = ({ secondary }) => {
	const { user } = useSelector(state => state.auth)
	// ["action","disabled","error","inherit","primary","secondary"]
	return secondary
		? <Treasury.SecondarySidebarTrigger style={{ marginLeft: "8px" }} >
			<Avatar
				style={{
					width: 32,
					height: 32,
					transition: '0.3s',
					//marginBottom: 24,
					//marginTop: 24,
				}}
				src={user ? user.PictureUrl : undefined}
			/>

		</Treasury.SecondarySidebarTrigger>
		: <Treasury.SidebarTrigger style={{ color: "white" }}>
			<Treasury.SidebarTriggerIcon />
		</Treasury.SidebarTrigger>
}

export default SidebarTrigger