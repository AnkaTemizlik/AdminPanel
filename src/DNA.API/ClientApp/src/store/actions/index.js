
export {
	init,
	tryInit,
	auth,
	auth2,
	register,
	confirm,
	recovery,
	changePassword,
	logout,
	setAuthRedirectPath,
	authCheckState,
	saveSettings,
	setCurrentConfig
} from './auth';

export {
	getEntities
} from './entity';

const reset = () => {
	console.write("[reset]")
	return dispatch => {
		dispatch({ type: "RESET" })
	}
}

export { reset }
