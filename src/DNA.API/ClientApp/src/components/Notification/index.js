
import React, { useEffect, useState, useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useHistory, useParams, Link } from "react-router-dom";
import Moment from 'react-moment';
import PopupState, { bindTrigger, bindPopover } from 'material-ui-popup-state';
import NotificationsIcon from "@material-ui/icons/Notifications";
import { Popover, Avatar, Badge, Box, IconButton, List, ListItem, ListItemAvatar, ListItemIcon, ListItemSecondaryAction, ListItemText, Tooltip, Typography, Icon, Divider } from "@material-ui/core";
import { Delete } from "@material-ui/icons";
import api from "../../store/api";
import { getNotifications, markAsReadOrUnread } from "../../store/slices/notificationSlice";

const NotificationComponent = (props) => {
	const dispatch = useDispatch();
	const { error, data, total } = useSelector(state => state.notification)
	const history = useHistory();

	const get = useCallback(() => {
		dispatch(getNotifications())
	}, [dispatch])

	const onDelete = (n) => {

	}

	const onMarkAsRead = (id) => {
		dispatch(markAsReadOrUnread({ id }))
		dispatch(getNotifications())
	}

	useEffect(() => {
		setTimeout(get, 3 * 1000)
		setInterval(get, (props.refreshIn || 5) * 60 * 1000)
	}, [get, props.refreshIn]);

	return (
		<PopupState variant="popover" popupId="demo-popup-popover">
			{(popupState) => (
				<div>

					<Tooltip title="Notifications">
						<span>
							<IconButton color="inherit" {...bindTrigger(popupState)}>
								<Badge badgeContent={total} color="error">
									<NotificationsIcon />
								</Badge>
							</IconButton>
						</span>
					</Tooltip>

					<Popover
						disableScrollLock
						{...bindPopover(popupState)}
						anchorOrigin={{
							vertical: 'bottom',
							horizontal: 'right',
						}}
						transformOrigin={{
							vertical: 'top',
							horizontal: 'right',
						}}
					>
						<Box p={1} style={{ width: "360px" }}>
							<List>

								<ListItem>
									<ListItemText primary="Notifications" />
								</ListItem>

								<Divider />

								{data && data.length == 0
									? <Box display="flex" flexDirection="column" style={{ minHeight: 100, width: '100%' }} justifyContent="center" alignItems="center">
										<Icon style={{ fontSize: 64, opacity: "0.2" }}>check</Icon>
										<Typography variant="body2">No data.</Typography>
									</Box>
									: <>
										{data.map((d, i) => {
											return <ListItem key={i} button onClick={(e) => {
												popupState.close(e)
												return d.Url ? history.push(d.Url) : null
											}
											}
												style={{ paddingLeft: 8 }}>
												<ListItemAvatar>
													<Avatar>
														<NotificationsIcon />
													</Avatar>
												</ListItemAvatar>
												{/* <ListItemIcon>
												<NotificationsIcon />
										</ListItemIcon> */}
												<ListItemText
													primary={d.Title}
													secondary={
														<React.Fragment>
															{d.Comment}
															{` — `}
															<Typography
																component="span"
																variant="body2"
																//className={classes.inline}
																color="textPrimary"
															>
																<Moment fromNow>{d.UpdateTime}</Moment>
															</Typography>
														</React.Fragment>
													}
												/>
												<ListItemSecondaryAction>
													<IconButton edge="end" aria-label="Mark as read"
														onClick={e => onMarkAsRead(d.Id)}>
														<Icon>mark_email_read</Icon>
													</IconButton>
												</ListItemSecondaryAction>

												{d.UserId > 0 &&
													<ListItemSecondaryAction>
														<IconButton edge="end" aria-label="delete" onClick={(e) => {
															popupState.close(e);
															onDelete(d)
														}}>
															<Delete />
														</IconButton>
													</ListItemSecondaryAction>
												}
											</ListItem>
										})}

									</>
								}

								<Divider />

								<ListItem button onClick={(e) => {
									popupState.close(e);
									history.push("/panel/screen/notification")
								}}>
									<ListItemText
										primary="Show all">
									</ListItemText>
									<ListItemSecondaryAction>
										<Icon>redo</Icon>
									</ListItemSecondaryAction>
								</ListItem>
							</List>
						</Box>
					</Popover>
				</div>
			)}
		</PopupState>
	)
}

export default NotificationComponent;
