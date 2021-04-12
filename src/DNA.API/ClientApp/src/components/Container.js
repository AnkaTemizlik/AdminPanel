import React, { useEffect, useState } from "react";
import { makeStyles } from "@material-ui/core/styles";
import Box from "@material-ui/core/Box";
import LinearProgress from "@material-ui/core/LinearProgress";

const useStyles = makeStyles(() => ({
	root: {
		marginTop: 24,
		minHeight: 768,
		position: "relative",
	},
	linearProgress: {
		position: "fixed",
		margin: "auto",
		left: 0,
		right: 0,
		top: "64px",
	},
}));

var timer;

const Container = ({ loading, children }) => {
	const classes = useStyles();
	const [active, setActive] = useState(false);

	useEffect(() => {
		if (loading) {
			timer = setTimeout(() => setActive(loading), 200);
		} else {
			setActive(false);
			clearTimeout(timer);
		}
		return () => clearTimeout(timer);
	}, [loading]);

	return (
		<Box m={1} className={classes.root} component="main">
			{active ? <LinearProgress color="secondary" className={classes.linearProgress} /> : null}
			{children}
		</Box>
	);
};

// { loading: state.exam.loading || state.lesson.loading || state.auth.loading }

export default Container;
