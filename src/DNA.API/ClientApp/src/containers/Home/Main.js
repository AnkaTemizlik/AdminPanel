import React from 'react'
import { Switch, Route } from 'react-router-dom';
import { Box } from '@material-ui/core'
import Welcome from './Welcome';
import plugin from '../../plugins'

function Main() {
	return <>
		<Box height="100vh" >
			<Switch>
				{plugin.Home
					? <Route path="/" component={plugin.Home} />
					: <Route path="/" component={Welcome} />
				}
			</Switch>
		</Box>
	</>
}

export default Main