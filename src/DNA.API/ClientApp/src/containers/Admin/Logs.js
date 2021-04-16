import React, { useEffect, useState } from "react";
import { useSelector } from 'react-redux';
import { useHistory } from "react-router-dom";
import {
	Grid,
	Toolbar,
	Box,
	IconButton,
	Paper,
	Card,
	CardHeader,
	Avatar,
	CardContent,
	Typography,
	CardActions,
	Chip,
} from "@material-ui/core";
import { Tooltip } from "@material-ui/core";
import InfoIcon from "@material-ui/icons/Info";
import ErrorIcon from "@material-ui/icons/Error";
import DebugIcon from "@material-ui/icons/BugReport";
import ComputerIcon from "@material-ui/icons/Computer";
import RefreshIcon from "@material-ui/icons/Refresh";
import { green, red, orange, blue } from "@material-ui/core/colors";
import api from "../../store/api";
import Container from "../../components/Container";
import { Trans, Tr, useTranslation } from "../../store/i18next";
import { ArrowBack } from "@material-ui/icons";
import withSnack from "../../store/snack";
import DataTable from "../../components/UI/Table/DataTable";
import { TextArea } from "devextreme-react";

const defaultColumns = [
	{ name: "Id", hidden: true, dataType: "number", defaultSortOrder: undefined },
	{ name: "Logged", title: "Tarih", width: 150, dataType: "date", format: "dd-MM-yy HH:mm:ss.SSS", defaultSortOrder: "desc" },
	{
		name: "Level", title: "Seviye", width: 80, dataType: "number", autoComplete: "LogLevelTypes"
	},
	{ name: "EntityName", title: "Model", width: 120 },
	{ name: "EntityKey", title: "Model Id", width: 120 },
	{ name: "MachineName", hidden: true },
	{ name: "Message", title: "Mesaj", }
]

const Logs = ({ snack }) => {

	const lists = useSelector(state => state.screenConfig.lists)

	const { t } = useTranslation();
	const history = useHistory();
	const [columns, setColumns] = useState([]);
	const [log, setLog] = useState(null);
	const [error, setError] = useState(null);
	const [grid, setGrid] = useState(null);
	const [dataSource] = useState({
		load: api.log.getLogs,
		onError: setError
	});

	const refresh = () => {
		grid && grid.refresh();
	};

	function GetLog(id) {
		api.log.getLog(id).then((status) => {
			if (status.Success) {
				setLog(status.Resource);
			} else {
				snack.show(status);
			}
		});
	}

	function focusedRowChanged(row) {
		if (row)
			GetLog(row.Id);
	}

	useEffect(() => {
		if (error) {
			snack.show(error);
		}
	}, [snack, error]);

	useEffect(() => {
		if (lists) {
			setColumns(() => {
				return defaultColumns.map(c => {
					if (c.autoComplete)
						c.lookup = {
							dataSource: lists[c.autoComplete],
							displayExpr: "caption",
							valueExpr: "value"
						}
					return c
				})
			})
		}
	}, [lists]);

	function renderAvatar() {
		switch (log.Level) {
			case "Info":
				return (
					<Avatar style={{ backgroundColor: blue[500] }}>
						<InfoIcon />
					</Avatar>
				);
			case "Error":
				return (
					<Avatar style={{ backgroundColor: red[500] }}>
						<ErrorIcon />
					</Avatar>
				);
			case "Warning":
				return (
					<Avatar style={{ backgroundColor: orange[500] }}>
						<ErrorIcon />
					</Avatar>
				);
			case "Debug":
				return (
					<Avatar style={{ backgroundColor: green[500] }}>
						<DebugIcon />
					</Avatar>
				);
			default:
				return (
					<Avatar style={{ backgroundColor: orange[500] }}>
						<ErrorIcon />
					</Avatar>
				);
		}
	}

	return (
		<Grid container spacing={2} alignItems="stretch">
			<Grid item xs={12}>
				<Toolbar>
					<Tooltip title={t("Back")}>
						<span>
							<IconButton edge="start" onClick={() => history.goBack()} >
								<ArrowBack />
							</IconButton>
						</span>
					</Tooltip>
					<Box pt={1} pl={1}>
						<Typography variant="h4" gutterBottom>
							<Trans>Logs</Trans>
						</Typography>
					</Box>
				</Toolbar>
			</Grid>

			<Grid item xs={12}  >
				<Paper id="gridPaper" style={{ minHeight: '260px' }}>
					<Box p={1}>
						{columns.length > 0 &&
							<DataTable
								id={"idForLogs"}
								instance={setGrid}
								onFocusedRowChanged={focusedRowChanged}
								onError={setError}
								keyFieldName="Id"
								columns={columns}
								dataSource={dataSource}
								columnResizingMode="nextColumn"
							/>
						}
					</Box>
				</Paper>
			</Grid>

			{log && (
				<Grid item xs={12}>
					<Card>
						<CardHeader
							avatar={renderAvatar()}
							title={<Tr>{log.Message}</Tr>}
							subheader={log.Logged}
							action={
								<Box mt={1} mr={1}>
									<Tooltip title={t("Logger")}>
										<Chip style={{ padding: 8, margin: 4 }} icon={<ComputerIcon />} label={log.MachineName} />
									</Tooltip>
								</Box>
							}
						/>
						{log.Exception && (
							<CardContent>
								<Box pl={1}>
									<TextArea style={{ fontFamily: 'monospace' }} readOnly={true} value={log.Exception} autoResizeEnabled={true} />
								</Box>
							</CardContent>
						)}
						<CardActions>
							<Box pl={2}>
								<Tooltip title="Logger">
									<pre>{log.Logger}</pre>
								</Tooltip>
							</Box>
							<Box>
								<Tooltip title="Call Site">
									<pre>{log.Callsite}</pre>
								</Tooltip>
							</Box>
						</CardActions>
					</Card>
				</Grid>
			)}
		</Grid>
	);
};

export default withSnack(Logs);
