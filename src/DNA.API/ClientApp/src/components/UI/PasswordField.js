import React from 'react'
import { OutlinedInput, IconButton, InputAdornment, Tooltip, TextField } from '@material-ui/core'
import { Visibility, VisibilityOff } from '@material-ui/icons';
import LockIcon from '@material-ui/icons/Lock';

const PasswordField = (props, ref) => {

	const { options, inputLabelProps, ...rest } = props
	const [values, setValues] = React.useState({
		amount: '',
		password: '',
		weight: '',
		weightRange: '',
		showPassword: false,
	});
	const handleClickShowPassword = () => {
		setValues({ ...values, showPassword: !values.showPassword });
	};

	const handleMouseDownPassword = (event) => {
		event.preventDefault();
	};

	function generatePassword(passwordLength) {
		var numberChars = "0123456789";
		var upperChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";
		var lowerChars = "abcdefghikmnopqrstuvwxyz";
		var allChars = numberChars + upperChars + lowerChars;
		var randPasswordArray = Array(passwordLength);
		randPasswordArray[0] = numberChars;
		randPasswordArray[1] = upperChars;
		randPasswordArray[2] = lowerChars;
		randPasswordArray = randPasswordArray.fill(allChars, 3);
		var newArr = randPasswordArray.map(function (x) {
			var val = x[Math.floor(Math.random() * x.length)]
			return val
		});
		return shuffleArray(newArr).join('');
	}

	function shuffleArray(array) {
		for (var i = array.length - 1; i > 0; i--) {
			var j = Math.floor(Math.random() * (i + 1));
			var temp = array[i];
			array[i] = array[j];
			array[j] = temp;
		}
		return array;
	}

	return <TextField
		{...rest}
		fullWidth
		margin="normal"
		type={values.showPassword ? 'text' : 'password'}
		autoComplete="new-password"
		InputLabelProps={inputLabelProps}
		InputProps={{
			endAdornment: <InputAdornment position="end">
				{options && options.generatePassword
					? <Tooltip title="Generate new password">
						<IconButton
							onClick={() => {
								options.onPasswordGenerated(generatePassword(8))
							}}
							onMouseDown={handleMouseDownPassword}
							edge="end"
						>
							<LockIcon />
						</IconButton>
					</Tooltip>
					: null
				}
				<IconButton
					onClick={handleClickShowPassword}
					onMouseDown={handleMouseDownPassword}
					edge="end"
				>
					{values.showPassword ? <Visibility /> : <VisibilityOff />}
				</IconButton>
			</InputAdornment>
		}}
	/>
}

export default PasswordField