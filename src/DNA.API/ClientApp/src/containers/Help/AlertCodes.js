import React, { useCallback, useEffect, useState } from "react";
import { useHistory } from "react-router-dom";
import { Box, Divider, Grid, IconButton, Paper, Toolbar, Tooltip, Typography } from "@material-ui/core";
import { ArrowBack } from "@material-ui/icons";
import { Trans, useTranslation } from "../../store/i18next";
import RefreshIcon from "@material-ui/icons/Refresh";
import Container from "../../components/Container";
import withSnack from "../../store/snack";
import api from "../../store/api";

function AlertCodes(props) {
	const { menu, snack } = props;
	let { t } = useTranslation();
	let history = useHistory();
	const [loading, setLoading] = useState(false);
	const [data, setData] = useState([]);
	const [filtered, setFiltered] = useState([]);
	const [search] = useState("");

	const getData = useCallback(() => {
		let url = "/api/help/alert-codes";
		setLoading(true);
		api.actions.run("GET", url).then((status) => {
			setLoading(false);
			if (status.Success) {
				setData(status.Resource);
				setFiltered(status.Resource);
			} else {
				snack.show(status);
			}
		});
	}, [snack]);

	useEffect(() => {
		getData();
	}, [getData]);

	useEffect(() => {
		setFiltered(() => {
			if (search == "") return data;
			else if (data)
				return data.map((g) => {
					//group
					if (g && g.Data && g.Data.length > 0) {
						// row
						const newRows = g.Data.map((r) => {
							if (r.Key && `${r.Key}`.indexOf(search)) return r;
							if (r.Value && r.Value.indexOf(search)) return r;
							if (r.Comment && r.Comment.indexOf(search)) return r;
							return null;
						});

						if (newRows && newRows.length > 0) {
							g.Data = newRows;
							return g;
						}
						return null;
					}
					return null;
				});
			else return [];
		});
	}, [search, data]);

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
								<Trans>{menu.label}</Trans>
							</Typography>
						</Box>
					</Toolbar>
				</Grid>

				<Grid item xs={12}>
					<Toolbar component={Paper}>
						<Tooltip title={t("Refresh")}>
							<span>
								<IconButton onClick={getData} disabled={loading}>
									<RefreshIcon />
								</IconButton>
							</span>
						</Tooltip>
						<Box flexGrow="1"></Box>
					</Toolbar>
				</Grid>

				<Grid item xs={12}>
					{filtered &&
						filtered.map((g, i) => {
							if (!g) return null;
							if (!g.Data) return null;
							if (g.Data.length == 0) return null;

							return (
								<Box p={2} key={i}>
									<Box pt={2}>
										<Typography variant="h5" gutterBottom>
											{t(g.GroupName)}
										</Typography>
									</Box>
									<Divider />
									{g.Data.map((r, j) => {
										return (
											<Box p={1} key={`${i}-${j}`}>
												<Box display="flex" alignItems="center">
													<Typography variant="h6" component="span" style={{ width: 56 }}>
														{r.Key > 0 ? r.Key : ""}
													</Typography>
													<div style={{ color: "gray" }}>
														<pre>{r.Value}</pre>
													</div>
												</Box>
												<Box pl={7}>
													{r.Status && (
														<Typography variant="body2" component="span" gutterBottom>
															{t(r.Status)}
														</Typography>
													)}
													<Typography variant="body1">{t(r.Value)}</Typography>
												</Box>
											</Box>
										);
									})}
								</Box>
							);
						})}
				</Grid>
			</Grid>
		</Container>
	);
}

export default withSnack(AlertCodes);
