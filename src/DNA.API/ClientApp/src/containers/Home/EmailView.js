import { AppBar, Box, Toolbar, Typography } from '@material-ui/core'
import React, { useEffect, useState } from 'react'
import { useLocation, useParams } from 'react-router-dom'
import api from '../../store/api'
/*
{
	toName,
	toAddress,
	cc,
	subject,
	replyTo,
	attachments,
	htmlView
}
*/
const EmailView = (props) => {
	const { confirmationCode, uniqueId, url } = useParams()
	const [data, setData] = useState({})
	const [loading, setLoading] = useState(false)
	const [height, setHeight] = useState(500)

	useEffect(() => {
		setLoading(true)
		api.execute("GET", `/files/emails/${uniqueId}.json`)
			.then(status => {
				console.info("EmailView", status)
				setData(status)
				setLoading(false)
			})
	}, [uniqueId])

	return <div>
		<AppBar position="static">
			<Toolbar />
		</AppBar>
		{loading
			? <Box p={6}>
				<Typography variant="body1">
					Lütfen bekleyin...
				</Typography>
			</Box>
			: data.htmlView
				? <iframe
					width="100%"
					height={height}
					style={{ border: 'none' }}
					title="email-view"
					srcDoc={data.htmlView}
					sandbox="allow-same-origin allow-popups allow-scripts allow-forms"
					onLoad={(obj) => {
						setHeight(obj.target.contentWindow.document.documentElement.scrollHeight);
					}}
				/>
				: <Box p={6}>
					<Typography variant="body1" >
						E-posta yüklenirken bir hata oluştu. Lüfen sistem yöneticisine danışın.
				</Typography>
				</Box>
		}
	</div>
}

export default EmailView