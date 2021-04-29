import React, { useEffect, useState, useCallback } from "react";
import { useSelector } from "react-redux";
import { useHistory, useParams } from "react-router-dom";
import { makeStyles } from "@material-ui/core/styles";
import { Grid, Toolbar, Box, IconButton, Paper, Typography, FormControl, InputLabel, OutlinedInput, Select, MenuItem } from "@material-ui/core";
import { Tooltip } from "@material-ui/core";
import { ArrowBack } from "@material-ui/icons";
import SendIcon from "@material-ui/icons/Send";
import SaveIcon from "@material-ui/icons/Save";
import api from "../../store/api";
import Container from "../../components/Container";
import Alert from "@material-ui/lab/Alert";
import { Trans, useTranslation } from "../../store/i18next";
import withSnack from "../../store/snack";

const useStyles = makeStyles((theme) => ({
	root: {
		"& > *": {
			marginBottom: theme.spacing(2),
		},
	},
}));

const UserEdit = (props) => {
	const { AppId } = useSelector((state) => state.settings)
	const { snack, me } = props;
	const { t } = useTranslation();
	const classes = useStyles();
	//const { path, url } = useRouteMatch();
	const history = useHistory();
	const { id } = useParams();
	const [user, setUser] = useState({ Role: "", FullName: "", Email: "" });
	const [loading, setLoading] = useState(false);
	const [email, setEmail] = useState(null);
	const [isFormValid, setIsFormValid] = useState(false);

	const [userForm, setForm] = useState({
		FullName: {
			value: "",
			validation: {
				required: true,
				minLength: 5,
				isNumeric: false,
			},
			valid: false,
			touched: false,
		},
		Email: {
			value: "",
			validation: {
				required: true,
				isEmail: true,
			},
			valid: false,
			touched: false,
		},
		Role: {
			value: "",
			validation: {
				required: true,
			},
			valid: false,
			touched: false,
		},
		// password: {
		// 	value: '',
		// 	validation: {
		// 		required: false,
		// 		isPassword: true
		// 	},
		// 	valid: false,
		// 	touched: false
		// }
	});

	var getUser = useCallback(
		(i) => {
			setLoading(true);
			return api.auth.getUser(i).then((status) => {
				setLoading(false);
				if (status.Success) {
					setUser(status.Resource);
					return Promise.resolve(status.Resource);
				} else {
					snack.show(status);
				}
			});
		},
		[snack]
	);

	const sendConfirmEmail = () => {
		setLoading(true);
		api.actions.run("POST", `api/auth/users/${user.Id}/send-confirmation`, null).then((status) => {
			setLoading(false);
			if (status.Success) {
				snack.success(t("Sent successfully"));
				setUser(status.Resource);
			} else {
				snack.show(status);
			}
		});
	};

	const save = () => {
		setLoading(true);
		api.auth.saveUser(user, AppId).then((status) => {
			setLoading(false);
			if (status.Success) {
				snack.success(t("Saved successfully"));
				if (!(id > 0)) {
					history.replace("/panel/users/" + status.Resource.Id);
				} else {
					setUser(status.Resource);
					setEmail(status.Resource.Email);
					setIsFormValid(false);
				}
			} else {
				snack.show(status);
			}
		});
	};

	const checkValidity = useCallback((value, rules) => {
		let ok = true;

		if (!rules) {
			return true;
		}

		if (rules.required) {
			ok = value.trim() !== "" && ok;
		}

		if (rules.minLength) {
			ok = value.length >= rules.minLength && ok;
		}

		if (rules.maxLength) {
			ok = value.length <= rules.maxLength && ok;
		}

		// if (cp === true) {
		// 	if (rules.isPassword) {
		// 		const pattern = /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{6,32}$/;
		// 		ok = pattern.test(value) && ok
		// 	}
		// }

		if (rules.isEmail) {
			const pattern = /^[^<>\s@]+(@[^<>\s@]+(\.[^<>\s@]+)+)$/;
			ok = pattern.test(value) && ok;
		}

		if (rules.isNumeric) {
			const pattern = /^\d+$/;
			ok = pattern.test(value) && ok;
		}

		return ok;
	}, []);

	const isInvalid = (elm) => {
		return elm.touched && !elm.valid;
	};

	const inputChangedHandler = (val, inputIdentifier) => {
		const f = {
			...userForm,
		};

		const updatedFormElement = {
			...f[inputIdentifier],
		};
		updatedFormElement.value = val ?? "";
		updatedFormElement.valid = checkValidity(updatedFormElement.value, updatedFormElement.validation);
		updatedFormElement.touched = true;
		f[inputIdentifier] = updatedFormElement;

		let formIsValid = true;
		for (let inputIdentifier in f) {
			formIsValid = f[inputIdentifier].valid && formIsValid;
		}

		setIsFormValid(formIsValid);
		setForm(f);
	};

	const setFormElementStates = useCallback(
		(u) => {
			if (u && u.Id > 0) {
				setForm((f) => {
					var v = { ...f };
					for (let inputIdentifier in v) {
						v[inputIdentifier].value = u[inputIdentifier] ?? "";
						v[inputIdentifier].valid = checkValidity(v[inputIdentifier].value, v[inputIdentifier].validation);
					}
					return v;
				});
			}
		},
		[checkValidity]
	);

	useEffect(() => {
		if (id > 0)
			getUser(id).then((x) => {
				if (x) setFormElementStates(x);
			});
	}, [id, getUser, setFormElementStates]);

	useEffect(() => {
		if (!email || !(id > 0)) setEmail(user.Email);
	}, [email, id, user]);

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
							{id > 0 ? (
								<Typography variant="h4" component="span" gutterBottom>
									<Trans>Edit User</Trans>
								</Typography>
							) : (
								<Typography variant="h4" gutterBottom>
									<Trans>New User</Trans>
								</Typography>
							)}
						</Box>
					</Toolbar>
				</Grid>
				<Grid item xs={12}>
					<Toolbar component={Paper}>
						<Box flexGrow="1"></Box>

						<Typography variant="body2" component="span">
							{email}
						</Typography>

						<Tooltip title={t("Save")}>
							<span>
								<IconButton onClick={save} disabled={loading || !isFormValid}>
									<SaveIcon />
								</IconButton>
							</span>
						</Tooltip>

						{user && (
							<Tooltip title={t("Send confirm to {{Email}}", { Email: email })}>
								<span>
									<IconButton onClick={sendConfirmEmail} disabled={loading || !(user.Id > 0) || me.Id == user.Id || user.EmailConfirmed}>
										<SendIcon />
									</IconButton>
								</span>
							</Tooltip>
						)}
					</Toolbar>
				</Grid>
				<Grid item xs={12}>
					{((id > 0 && user.Id) || id == undefined) && (
						<Paper>
							{me.Id != user.Id && (
								<Box p={3}>
									{id > 0 ? (
										<Alert severity="info">{t("When the e-mail address is changed, a message is sent for confirmation to '{{Email}}'", user)}</Alert>
									) : (
										<Alert severity="info">{t("A message is sent to new users for email address confirmation to '{{Email}}'", user)}</Alert>
									)}
								</Box>
							)}
							<Grid container spacing={2} justify="center">
								<Grid item xs={12} sm={6}>
									<Box p={2}>
										<form noValidate autoComplete="off" className={classes.root}>
											<FormControl variant="outlined" fullWidth error={isInvalid(userForm.FullName)}>
												<InputLabel htmlFor="name">
													<Trans>FullName</Trans>
												</InputLabel>
												<OutlinedInput
													id="name"
													label={t("FullName")}
													defaultValue={user.FullName}
													required={userForm.FullName.required}
													onChange={(e) => {
														inputChangedHandler(e.target.value, "FullName");
														setUser({ ...user, FullName: e.target.value });
													}}
												/>
											</FormControl>

											<FormControl variant="outlined" fullWidth error={isInvalid(userForm.Email)} disabled={loading || me.Id == user.Id}>
												<InputLabel htmlFor="email">
													<Trans>Email</Trans>
												</InputLabel>
												<OutlinedInput
													id="email"
													label={t("Email")}
													defaultValue={user.Email}
													required={userForm.Email.required}
													onChange={(e) => {
														inputChangedHandler(e.target.value, "Email");
														setUser({ ...user, Email: e.target.value });
													}}
												/>
											</FormControl>

											<FormControl variant="outlined" fullWidth error={isInvalid(userForm.Role)} disabled={loading || me.Id == user.Id}>
												<InputLabel id="role">
													<Trans>Role</Trans>
												</InputLabel>
												<Select
													id="role-select"
													label={t("Role")}
													labelId="role"
													value={user.Role}
													onChange={(e) => {
														inputChangedHandler(e.target.value, "Role");
														setUser({ ...user, Role: e.target.value });
													}}
													required={userForm.Role.required}
												>
													<MenuItem value="">-</MenuItem>
													{me.Roles.indexOf("Admin") > -1 && <MenuItem value="Admin">Admin</MenuItem>}
													<MenuItem value="Reader">Reader</MenuItem>
													<MenuItem value="Writer">Writer</MenuItem>
												</Select>
											</FormControl>
										</form>
									</Box>
								</Grid>
							</Grid>
						</Paper>
					)}
				</Grid>
			</Grid>
		</Container>
	);
};

export default withSnack(UserEdit);
