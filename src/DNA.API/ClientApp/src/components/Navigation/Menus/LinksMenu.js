import React from 'react'
import { makeStyles } from '@material-ui/core/styles'
import { Box, Typography, Link, Grid, Divider, ListItem, ListItemText } from '@material-ui/core'
import { Trans } from '../../../store/i18next'

const useStyles = makeStyles((theme) => ({
	link: {
		color: theme.palette.text.primary,
	}
}));

const LinksMenu = ({ menu }) => {

	const classes = useStyles();

	return <Grid container>
		<Grid item xs={12}>
			<Box p={2}>
				<Typography color={"inherit"} variant="h5">
					<Trans>{menu.label}</Trans>
				</Typography>
			</Box>
		</Grid>
		<Grid item xs={12}>
			{menu.menus.map((m, i) => {
				if (m.divider)
					return <Divider key={i} />
				return <ListItem button component={Link} to={m.to} key={i}>
					<ListItemText className={classes.link}>
						<Trans>{m.label}</Trans>
					</ListItemText>
				</ListItem>
			})}
		</Grid>
	</Grid>
}

export default LinksMenu