import React, { useEffect, useState, useCallback } from "react";
import { useSelector, useDispatch } from 'react-redux';
import { Switch, Link, useHistory, useParams, useRouteMatch, useLocation, Route } from "react-router-dom";
//import { Switch, Link, useRouteMatch, useHistory, Redirect, Route } from "react-router-dom";
import { Grid, Toolbar, Box, IconButton, Badge, Paper, Typography, Hidden, Tabs, Tab, Collapse } from "@material-ui/core";
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
import Calendar from "./Calendar";
import { Button } from "devextreme-react";
import Iconify from "../../../components/UI/Icons/Iconify";

function useQuery() {
	return new URLSearchParams(useLocation().search);
}

const Screen = React.memo((props) => {
	const { name, snack } = props;
	const { t } = useTranslation();
	const history = useHistory();
	const params = useParams();
	const query = useQuery();
	let { path, url } = useRouteMatch();
	const dispatch = useDispatch();
	const { currentScreen, row, loading } = useSelector(selectScreen)
	const [error, setError] = useState(null);
	const [grid, setGrid] = useState(null);
	const [filter, setFilter] = useState(null);

	//console.info("filter", path, url, query.get("title"), params)

	const subMenu = currentScreen && currentScreen.subMenus && currentScreen.subMenus.find(f => f.name == params.subMenuName);

	const ActionsComponent = Plugin.Actions ? Plugin.Actions[name] : null;

	const refresh = () => {
		console.info("grid.refresh")
		grid && grid.refresh();
	};

	useEffect(() => {
		setFilter(params.field ? [params.field, '=', params.value] : null)
	}, [params.field, params.value]);

	useEffect(() => {
		if (error) {
			snack.show(error)
			setError(null)
		}
	}, [snack, error]);

	const focusedRowChanged = (focused) => {
		if (focused != row || (focused && focused[currentScreen.keyFieldName] != row[currentScreen.keyFieldName]))
			dispatch(setRow(focused || null))
	}

	const setLoadStatus = (e) => {
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

								{subMenu && subMenu.defaultFilter && query.get("title") && <Typography variant="body1" style={{ display: 'flex', alignItems: 'center' }}>
									<span>{query.get("title")}</span>
									<span>
										<Button
											onClick={() => history.push("/panel/screen/" + name)}
											icon="close"
											stylingMode="text"
											hint={t("Clear")}
											type="back"
											style={{ padding: 2 }}
										/>
									</span>
								</Typography>}
							</Box>

							<Box flexGrow={1} />

							<Box pt={1} pl={2}>
								<Iconify fontSize={64} style={{ opacity: "0.20" }}>{currentScreen.icon}</Iconify>
							</Box>

						</Toolbar>
					</Grid>

					<Grid item xs={12}>

						<Switch>

							{currentScreen.calendar &&
								<Route path={`/panel/screen/${name}/calendar`}>
									<Calendar name={name} />
								</Route>
							}

							<Route path={`/panel/screen/${name}/new`}>
								<View name={name} action="new" onInsert={(d) => {
									var u = "/panel/screen/" + name + "/edit/" + d
									console.purple("onInsert", u)
									history.push(u)
								}} />
							</Route>

							<Route path={`/panel/screen/${name}/edit/:id`}>
								<View name={name} action="edit" />
							</Route>

							<Route>
								<Paper id="gridPaper" style={{ minHeight: '260px' }}>
									<Box p={1}>

										<DataTable
											id={"idFor" + name}
											name={name}
											title={t(name)}
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
											defaultValue={currentScreen.newRowDefaultValues}
											actionsTemplate={() => (currentScreen.actions)
												? <ActionsView
													actions={currentScreen.actions}
													refresh={refresh}
													renderActions={
														() => ActionsComponent
															? <ActionsComponent
																row={row}
																setLoading={setLoadStatus}
																loading={loading}
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