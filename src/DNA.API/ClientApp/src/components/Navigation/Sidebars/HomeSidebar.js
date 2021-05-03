import React from 'react'
import PropTypes from 'prop-types'
import * as Treasury from "@mui-treasury/layout";
import { SocialMenu } from '../Menus';
import { Divider, Box } from '@material-ui/core';
import SidebarMenu from '../../UI/Menu/SidebarMenu';
import AuthInfo from '../AuthInfo';
import BrandLogo from '../BrandLogo';

const HomeSidebar = (props) => {

	return <Treasury.SecondarySidebar>
		<AuthInfo />

		<Divider />

		<SidebarMenu {...props} menu={props.menus.user} />
		<Divider />

		<SidebarMenu {...props} menu={props.menus.home} />
		<Divider />

		<Box display="flex" justifyContent="center" mb={8}>
			<SocialMenu menu={props.menus.social} />
		</Box>

		<BrandLogo width={100} />

	</Treasury.SecondarySidebar>
}

HomeSidebar.propTypes = {
	setOpened: PropTypes.func.isRequired,
};
export default HomeSidebar