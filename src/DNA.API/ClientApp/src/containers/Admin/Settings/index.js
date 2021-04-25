import React, { useEffect, useState } from "react";
import { connect, useDispatch, useSelector } from "react-redux";
import { useHistory } from "react-router-dom";
import Drawer from 'devextreme-react/drawer';
import List from 'devextreme-react/list.js';
import SaveIcon from "@material-ui/icons/Save";
import UndoIcon from "@material-ui/icons/Undo";
import { ArrowBack } from "@material-ui/icons";
import {
	Grid, Typography, Button, Paper, Box, Toolbar, IconButton, Badge,
	Table, TableBody, TableHead, TableRow, TableCell, Tooltip, Collapse, Icon,
} from "@material-ui/core";
import Container from "../../../components/Container";
import { Trans, useTranslation } from "../../../store/i18next";
import withSnack from "../../../store/snack";
import { Alert } from "@material-ui/lab";
import Section from './Section';
import { setCurrentConfig, changeSection, discardChanges, addChange, checkRestartWarn, saveAppSettings } from '../../../store/slices/appsettingsSlice'

const Settings = (props) => {

	let { t } = useTranslation();
	let history = useHistory();
	const dispatch = useDispatch();

	const { configs: appsetting } = useSelector((state) => state.appsettings)
	const { currentSection } = useSelector((state) => state.appsettings)
	const { sectionNames } = useSelector((state) => state.appsettings)
	const { changeCount, restartWarn } = useSelector((state) => state.appsettings)
	const { error, snack } = props;
	const [loading] = useState(null);
	const [open, setOpen] = useState(true);
	const [selectedSection, setSelectedSection] = useState(null);

	useEffect(() => {
		if (error) snack.error(error);
	}, [error, snack]);

	useEffect(() => {
		selectedSection && dispatch(changeSection(selectedSection.name));
	}, [dispatch, selectedSection]);

	return (
		<Container loading={loading}>
			<Grid container spacing={2}>
				<Grid item xs={12}>
					<Toolbar>
						<Tooltip title={t("Back")}>
							<span>
								<IconButton edge="start" onClick={() => history.goBack()} disabled={loading}>
									<ArrowBack />
								</IconButton>
							</span>
						</Tooltip>
						<Box pt={1} pl={1}>
							<Typography variant="h4" gutterBottom>
								<Trans>{appsetting.title}</Trans>
							</Typography>
						</Box>

						<Box flexGrow={1} />

						<Box pt={1} pl={1}>
							<Icon style={{ fontSize: 48, opacity: "0.3" }}>{appsetting.icon}</Icon>
						</Box>
					</Toolbar>
				</Grid>

				<Grid item xs={12} style={{ padding: '0 8px' }}>
					<Collapse in={restartWarn} timeout="auto" unmountOnExit>
						<Alert variant="filled" color="warning">
							{t("APIRestartRequiredForChangesEffect")}
						</Alert>
					</Collapse>
				</Grid>

				<Grid item xs={12}>
					<Table stickyHeader>
						<TableHead>
							<TableRow>
								<TableCell style={{ padding: 0 }}>
									<Toolbar component={Paper}>
										<IconButton onClick={() => setOpen(!open)}>
											<Icon>menu</Icon>
										</IconButton>
										<IconButton
											onClick={() => dispatch(saveAppSettings())}
											disabled={changeCount == 0 || loading}>
											<Badge color="secondary" badgeContent={changeCount}>
												<SaveIcon />
											</Badge>
										</IconButton>
										<IconButton onClick={() => dispatch(discardChanges())} disabled={changeCount == 0 || loading}>
											<UndoIcon />
										</IconButton>
									</Toolbar>
								</TableCell>
							</TableRow>
						</TableHead>
						<TableBody>
							<TableRow>
								<TableCell style={{ padding: "8px 0" }}>

									{sectionNames && <Drawer
										opened={open}
										openedStateMode={"shrink"}
										position={"left"}
										//revealMode={revealMode}
										component={() => <div style={{ width: 200, padding: '8px 16px 0 0' }}>
											<List
												hoverStateEnabled={true}
												selectionMode="single"
												focusStateEnabled={false}
												dataSource={sectionNames}
												selectedItems={[selectedSection]}
												itemRender={(i) => i.caption}
												onSelectionChanged={(i) => {
													setSelectedSection(i.addedItems[0])
												}}
											/>
										</div>}
										//closeOnOutsideClick={this.onOutsideClick}
										minHeight={400}>
										<div id="content" className="dx-theme-background-color">
											{currentSection
												? <Section
													onValueChange={(fieldName, newValue) => {
														dispatch(checkRestartWarn({ fieldName, selectedSection: selectedSection.name }))
														dispatch(addChange({ fullname: fieldName, value: newValue }))
													}}
												/>
												: <Grid container style={{ minHeight: 500 }} justify="center" alignItems="center">
													<Icon style={{ fontSize: 240, opacity: "0.1" }}>{appsetting.icon}</Icon>
												</Grid>
											}
										</div>
									</Drawer>
									}

								</TableCell>
							</TableRow>
						</TableBody>
					</Table>
				</Grid>
			</Grid>
		</Container>
	);
};

const mapStateToProps = (state) => {
	return {
		roles: state.auth.user ? state.auth.user.roles : null,
		error: state.auth.error,
	};
};

export default connect(mapStateToProps)(withSnack(Settings));
