import React, { useCallback, useEffect, useState } from "react";
import { connect } from "react-redux";
import { Switch, Link, useRouteMatch, Route, useHistory, useParams } from "react-router-dom";
import { CssBaseline, Box, Button, IconButton, Toolbar, Typography, Tooltip } from "@material-ui/core";
import NavigateBeforeIcon from "@material-ui/icons/NavigateBeforeTwoTone";
import HomeIcon from "@material-ui/icons/HomeTwoTone";
import DashboardIcon from "@material-ui/icons/DashboardTwoTone";
import * as Treasury from "@mui-treasury/layout";
import { useTranslation } from '../../store/i18next'
import Footer from "../../components/Navigation/Footer";
import Dashboard from "./Dashboard";
import AlertCodes from "./AlertCodes";
import Layout from "../../components/Layout";
import { HomeSidebar, SidebarTrigger } from "../../components/Navigation/Sidebars";
import Container from "../../components/Container";
import SidebarMenu from "../../components/UI/Menu/SidebarMenu";
import LanguageChanger from "../../components/UI/LanguageChanger";
import withSnack from "../../store/snack";
import api from "../../store/api";
import Page from "./Page";

const config = {
	xs: Treasury.getDefaultScreenConfig({
		sidebar: {
			variant: "temporary", // ["permanent","persistent","temporary"]
			width: 300,
		},
		secondarySidebar: {
			variant: "temporary",
			width: 300,
			collapsible: false,
		},
	}),
	md: Treasury.getDefaultScreenConfig({
		sidebar: {
			variant: "permanent",
			width: 300,
		},
		secondarySidebar: {
			variant: "temporary",
			collapsible: false,
			width: 300,
		},
		header: {
			position: "fixed", //["absolute","fixed","relative","static","sticky"]
			clipped: true,
		},
	}),
};

// https://www.muicss.com/docs/v1/css-js/tables

const Help = (props) => {
	let { path, url } = useRouteMatch();
	const { snack } = props;
	let helpMenu = props.menus ? props.menus.help || {} : {};
	let history = useHistory();
	let { t } = useTranslation();
	const [data, setData] = useState({ ...helpMenu });
	const [loading, setLoading] = useState(false);

	const getData = useCallback(() => {
		let url = "/api/help";
		setLoading(true);

		api.actions.run("GET", url).then((status) => {
			setLoading(false);
			if (status.Success) {
				setData((m) => {
					m.menus = [...helpMenu.menus, ...status.Resource.menus];
					return { ...m };
				});
			} else {
				snack.show(status);
			}
		});
	}, [helpMenu, snack]);

	useEffect(() => {
		getData();
	}, [getData]);

	let components = {
		alertCodes: AlertCodes,
	};

	const renderRoutes = () => {
		var routes = [];

		data.menus && data.menus.map((menu, i) => {
			let SpecificPage = components[menu.name];
			if (SpecificPage)
				routes.push(<Route key={i} path={`${url}${menu.to}`} exact>
					<SpecificPage key={i} menu={menu} />
				</Route>)
			else {
				routes.push(
					<Route key={i} path={`${url}${menu.to}`} exact>
						<Page menu={menu} />
					</Route>
				);

				menu && menu.menus && menu.menus.length > 0 &&
					routes.push(
						<Route key={i} path={`${url}${menu.to}/:to`} exact>
							<Page menus={menu.menus} />
						</Route>
					)
			}
		})

		return routes
	}

	return (
		<Layout config={config}>
			{({ headerStyles, sidebarStyles, footerStyles, setOpened, setSecondaryOpened }) => (
				<>
					<CssBaseline />

					<Treasury.Header color="primary">
						<Toolbar>
							<SidebarTrigger />

							<IconButton onClick={() => history.goBack()} color="inherit">
								<NavigateBeforeIcon />
							</IconButton>

							<Tooltip title={t("Home")}>
								<span>
									<IconButton component={Link} to="/" color="inherit">
										<HomeIcon />
									</IconButton>
								</span>
							</Tooltip>

							<Tooltip title={t("Panel")}>
								<span>
									<IconButton component={Link} to="/panel" color="inherit" >
										<DashboardIcon />
									</IconButton>
								</span>
							</Tooltip>

							<Box pl={2}>
								<Typography variant="h6">{data.label}</Typography>
							</Box>

							<Box flexGrow="1" />

							<SidebarTrigger secondary />
						</Toolbar>
					</Treasury.Header>

					{props.menus ? (
						<Treasury.Sidebar>
							<SidebarMenu setOpened={setOpened} menu={data} />
						</Treasury.Sidebar>
					) : null}

					{props.menus ? <HomeSidebar menus={props.menus} setOpened={setSecondaryOpened} user={props.user} /> : null}

					<Treasury.Content>
						<Container loading={loading}>
							<Switch>

								{renderRoutes().map(m => m)}

								<Route path={`${url}`}>
									<Dashboard menu={data} />
								</Route>

							</Switch>
						</Container>
					</Treasury.Content>

					<Treasury.Footer>
						<Footer menus={props.menus} />
					</Treasury.Footer>
				</>
			)
			}
		</Layout >
	);
};

const mapStateToProps = (state) => ({
	user: state.auth.user,
	isAuthenticated: state.auth.token != null,
	menus: state.auth.menus,
});

export default connect(mapStateToProps)(withSnack(Help));
