import React, { useEffect } from "react";
import { connect, useSelector } from 'react-redux'
// import ReCAPTCHA from "react-google-recaptcha";
import Alert from '@material-ui/lab/Alert';
import { makeStyles, withStyles } from "@material-ui/core"
import { Container, FormControl, Box, Button, Link, IconButton, Typography, Divider, FormGroup, Checkbox, FormControlLabel, Avatar, Grid, Badge, Icon, Tooltip } from "@material-ui/core";
import * as actions from '../../store/actions'
import AccountCircleIcon from "@material-ui/icons/AccountCircleTwoTone";
import FormWrapper from "../../components/UI/FormContainer";
import TextField from "../../components/UI/TextField";
import { Link as RouterLink } from "react-router-dom";
import { useTranslation } from "react-i18next";

const StyledBadge = withStyles((theme) => ({
	root: {

	},
	badge: {
		height: 36,
		width: 36,
		borderRadius: 4,
	}
}))(Badge);

const comment = "";
const development = (process.env && process.env.NODE_ENV === "development")

const Settings = () => {
	const { user, token } = useSelector(state => state.auth)
	const { Plugin } = useSelector(state => state.settings)
	const { t } = useTranslation()
	const [error, setError] = React.useState(null)
	if (!user)
		return null

	return <Container maxWidth="sm" style={{ minHeight: 768 }}>
		<Box pt={6}>
			<Grid container spacing={2}>
				{error &&
					<Grid item xs={12} >
						<Alert variant="outlined" severity="error">
							{error}
						</Alert>
					</Grid>
				}

				<Grid container item xs={12} sm={6} justify="center" >
					<StyledBadge
						overlap="circle"
						anchorOrigin={{
							vertical: 'bottom',
							horizontal: 'right',
						}}
						color="primary"
						// style={{ badge: { width: 32, height: 32 } }}
						badgeContent={
							<Tooltip title={t("Change profile picture")}>
								<span>
									<IconButton color="inherit" disabled>
										<Icon>photo_camera</Icon>
									</IconButton>
								</span>
							</Tooltip>
						}
					>
						<Avatar src={user.PictureUrl} style={{ width: 150, height: 150 }} />
					</StyledBadge>
				</Grid>

				<Grid item xs={12} sm={6}>
					<Box pt={2} pb={2}>
						<Typography variant="h5">{user.FullName}</Typography>
					</Box>

					<Grid container direction="row" alignItems="center">
						<Icon>email</Icon>
						<Typography variant="h6" style={{ paddingLeft: 8 }} gutterBottom>{user.Email}</Typography>
					</Grid>

					<Grid container direction="row" alignItems="center">
						<Icon>work</Icon>
						<Typography style={{ paddingLeft: 8 }} gutterBottom>{user.Role}</Typography>
					</Grid>
				</Grid>

				<Grid item xs={12}>
					<Box pt={4} pb={4}>
						<Divider />
					</Box>
				</Grid>

				{Plugin.AuthSettings.AllowPasswordChanging == true &&
					<Grid container item justify="flex-end">
						<Link to="/auth/passwordRecovery" variant="body1" component={RouterLink}>
							{t('Şifreni sıfırla...')}
						</Link>
					</Grid>
				}

			</Grid>
		</Box>
	</Container>
}

export default Settings;