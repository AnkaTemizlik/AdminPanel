import React from 'react'
import { Box, Tooltip, Button, Icon, ButtonGroup } from '@material-ui/core';

const Actions = (props) => {
	const { actions, execute, loading } = props

	return <Box pl={1}>
		<ButtonGroup size="small" variant="contained" color="primary">
			{actions.Names.map((n, i) => {
				var action = actions[n];
				if (!action)
					return null
				if (action.visible == false)
					return null
				return <Button key={i}
					onClick={() => execute(action)}
					disabled={loading}
					startIcon={action.icon ? (action.showTitle === false ? null : <Icon>{action.icon}</Icon>) : null}
				>
					<Tooltip title={action.tooltip}>
						{action.showTitle === false
							? <Icon fontSize="small">{action.icon}</Icon>
							: <span>{action.title}</span>
						}
					</Tooltip>
				</Button>
			})}
		</ButtonGroup>
	</Box>
}

export default Actions