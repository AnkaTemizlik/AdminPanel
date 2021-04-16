import React, { useEffect, useState, useCallback } from "react";
import { Link, useRouteMatch, useHistory } from "react-router-dom";
import { Grid, Toolbar, Box, IconButton, Paper, Typography } from "@material-ui/core";
import { Tooltip } from "@material-ui/core";
import RefreshIcon from "@material-ui/icons/Refresh";
import AddIcon from "@material-ui/icons/PersonAdd";
import EditIcon from "@material-ui/icons/Edit";
import DeleteIcon from "@material-ui/icons/Delete";
import api from "../../store/api";
import Container from "../../components/Container";
import { Trans, useTranslation } from "../../store/i18next";
import { ArrowBack } from "@material-ui/icons";
import withSnack from "../../store/snack";
import DataTable from "../../components/UI/Table/DataTable";

const Users = (props) => {
	const { snack } = props;
	let { url } = useRouteMatch();
	let history = useHistory();
	let { t } = useTranslation();

	const [columns] = useState(
		[
			{ name: "Id", hidden: true },
			{ name: "FullName", width: 220 },
			{ name: "Email", width: 220 },
			{ name: "Role", width: 90 },
			{ name: "EmailConfirmed", dataType: "boolean", width: 120 },
			{ name: "PasswordCreated", dataType: "boolean", width: 120 },
		]
	);

	const [users, setUsers] = useState([]);
	const [user, setUser] = useState(null);
	const [loading, setLoading] = useState(false);
	const [size, setSize] = useState(10);
	const [page, setPage] = useState(0);
	const [total, setTotal] = useState(0);
	const [search, setSearch] = useState("");

	var getData = useCallback(
		(p, s, t) => {
			setLoading(true);
			api.auth.getUsers({ page: p, size: s, filter: { search: t } }).then((status) => {
				setLoading(false);
				if (status.Success) {
					let items = status.Resource.Items.map((u) => {
						u.PasswordCreated = !u.IsInitialPassword;
						return u;
					});
					setUsers(items);
					setTotal(status.Resource.TotalItems);
				} else {
					snack.show(status);
				}
			});
		},
		[snack]
	);

	useEffect(() => {
		setPage(0);
	}, [search]);

	useEffect(() => {
		getData(page, size, search);
	}, [getData, size, page, search]);

	const refresh = () => {
		getData(page, size, search);
	};

	function focusedRowChanged(row) {
		setUser(row);
	}

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
						<Box pt={1} pl={1}>
							<Typography variant="h4" gutterBottom>
								<Trans>Users</Trans>
							</Typography>
						</Box>
					</Toolbar>
				</Grid>
				<Grid item xs={12}>
					<Toolbar component={Paper}>
						<Tooltip title={t("Refresh")}>
							<span>
								<IconButton onClick={refresh} disabled={loading}>
									<RefreshIcon />
								</IconButton>
							</span>
						</Tooltip>
						<Box flexGrow="1"></Box>
						<Tooltip title={t("Add New")}>
							<span>
								<IconButton component={Link} to={`${url}/new`} disabled={loading}>
									<AddIcon />
								</IconButton>
							</span>
						</Tooltip>
						<Tooltip title={t("Edit")}>
							<span>
								<IconButton edge="start" component={Link} to={`${url}/${user ? user.Id : 0}`} disabled={loading || !user}>
									<EditIcon />
								</IconButton>
							</span>
						</Tooltip>
						<Tooltip title={t("Delete")}>
							<span>
								<IconButton edge="start" onClick={refresh} disabled={loading || !user}>
									<DeleteIcon />
								</IconButton>
							</span>
						</Tooltip>
					</Toolbar>
				</Grid>
				<Grid item xs={12}>
					<Paper>
						<Box p={1}>
							<DataTable
								id={"idForUsers"}
								remoteOperations={false}
								dataSource={users || []}
								columns={columns}
								keyFieldName="Id"
								allowFilter={false}
								allowPaging={false}
								onFocusedRowChanged={focusedRowChanged}
							/>
						</Box>
					</Paper>
				</Grid>
			</Grid>
		</Container>
	);
};

export default withSnack(Users);
