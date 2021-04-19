import React from "react";
import { useSelector } from 'react-redux'
import PropTypes from 'prop-types';
import Box from '@material-ui/core/Box';
import logo from '../assets/logo6432.png';

const BrandImage = ({ dark, size, className }) => {
	const Plugin = useSelector(state => state.settings.Plugin)
	//const color = dark ? '#323545' : '#ffaa00'
	// const rate = viewBox.h / viewBox.w;
	// const width = size ? size.width : viewBox.w
	// const height = (width * rate)

	return <Box p={1}>
		<img src={(Plugin && Plugin.CompanyLogo) || logo} alt="" width={size ? size.width : undefined} />
	</Box>
}

BrandImage.propTypes = {
	size: PropTypes.exact({
		width: PropTypes.number.isRequired
	}).isRequired
};

export default BrandImage