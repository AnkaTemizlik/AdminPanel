import React, { useEffect, useState } from 'react';
import classnames from 'classnames';
import { makeStyles } from '@material-ui/core/styles';
import { useSnackbar, SnackbarContent } from 'notistack';
import Collapse from '@material-ui/core/Collapse';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import Button from '@material-ui/core/Button';
import IconButton from '@material-ui/core/IconButton';
import CloseIcon from '@material-ui/icons/Close';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import CheckCircleIcon from '@material-ui/icons/CheckCircle';
import { Alert, AlertTitle } from '@material-ui/lab';
import { Box } from '@material-ui/core';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles(theme => ({
	root: {
		minWidth: '344px !important',
		[theme.breakpoints.up('sm')]: {
			minWidth: '344px !important',
		},
	},
	typography: {
		fontWeight: 'bold',
	},
	actionRoot: {
		padding: '8px 8px 8px 16px',
	},
	icons: {
		marginLeft: 'auto',
	},
	expand: {
		padding: '8px 8px',
		transform: 'rotate(0deg)',
		transition: theme.transitions.create('transform', {
			duration: theme.transitions.duration.shortest,
		}),
	},
	expandOpen: {
		transform: 'rotate(180deg)',
	},
	collapse: {
		padding: 16,
	},
	checkIcon: {
		fontSize: 20,
		color: '#b3b3b3',
		paddingRight: 4,
	},
	button: {
		padding: 0,
		textTransform: 'none',
	},
}));


const SnackMessage = React.forwardRef((props, ref) => {
	const { t } = useTranslation()
	const classes = useStyles();
	const { closeSnackbar } = useSnackbar();
	const [expanded, setExpanded] = useState(false);
	const [data, setData] = useState({});
	const { variant, message } = props

	const handleExpandClick = () => {
		setExpanded(!expanded);
	};

	const handleDismiss = () => {
		closeSnackbar(props.id);
	};

	useEffect(() => {
		let d = typeof message == "object"
			? { ...message, Variant: message.Success ? "success" : "error" }
			: { Message: message, Variant: variant ?? "info" }
		setData(d)
	}, [message, variant])

	return (
		<SnackbarContent ref={ref} className={classes.root}>

			<Alert severity={data.Variant} variant="filled" action={<>
				{data.Details &&
					<IconButton
						aria-label="Show more"
						className={classnames(classes.expand, { [classes.expandOpen]: expanded })}
						onClick={handleExpandClick}
					>
						<ExpandMoreIcon />
					</IconButton>
				}
				<IconButton className={classes.expand} onClick={handleDismiss}>
					<CloseIcon />
				</IconButton>
			</>
			}>

				{/* <Typography variant="subtitle2">{data.Message}</Typography> */}
				{/* <AlertTitle>{message}</AlertTitle> */}
				{data.Comment
					? <>
						<AlertTitle>{t(data.Message)}</AlertTitle>
						<Typography variant="subtitle2">{data.Comment}</Typography>
						{data.Details &&
							<Collapse in={expanded} timeout="auto" unmountOnExit>
								<Box p={1}>
									<Typography variant="subtitle2">{data.Details}</Typography>
								</Box>
							</Collapse>
						}
					</>
					: <Typography variant="subtitle2">{t(data.Message || data.Resource)}</Typography>
				}
			</Alert>
		</SnackbarContent>
	);
});

export default React.memo(SnackMessage);