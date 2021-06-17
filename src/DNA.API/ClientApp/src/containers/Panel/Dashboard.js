import React, { useEffect, useState, useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useHistory } from "react-router-dom";
import { makeStyles } from "@material-ui/core/styles";
import { Grid, Box, Typography, Card, CardActionArea, CardHeader, CardMedia, CardContent, Avatar, Icon } from "@material-ui/core";
import { Tooltip } from "@material-ui/core";
import RefreshIcon from "@material-ui/icons/Refresh";
import * as colors from "@material-ui/core/colors"; // green, red, orange, blue ...
import DashboardIcon from "@material-ui/icons/Dashboard";
import api from "../../store/api";
import Container from "../../components/Container";
import DxChart from "../../components/UI/Charts/DxChart";
import CollapsibleCard from "../../components/UI/CollapsibleCard";
import { Trans, Tr, useTranslation } from "../../store/i18next";
import withSnack from "../../store/snack";
import Moment from "react-moment";
import { Link } from "react-router-dom";
import TileView, { Item } from 'devextreme-react/tile-view';
import Iconify from "../../components/UI/Icons/Iconify";

const useStyles = makeStyles((theme) => ({
	avatar: {
		color: theme.palette.primary.main,
		backgroundColor: "unset",
	},
	tileItemIcon: {
		color: theme.palette.primary.main,
		marginBottom: 20
	},
	tileItem: {
		cursor: "pointer",
		display: "flex",
		flexDirection: "column",
		alignItems: "center",
		justifyContent: "center",
		height: "100%"
	}
}));

const Dashboard = ({ snack }) => {
	const classes = useStyles();
	const { t } = useTranslation();
	const history = useHistory();
	const [loading, setLoading] = useState(false);
	const [cardsData, setCardsData] = useState({});
	const [cardNames, setCardNames] = useState([]);
	const { cards, lists } = useSelector((state) => state.screenConfig)
	console.info("Dashboard", cards)

	var getCards = useCallback(
		(names) => {
			setLoading(true);
			api.entity.getCards(names).then((status) => {
				setLoading(false);
				console.info("Dashboard", status.Resource)
				if (status.Success) {
					names.forEach((name) => {
						let config = cards[name];
						let newRows = [];
						let cardData = status.Resource[name];
						cardData.forEach((row) => {
							row[config.argumentField + 'Desc'] = row[config.argumentField]
							if (config.autoComplete) {
								let newVal = lists[config.autoComplete].filter((l) => l.id === row[config.argumentField]);
								if (newVal[0]) {
									if (newVal[0].value) {
										row[config.argumentField + 'Desc'] = t(newVal[0].value);
									}
								}
							}
							newRows.push(row);
						});
						status.Resource[name] = newRows;
					});
					setCardsData(status.Resource);
				} else
					snack.show(status);
			});
		},
		[cards, snack, lists, t]
	);

	useEffect(() => {
		if (cardNames && cardNames.length > 0) getCards(cardNames);
	}, [cardNames, getCards]);

	useEffect(() => {
		if (cards) setCardNames(cards.Names);
	}, [cards]);

	function getData(name) {
		var data = cardsData && cardsData[name];
		return data || [];
	}

	return (
		<Container loading={loading}>
			<Grid container spacing={2} >

				<Grid item xs={12} style={{ margin: 4 }}>
					<TileView
						style={{ marginLeft: -16 }}
						direction="horizontal"
						itemMargin={16}
						width='100%'
						height={190}
						baseItemWidth={160}
						baseItemHeight={160}
					>
						{cardNames && cardNames.map((cardName, i) => {
							let c = cards[cardName];
							if (!c || c.type != "link")
								return null

							return <Item
								onClick={() => history.push(c.route)}
								key={i}
							>
								<div className={classes.tileItem}>
									<Iconify className={classes.tileItemIcon} icon={`${c.icon}`} fontSize={56} />
									<div style={{ display: "flex" }}>
										{c.actionIcon && <span><Icon>{c.actionIcon}</Icon></span>}
										<Typography>
											{t(c.title)}
										</Typography>
									</div>
								</div>

							</Item>
						})}
					</TileView>


				</Grid>

				{cardNames && cardNames.map((cardName, i) => {
					let c = cards[cardName];
					if (!c || c.type == "link")
						return null
					// eslint-disable-next-line no-new-func
					var customFunction = c.collection && c.collection.function ? new Function(c.collection.function)() : () => { };
					let data = getData(cardName);
					return (
						<Grid item xs={12} md={6} xl={4} key={i}>
							<CollapsibleCard title={t(c.title)} header="h5" exp={true}>
								{data.length === 0 ? (
									<Grid container style={{ height: 300 }} justify="center" alignItems="center">
										<Typography>{loading ? t("Loading") : t(c.noDataText)}</Typography>
									</Grid>
								) : c.series ? (
									<DxChart
										rotated={c.rotated != false}
										data={data}
										series={c.series}
										argumentField={c.argumentField + 'Desc'}
									/>
								) : (
									<>
										<Grid container spacing={2}>
											{c.collection &&
												data.map((r, i) => {
													let colletionCustomData = customFunction(r);
													return (
														<Grid item xs={12} sm={6} key={i}>
															<Box mt={1} mb={1}>
																<Card>
																	<CardActionArea component={Link} to={`${c.route}`}>
																		<CardHeader
																			avatar={
																				<Avatar className={classes.avatar}>
																					<Icon>{c.icon ?? "picture_in_picture"}</Icon>
																				</Avatar>
																			}
																			title={t(r[c.collection.title])}
																			subheader={
																				<Tooltip title={t(c.collection.date)}>
																					<span>
																						<Moment format={"LLL"}>{new Date(r[c.collection.date])}</Moment>
																					</span>
																				</Tooltip>
																			}
																		/>
																		<CardMedia>
																			<Box display="flex" alignItems="center" alignContent="center" justifyContent="center">
																				<Icon style={{ fontSize: 80, color: colors[colletionCustomData.color][500] }}>
																					{colletionCustomData.icon}
																				</Icon>
																			</Box>
																		</CardMedia>
																		<CardContent>
																			<Box display="flex" justifyContent="center">
																				<Typography gutterBottom variant="body1">
																					<Trans>{r[c.argumentField + 'Desc']}</Trans>
																				</Typography>
																			</Box>
																		</CardContent>
																	</CardActionArea>
																	{/* <CardActions>
																			<Button size="small" color="primary" component={Link} to={`${path}`}>
																				<Trans>Learn More</Trans>
																			</Button>
																		</CardActions> */}
																</Card>
															</Box>
														</Grid>
													);
												})}
										</Grid>
									</>
								)}
							</CollapsibleCard>
						</Grid>
					);
				})}

				<Grid item xs={12} style={{ minHeight: 300 }}>
					<Grid container justify="center" alignItems="center">
						<DashboardIcon style={{ fontSize: 240, opacity: "0.1" }} />
					</Grid>
				</Grid>
			</Grid>
		</Container>
	);
};


export default withSnack(React.memo(Dashboard));
