import React, { useEffect, useState } from 'react'
import { Typography, Grid } from '@material-ui/core'
import api from '../../../store/api'
import Container from '../../../components/Container'

const UserManagement = () => {

	const [loading, setLoading] = useState(false)
	const [data, setData] = useState({})

	useEffect(() => {
		setLoading(true)
		api.auth.getUsers().then((status) => {
			setLoading(false)
			setData(status.Resource)
		})
	}, [])

	return <Container loading={loading}>
		<Grid container spacing={2} alignItems="stretch">
			<Grid item xs={12}>
				<Typography variant="h4" gutterBottom>
					User Management
                </Typography>
			</Grid>
			<Grid item xs={12}>
				<pre>
					{JSON.stringify(data, null, 2)}
				</pre>
			</Grid>
		</Grid>
	</Container>
}

export default UserManagement