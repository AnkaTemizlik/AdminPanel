import React, { useEffect } from 'react';
import { Redirect } from 'react-router-dom';
import { connect, useDispatch } from 'react-redux';
import * as actions from '../../store/actions';
import { logout } from '../../store/slices/authSlice';

const Logout = ({ onLogout, onInit }) => {
	const dispatch = useDispatch()
	useEffect(() => {
		dispatch(logout())
		onLogout() //.then(() => onInit());
	}, [dispatch, onLogout])

	return <Redirect to="/" />
}

const mapDispatchToProps = dispatch => {
	return {
		onLogout: () => dispatch(actions.logout())
	};
};

export default connect(null, mapDispatchToProps)(Logout);