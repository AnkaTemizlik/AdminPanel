import React from 'react'
import { TextField as MuiTextField } from '@material-ui/core'

const TextField = (props) => {
	const { size, multiline, margin, ...rest } = props
	return <MuiTextField {...rest}
		margin={margin || "dense"}
		fullWidth
		size={size || "small"}
		multiline={multiline === true}
		variant="outlined" />
}

export default TextField