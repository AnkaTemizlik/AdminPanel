import React, { useState, useEffect, useCallback } from 'react';
import { fade, makeStyles, withStyles } from '@material-ui/core/styles';
import SearchIcon from '@material-ui/icons/Search';
import { TextField, InputAdornment, OutlinedInput, FormControl, InputLabel, IconButton } from '@material-ui/core';

const useStyles = makeStyles(theme => ({
	root: {
		width: 160,
		'& input': {
			padding: '7px 0',
		},
		"& fieldset": {
			borderColor: theme.palette.primary.light
		}
	},
	adornedStart: {
		paddingLeft: 4
	}
}));

export default function SearchButton({ onChange, value, placeholder, size }) {

	const classes = useStyles()

	const [search, setSearch] = useState(value)

	const set = useCallback((s) => {
		onChange(s)
	}, [onChange])

	useEffect(() => {
		if (search != value)
			if (search && search.length >= 2) {
				var timer = setTimeout(() => set(search), 700)
			}
			else
				set("")
		return () => clearTimeout(timer)
	}, [search, value, set])

	useEffect(() => {
		if (!value)
			setSearch("")
		else
			setSearch(value)
	}, [value])

	return <TextField
		className={classes.root}
		placeholder={placeholder || "Ara.."}
		variant="outlined"
		id="search-input"
		color="primary"
		size={size || "small"}
		value={search}
		onChange={(e) => { setSearch(e.target.value || "") }}
		InputProps={{
			style: { fontSize: '0.8125rem' },
			startAdornment: <InputAdornment>
				<SearchIcon style={{ color: "darkgray" }} />
			</InputAdornment>,
			classes: {
				adornedStart: classes.adornedStart
			}
		}}
	/>

}