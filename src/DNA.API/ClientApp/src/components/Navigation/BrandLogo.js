import React from 'react'
import { Link } from 'react-router-dom'
import { makeStyles } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import BrandImage from '../../constants/BrandImage';
import { IconButton } from '@material-ui/core';

const useStyles = makeStyles((theme) => ({
	title: {
		padding: "6px 2px"
	},
	logo: {
		lineHeight: 0,
		marginRight: theme.spacing(1),
		position: "absolute",
		left: theme.spacing(1),
		display: "block"
	},
	brand: {
		minWidth: props => props.hideIcon ? "80px" : "110px",
		//color: props => props.dark ? theme.palette.common.black : theme.palette.primary.main,
		textTransform: 'none',
		textDecoration: 'none',
		fontWeight: "bolder",
		fontSize: props => props.hideIcon ? "16px" : "20px",
		paddingLeft: theme.spacing(1),
		paddingRight: theme.spacing(1)
	}
}))

const BrandLogo = (props, ref) => {
	const { dark } = props
	const classes = useStyles(props);
	const icon = <BrandImage dark={dark} size={{ width: props.width || 160 }} />
	const text = null

	if (props.hideIcon) {
		return <Button className={classes.title} component={Link} to="/" color="inherit" >
			{text}
		</Button>
	}

	if (props.hideText) {
		return <IconButton edge="start" component={Link} to="/">
			{icon}
		</IconButton>
	}

	return <Button className={classes.title} component={Link} to="/" >
		{icon}
		{text}
	</Button>
}

export default BrandLogo