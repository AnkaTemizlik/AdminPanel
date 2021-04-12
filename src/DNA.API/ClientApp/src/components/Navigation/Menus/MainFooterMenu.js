import React from "react";
import { makeStyles } from "@material-ui/core/styles";
import { Grid, Box, Typography, Divider, ListItem, ListItemText, Link } from "@material-ui/core";

const useStyles = makeStyles((theme) => ({
	button: {
		textTransform: "none",
	},
	link: {
		color: theme.palette.text.primary,
	},
}));

const MainFooterMenu = ({ menu }) => {
	//const classes = useStyles();

	return (
		<Grid container>
			<Grid item xs={12}>
				<Box p={2}>
					<Typography color={"inherit"} variant="h5">
						MENU 1
					</Typography>
				</Box>
				<Divider />
			</Grid>
			<Grid item xs={12}>
				<Box p={2}>
					<Typography color={"inherit"} variant="h5">
						MENU 2
					</Typography>
				</Box>
				<Divider />
				{menu.menus.map((m, i) => {
					if (!m.menus)
						return (
							<ListItem button component={Link} to={m.to} key={i}>
								<ListItemText>{m.label}</ListItemText>
							</ListItem>
						);
					return null;
				})}
			</Grid>
		</Grid>
	);
};

export default MainFooterMenu;
