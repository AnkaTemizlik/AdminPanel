import React, { useEffect } from "react";
import { connect, useDispatch, useSelector } from "react-redux";
import { Switch, Link, useRouteMatch } from "react-router-dom";
import { useTranslation } from "../../store/i18next";
import { CssBaseline, Box, Toolbar, Hidden, IconButton, Divider, Typography, Tooltip } from "@material-ui/core";
import DashboardIcon from "@material-ui/icons/DashboardTwoTone";
import HomeIcon from "@material-ui/icons/HomeTwoTone";
import * as Treasury from "@mui-treasury/layout";
import Footer from "../../components/Navigation/Footer";
import Layout from "../../components/Layout";
import Container from "../../components/Container";
import { SidebarTrigger, HomeSidebar } from "../../components/Navigation/Sidebars";
import PrivateRoute from "../../components/Route/PrivateRoute";
import Dashboard from "./Dashboard";
import Screen from "./Screen/index";
import SidebarMenu from "../../components/UI/Menu/SidebarMenu";
import Plugin from "../../plugins";
import LanguageChanger from "../../components/UI/LanguageChanger";
import NotificationComponent from "../../components/Notification";
import Settings from "../Admin/Settings";
import Users from "../Admin/Users";
import UserEdit from "../Admin/UserEdit";
import Logs from "../Admin/Logs";
import { selectScreen } from './Screen/store/screenSlice'
import { getSettings } from '../../store/slices/settingsSlice'

const treasuryConfig = {
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
		},
		secondarySidebar: {
			variant: "temporary",
			collapsible: false,
			width: 300,
		},
		header: {
			position: "absolute", // ["absolute","fixed","relative","static","sticky"]
			clipped: true,
		},
	}),
};

const Panel = (props) => {
	//const { snack } = props;
	let dispatch = useDispatch();
	let { t } = useTranslation();
	let { path, url } = useRouteMatch();
	const { menus, screenConfig, settings } = useSelector((state) => state)
	const isAuthenticated = useSelector((state) => state.auth.token !== null)
	const { loading } = useSelector(selectScreen)
	const { names } = useSelector((state) => state.screenConfig)
	let panelMenu = menus ? menus.panel || {} : {};

	useEffect(() => {
		if (isAuthenticated)
			dispatch(getSettings())
	}, [dispatch, isAuthenticated]);

	return (
		<Layout config={treasuryConfig}>
			{(args) => {
				const { footerStyles, setOpened, setSecondaryOpened } = args;

				return (
					<>
						<CssBaseline />

						<Treasury.Header color="primary">
							<Toolbar>
								<SidebarTrigger />

								{settings.Plugin.AuthSettings.GoPanelOnStart == false &&
									<Tooltip title="Home">
										<span>
											<IconButton component={Link} to="/" color="inherit">
												<HomeIcon />
											</IconButton>
										</span>
									</Tooltip>
								}

								<Tooltip title="Dashboard">
									<span>
										<IconButton component={Link} to={path} color="inherit">
											<DashboardIcon />
										</IconButton>
									</span>
								</Tooltip>

								<Hidden xsDown>
									<Box flexGrow="1" />
								</Hidden>

								<Box pl={2}>
									<Typography variant="h6">{settings.Plugin.ProgramName}</Typography>
								</Box>

								<Box flexGrow="1" />

								{settings && settings.Notification && settings.Notification.Enabled &&
									<NotificationComponent refreshIn={settings.Notification.RefreshCycleInMinutes} />
								}

								{process.env && process.env.NODE_ENV === "development" && <LanguageChanger />}

								<SidebarTrigger secondary />
							</Toolbar>
						</Treasury.Header>

						{menus ? (
							<Treasury.Sidebar>
								<SidebarMenu setOpened={setOpened} menu={menus.panel} />
								<Divider />
								<SidebarMenu setOpened={setOpened} menu={menus.help} />
							</Treasury.Sidebar>
						) : null}

						{menus ? <HomeSidebar setOpened={setSecondaryOpened} menus={menus} user={props.user} /> : null}

						<Treasury.Content>
							<Container loading={loading}>
								<Switch>

									{names && names.map((n, i) => {
										var s = screenConfig.screens[n];
										if (s.type == "customPage") {
											// Plugin.Pages
											if (Plugin.Pages) {
												const CustomPage = Plugin.Pages[n];
												if (CustomPage)
													return (
														<PrivateRoute path={`${url}/screen/${s.route}`} key={i}>
															<CustomPage screen={s} />
														</PrivateRoute>
													);
											}
											return null;
										} else {

											const routes = [];
											if (s.calendar) {
												routes.push(
													<PrivateRoute path={`${url}/screen/${s.route}/calendar`} exact key={s.route + '/calendar'}>
														<Screen name={n} />
													</PrivateRoute>
												);
											}

											s.subMenus && s.subMenus.map((f, j) => {
												routes.push(
													<PrivateRoute path={`${url}/screen/${s.route}/:subMenuName`} exact key={f.name + j}>
														<Screen name={n} />
													</PrivateRoute>
												);
											})

											// s.subModels && s.subModels.map((f, j) => {
											// 	let sms = screenConfig.screens[f.name]
											// 	if (f.type == "list" && f.visible !== false) {
											routes.push(
												<PrivateRoute path={`${url}/screen/${n}/:field/:value`} key={n + i}>
													<Screen name={n} />
												</PrivateRoute>
											);
											// 	}
											// })

											routes.push(
												<PrivateRoute path={`${url}/screen/${s.route ?? n}`} key={n + i}>
													<Screen name={n} />
												</PrivateRoute>
											);
											return routes;
										}
									})}
									{/* SpecificPage */}
									{panelMenu.menus &&
										panelMenu.menus.map((menu, i) => {
											const SpecificPage = (Plugin && Plugin.Components) ? Plugin.Components[menu.name] : null;

											if (SpecificPage)
												return (
													<PrivateRoute key={i} path={`${url}${menu.to}`}>
														<SpecificPage menu={menu} />
													</PrivateRoute>
												);
											else
												return null
										})}

									<PrivateRoute path={`${url}/settings`}>
										<Settings />
									</PrivateRoute>

									<PrivateRoute path={`${url}/Logs`}>
										<Logs />
									</PrivateRoute>

									<PrivateRoute path={`${url}/users/new`}>
										<UserEdit me={props.user} />
									</PrivateRoute>
									<PrivateRoute path={`${url}/users/:id`}>
										<UserEdit me={props.user} />
									</PrivateRoute>
									<PrivateRoute path={`${url}/users`}>
										<Users />
									</PrivateRoute>

									{/* default */}
									{screenConfig && (
										<PrivateRoute path={`${url}`}>
											<Dashboard />
										</PrivateRoute>
									)}
								</Switch>
							</Container>
						</Treasury.Content>

						<Treasury.Footer>
							<Footer menus={menus} />
						</Treasury.Footer>
					</>
				);
			}}
		</Layout>
	);
};

const mapStateToProps = (state) => ({
	user: state.auth.user
});

export default connect(mapStateToProps)(Panel);
