import React from 'react';
import PropTypes from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';
import useScrollTrigger from '@material-ui/core/useScrollTrigger';
import Fab from '@material-ui/core/Fab';
import Zoom from '@material-ui/core/Zoom';
import KeyboardArrowUp from '@material-ui/icons/KeyboardArrowUp'

const useStyles = makeStyles((t) => ({
	root: {
		position: 'fixed',
		bottom: t.spacing(2),
		right: t.spacing(2),
		zIndex: 1
	},
	// fab: {
	//     backgroundColor: secondary,
	//     color: white
	// }
}));

function ScrollTop(props) {
	const { children } = props;
	const classes = useStyles();

	const trigger = useScrollTrigger({
		disableHysteresis: true,
		threshold: 100,
	});

	const handleClick = (event) => {
		const anchor = (event.target.ownerDocument || document).querySelector('#back-to-top-anchor');
		console.write(anchor);

		if (anchor) {
			anchor.scrollIntoView({ behavior: 'smooth', block: 'center' });
		}
	};

	return (
		<Zoom in={trigger}>
			<div onClick={handleClick} role="presentation" className={classes.root}>
				{children}
			</div>
		</Zoom>
	);
}

ScrollTop.propTypes = {
	children: PropTypes.element.isRequired,
};

const ScrollTopComponent = (props) => {
	const classes = useStyles();
	return <ScrollTop {...props}>
		<Fab color="primary" className={classes.fab} size="small">
			<KeyboardArrowUp />
		</Fab>
	</ScrollTop>
}

export default ScrollTopComponent