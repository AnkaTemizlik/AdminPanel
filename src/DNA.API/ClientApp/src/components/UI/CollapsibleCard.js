import React, { useState } from 'react'
import { makeStyles } from '@material-ui/core/styles';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { Card, CardContent, Typography, CardActions, Box, IconButton, Collapse } from '@material-ui/core'
import clsx from 'clsx';

const useStyles = makeStyles((theme) => ({
	expand: {
		transform: 'rotate(0deg)',
		marginLeft: 'auto',
		transition: theme.transitions.create('transform', {
			duration: theme.transitions.duration.shortest,
		}),
	},
	expandOpen: {
		transform: 'rotate(180deg)',
	}
}))

const CollapsibleCard = ({ children, title, header, exp, addComponent }) => {
	const [expanded, setExpanded] = useState(exp);
	const classes = useStyles();
	const handleExpandClick = () => {
		setExpanded(!expanded);
	};

	return <Card variant="outlined" >
		<CardActions disableSpacing style={{ padding: "0px 8px", background: "whitesmoke" }}>
			<Box p={1}>
				<Typography variant={header}>
					{title}
				</Typography>
			</Box>
			<Box flexGrow="1"></Box>
			{expanded && addComponent}
			<IconButton
				className={clsx(classes.expand, {
					[classes.expandOpen]: expanded,
				})}
				onClick={handleExpandClick}
				aria-expanded={expanded}
			>
				<ExpandMoreIcon />
			</IconButton>
		</CardActions>

		<Collapse in={expanded} timeout="auto" unmountOnExit>
			<CardContent style={{ padding: "4px 12px 12px 12px" }}>
				{children}
			</CardContent>
		</Collapse>
	</Card>
}

export default CollapsibleCard