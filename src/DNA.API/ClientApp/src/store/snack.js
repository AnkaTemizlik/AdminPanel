import React, { useCallback } from "react";
import { useDispatch, useSelector } from 'react-redux';
import { useSnackbar } from "notistack";
import SnackMessage from "../components/UI/SnackMessage";
import { hideMessage } from '../store/slices/alertsSlice'

const withSnack = (Component) => {
	const WrappedComponent = React.forwardRef((props, ref) => {
		const sb = useSnackbar();
		const dispatch = useDispatch();

		// https://iamhosseindhv.com/notistack/api#mutual
		function show(m, v) {
			if (v == "error")
				console.error("Snackbar", m)
			sb.enqueueSnackbar(null, {
				content: (key) => <SnackMessage id={key} message={m} variant={v} />,
				onClose: () => dispatch(hideMessage()),
				preventDuplicate: true
			});
		}

		return (
			<Component
				{...props}
				ref={ref}
				snack={{
					show: (m) => show(m),
					default: (m) => show(m, "default"),
					info: (m) => show(m, "error"),
					error: (m) => show(m, "error"),
					warning: (m) => show(m, "warning"),
					success: (m) => show(m, "success"),
				}}
			/>
		);
	});

	return WrappedComponent;
};

export default withSnack
