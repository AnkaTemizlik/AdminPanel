import React, { useEffect, useState, useCallback } from "react";
import { useSelector, useDispatch } from 'react-redux';
import { Switch, Link, useHistory, useParams, useRouteMatch, useLocation, Route } from "react-router-dom";
//import { Switch, Link, useRouteMatch, useHistory, Redirect, Route } from "react-router-dom";
import { Grid, Toolbar, Box, Button, IconButton, Badge, Paper, Typography, Hidden, Tabs, Tab, Collapse, Icon } from "@material-ui/core";
import { Tooltip } from "@material-ui/core";
import ArrowBackIcon from "@material-ui/icons/ArrowBack";
import Plugin from "../../../plugins";
import { useTranslation } from "../../../store/i18next";
import withSnack from "../../../store/snack";
import FooterView from "./components/FooterView";
import { setCurrentScreen, setRow, selectScreen, setLoading } from './store/screenSlice'
import DataTable from "../../../components/UI/Table/DataTable";
import ActionsView from "./components/ActionsView";
import View from "./View";

function useQuery() {
	return new URLSearchParams(useLocation().search);
}

const Screen = React.memo((props) => {
	const { name, snack } = props;
	const { t } = useTranslation();
	const history = useHistory();
	const params = useParams();
	let { path, url } = useRouteMatch();
	const dispatch = useDispatch();
	const { currentScreen, row, loading } = useSelector(selectScreen)
	const [error, setError] = useState(null);
	const [grid, setGrid] = useState(null);
	const [filter] = useState(params.field ? [params.field, '=', params.value] : null);

	const subMenu = currentScreen && currentScreen.subMenus && currentScreen.subMenus.find(f => f.name == params.subMenuName);

	const ActionsComponent = Plugin.Actions ? Plugin.Actions[name] : null;

	const refresh = () => {
		grid && grid.refresh();
	};

	useEffect(() => {
		if (error) {
			snack.show(error)
			setError(null)
		}
	}, [snack, error]);

	function focusedRowChanged(focused) {
		if (focused != row || (focused && focused[currentScreen.keyFieldName] != row[currentScreen.keyFieldName]))
			dispatch(setRow(focused || null))
	}

	function setLoadStatus(e) {
		dispatch(setLoading(e))
	}

	useEffect(() => {
		dispatch(setLoading(true))
		dispatch(setCurrentScreen(name))
	}, [dispatch, name]);


	return (
		<>
			{currentScreen &&
				<Grid container spacing={2} alignItems="stretch">
					<Grid item xs={12}>
						<Toolbar>
							<Tooltip title={t("Back")}>
								<span>
									<IconButton edge="start" onClick={() => history.goBack()} disabled={loading}>
										<ArrowBackIcon />
									</IconButton>
								</span>
							</Tooltip>
							<Box pt={1} pl={1}>
								<Typography variant="h4" gutterBottom>
									{t(currentScreen.title)}
								</Typography>
							</Box>

							<Box flexGrow={1} />

							<Box pt={1} pl={1}>
								<Icon style={{ fontSize: 48, opacity: "0.3" }}>{currentScreen.icon}</Icon>
							</Box>

						</Toolbar>
					</Grid>

					<Grid item xs={12}>

						{/* <Link to={`${url}/2`}>{name} 2</Link>
						&nbsp; &#128151; &nbsp;
						<Link to={`${url}`}>{name}</Link>
						<br /> */}

						<Switch>

							<Route path={`${url}/:id`} exact>
								<View name={name} />
							</Route>

							<Route>
								<Paper id="gridPaper" style={{ minHeight: '260px' }}>
									<Box p={1}>

										<DataTable
											id={"idFor" + name}
											name={name}
											keyFieldName={currentScreen.keyFieldName}
											gridOptions={currentScreen.grid}
											instance={setGrid}
											dataSource={currentScreen.dataSource}
											onFocusedRowChanged={focusedRowChanged}
											onError={setError}
											onRowPrepared={currentScreen.onRowPrepared}
											columns={currentScreen.columns}
											defaultFilter={(filter && subMenu && subMenu.defaultFilter) ? [filter, "and", subMenu.defaultFilter] : (filter ? filter : subMenu ? subMenu.defaultFilter : null)}
											editing={currentScreen.editing}
											actions={currentScreen.actions}
											actionsTemplate={() => (currentScreen.subModels || currentScreen.actions)
												? <ActionsView
													actions={currentScreen.actions}
													refresh={refresh}
													renderActions={
														() => ActionsComponent
															? <ActionsComponent
																row={row}
																setLoading={setLoadStatus}
																loading={loading}
																screens={currentScreen ? currentScreen.subScreens : []}
																refresh={refresh}
															/>
															: null
													} />
												: null}
										/>

									</Box>
								</Paper>
							</Route>
						</Switch>

					</Grid>

					{row &&
						<Grid item xs={12}>
							<FooterView screen={currentScreen} row={row} name={name} />
						</Grid>
					}
				</Grid>
			}
		</>
	);
});

export default withSnack(Screen)