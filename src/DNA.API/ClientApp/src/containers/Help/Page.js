import React, { useCallback, useEffect, useState } from "react";
import withSnack from "../../store/snack";
import { Switch, Link, useRouteMatch, Route, useHistory, useParams } from "react-router-dom";
import { ArrowBack } from "@material-ui/icons";
import api from "../../store/api";
import { Box, Divider, Grid, IconButton, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Toolbar, Tooltip, Typography } from "@material-ui/core";
import Container from "../../components/Container";
import { Trans, Tr, useTranslation } from "../../store/i18next";
import { makeStyles } from "@material-ui/core/styles";
import { Alert } from "@material-ui/lab";

const useStyles = makeStyles((theme) => ({
	root: {
		"& table": {
			borderCollapse: "collapse",
		},
		"& th": {
			fontSize: "large",
			backgroundColor: "whitesmoke",
		},
		"& th, td": {
			border: "1px solid #e0e0e0",
			padding: 4,
		},
		"& td": {
			verticalAlign: "top",
		},
	},
}));

function Page(props) {
	const { menu, menus, snack } = props;
	const classes = useStyles();
	let { t } = useTranslation();
	let { path, url } = useRouteMatch();
	let { to } = useParams();
	let history = useHistory();
	const [data, setData] = useState("");
	const [error, setError] = useState(null);
	const [currentMenu, setCurrentMenu] = useState({});
	const [loading, setLoading] = useState(false);

	const getData = useCallback(() => {
		setLoading(true);
		api.actions.run("GET", "api" + url).then((status) => {
			console.purple("getData", status)
			if (status.Success) {
				setData(status.Resource.Value);
				setError(null)
			} else {
				setError(status)
			}
			setLoading(false);
		});
	}, [url]);

	useEffect(() => {
		if (error)
			snack.show(error);
	}, [error, snack]);

	useEffect(() => {
		getData();
		if (menu) {
			setCurrentMenu(menu);
		} else if (menus) {
			setCurrentMenu(menus.filter((_) => _.name == to)[0]);
		}
	}, [to, menu, menus, getData]);

	return (
		<Container loading={loading}>
			<Grid container spacing={2} alignItems="stretch">
				<Grid item xs={12}>
					<Toolbar>
						<Tooltip title={t("Back")}>
							<span>
								<IconButton edge="start" onClick={() => history.goBack()} disabled={loading}>
									<ArrowBack />
								</IconButton>
							</span>
						</Tooltip>

						{currentMenu && (
							<Box pt={1} pl={1}>
								<Typography variant="h4" gutterBottom>
									<Trans>{currentMenu.label}</Trans>
								</Typography>
							</Box>
						)}
					</Toolbar>
				</Grid>
				{currentMenu && currentMenu.description && (
					<Grid item xs={12}>
						<Alert severity="info">
							<Trans>{currentMenu.description}</Trans>
						</Alert>
					</Grid>
				)}
			</Grid>
			<Grid item xs={12}>
				<Box pt={4}>
					<Paper>
						<Box p={2}>{data ? <div className={classes.root} dangerouslySetInnerHTML={{ __html: data }} /> : null}</Box>
					</Paper>
				</Box>
			</Grid>
		</Container>
	);
}

export default withSnack(Page);
