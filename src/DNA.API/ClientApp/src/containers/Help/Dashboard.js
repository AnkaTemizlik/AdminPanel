import React, { useState } from "react";
import { Link, useRouteMatch, useHistory } from "react-router-dom";
import { makeStyles } from "@material-ui/core/styles";
import {
	Box,
	Grid,
	IconButton,
	Paper,
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Toolbar,
	Tooltip,
	Typography,
	Icon,
	Card,
	CardActionArea,
	CardMedia,
	CardContent,
	CardActions,
	Button,
	Divider,
	CardHeader,
	Avatar,
} from "@material-ui/core";
import { ArrowBack } from "@material-ui/icons";
import { Trans, Tr, useTranslation } from "../../store/i18next";
import Container from "../../components/Container";
import { deepOrange, deepPurple } from "@material-ui/core/colors";

const useStyles = makeStyles((theme) => ({
	avatar: {
		color: theme.palette.getContrastText(theme.palette.primary.main),
		backgroundColor: theme.palette.primary.main,
	},
}));

function AlertCodes(props) {
	const { menu } = props;
	const classes = useStyles();
	let { t } = useTranslation();
	let history = useHistory();
	let { url } = useRouteMatch();

	function render(menu, to, breadcrumb) {
		if (menu.menus.length > 0) {
			return menu.menus.map((m, i) => {
				return (
					<>
						{renderCard(m, m.noLink ? to : `${to}${m.to}`, m.label, breadcrumb)}
						{render(m, `${to}${m.to}`, `${breadcrumb} > ${m.label}`)}
					</>
				);
			});
		}
	}

	function renderCard(m, path, title, breadcrumb) {
		return (
			<Grid item xs={12} sm={6} md={4} lg={3} key={path + breadcrumb}>
				<Card>
					{title && (
						<CardHeader
							avatar={
								<Avatar className={classes.avatar}>
									<Icon>{m.icon ?? "warning"}</Icon>
								</Avatar>
							}
							title={title}
							subheader={breadcrumb}
						/>
					)}
					<CardActionArea component={Link} to={`${path}`}>
						<CardMedia>
							<Box display="flex" alignItems="center" alignContent="center" justifyContent="center">
								<Icon style={{ fontSize: 140, color: "lightgray" }}>{m.icon ?? "warning"}</Icon>
							</Box>
						</CardMedia>
						<CardContent>
							<Typography gutterBottom variant="h6">
								<Trans>{m.label}</Trans>
							</Typography>
							<Typography variant="body2" color="textSecondary" component="p">
								<Trans>{m.description}</Trans>
							</Typography>
						</CardContent>
					</CardActionArea>
					<CardActions>
						{/* <Button size="small" color="primary">
							Share
        		</Button> */}
						<Button size="small" color="primary" component={Link} to={`${path}`}>
							<Trans>Learn More</Trans>
						</Button>
					</CardActions>
				</Card>
			</Grid>
		);
	}

	return (
		<Container>
			<Grid container spacing={2} alignItems="stretch">
				<Grid item xs={12}>
					<Toolbar>
						<Tooltip title={t("Back")}>
							<span>
								<IconButton edge="start" onClick={() => history.goBack()}>
									<ArrowBack />
								</IconButton>
							</span>
						</Tooltip>
						<Box pt={1} pl={1}>
							<Typography variant="h4" gutterBottom>
								<Trans>Help Topics</Trans>
							</Typography>
						</Box>
					</Toolbar>
				</Grid>

				{render(menu, menu.to, menu.label)}

				{/* <Divider />
			{menu.menus.map(m => {
				if (m.menus.length > 0) {
					return <Grid item xs={12} key={m.name}>
						<Box pt={2}>
							<Typography variant="h4"> <Trans>{m.label}</Trans> </Typography>
							<Typography variant="body1" gutterBottom> <Trans>{m.description}</Trans> </Typography>
						</Box>
						{m.menus.map(s => {
							return renderCard(s, m.to, s.name)
						})}
					</Grid>
				}
				else
					return renderCard(m, '', m.name)
			})} */}
			</Grid>
		</Container>
	);
}

export default AlertCodes;
