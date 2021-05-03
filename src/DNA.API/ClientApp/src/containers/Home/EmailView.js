import { AppBar, Toolbar } from '@material-ui/core'
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
	const [height, setHeight] = useState(500)

	useEffect(() => {
		api.execute("GET", `/files/emails/${uniqueId}.json`)
			.then(status => {
				console.info("EmailView", status)
				setData(status)
			})
	}, [uniqueId])

	return <div>
		<AppBar position="static">
			<Toolbar />
		</AppBar>

		{data.htmlView
			? <iframe
				width="100%"
				height={height}
				style={{ border: 'none' }}
				title="email-view"
				srcDoc={data.htmlView}
				sandbox="allow-same-origin allow-popups allow-scripts allow-forms allow-full-screen"
				onLoad={(obj) => {
					setHeight(obj.target.contentWindow.document.documentElement.scrollHeight);
				}}
			/>
			: "loading..."}
	</div>
}

export default EmailView