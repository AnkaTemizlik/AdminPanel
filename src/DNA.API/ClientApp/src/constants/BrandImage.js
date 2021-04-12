import React from "react";
import PropTypes from 'prop-types';
import Box from '@material-ui/core/Box';
import logo from '../assets/logo6432.png';

const BrandImage = ({ dark, size, className }) => {
	//const color = dark ? '#323545' : '#ffaa00'
	// const rate = viewBox.h / viewBox.w;
	// const width = size ? size.width : viewBox.w
	// const height = (width * rate)

	return <Box p={1}>
		<img src={logo} alt="" width={size ? size.width : undefined} />
	</Box>
}

BrandImage.propTypes = {
	size: PropTypes.exact({
		width: PropTypes.number.isRequired
	}).isRequired
};

export default BrandImage