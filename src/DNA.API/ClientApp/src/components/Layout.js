import React from 'react'
import { makeStyles } from "@material-ui/core/styles";
import { Root as TreasuryRoot } from "@mui-treasury/layout";
import createTheme from '../theme'

const useStyles = makeStyles(() => ({
	footerPrimaryBorder: {
		paddingTop: "48px",
		marginTop: "48px",
		borderTop: props => "1px solid " + props.theme.palette.primary.dark
	},
	footerSecondaryBorder: {
		paddingTop: "48px",
		marginTop: "48px",
		borderTop: props => "1px solid " + props.theme.palette.secondary.dark
	}
}));

const Layout = (props) => {
	const theme = createTheme()
	const classes = useStyles({ ...props, theme });

	return <TreasuryRoot theme={theme} config={props.config}>

		{(args) => {

			const childProps = {
				...args,
				footerStyles: {
					primaryBorder: classes.footerPrimaryBorder,
					secondaryBorder: classes.footerSecondaryBorder
				}
			}
			return typeof props.children === 'function'
				? props.children(childProps)
				: props.children
		}}
	</TreasuryRoot>
}

export default Layout

