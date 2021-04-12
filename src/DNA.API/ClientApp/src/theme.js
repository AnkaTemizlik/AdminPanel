
import * as styles from '@material-ui/core/styles';
import Plugin from './plugins'

const createTheme = (pri, sec) => {
	const pluginPrimary = Plugin && Plugin.Theme && Plugin.Theme.colors && Plugin.Theme.colors.primary;
	const pluginSecondary = Plugin && Plugin.Theme && Plugin.Theme.colors && Plugin.Theme.colors.secondary;
	//const white = '#fff'
	const primary = pri || pluginPrimary || '#1e88e5'
	const secondary = sec || pluginSecondary || '#f50057'

	//const dark = '#323545'
	//const dark2 = '#272a3a'
	const theme = styles.createMuiTheme({
		typography: {
			fontFamily: ["Quicksand", "sans-serif"],
			fontSize: 12,
			// h1: {
			//     color: primary,
			//     fontSize: "3.6rem",
			//     // marginTop: "2.5rem",
			//     // marginBottom: "1rem"
			// },
			// h2: {
			//     color: primary,
			//     fontSize: "3rem",
			//     // marginTop: "2.1rem"
			// },
			// h3: {
			//     color: primary,
			//     fontSize: "2.4rem",
			//     // marginTop: "1.7rem"
			// },
			// h4: {
			//     color: primary,
			//     fontSize: "1.8rem",
			//     // marginTop: "1.5rem"
			// },
			// h5: {
			//     color: primary,
			//     // marginTop: "1.2rem"
			// },
			// h6: {
			//     color: primary,
			//     // marginTop: "1.0rem"
			// },
		},
		palette: {
			//type: 'dark',
			// common: {
			//     black: dark,
			//     white: white
			// },
			primary: {
				main: primary,
				//contrastText: dark
			},
			secondary: {
				main: secondary,
				//contrastText: white
			},
			// background: {
			//     paper: dark2,
			//     default: dark
			// },
			//action: { selected: secondary }
		},

	})

	// theme.palette.success = theme.palette.augmentColor({
	//     main: "#689f38"
	// });

	// const isSuccess = style => props =>
	//     props.color === "success" && props.variant === "contained" ? style : {};


	theme.overrides = {
		//     MuiListItem: {
		//         root: {
		//             '&.Mui-selected': {
		//                 backgroundColor: '#7d4709',
		//                 color: '#fff',
		//                 '& svg': {
		//                     color: '#fff',
		//                 },
		//                 '&:hover': {
		//                     backgroundColor: '#95550b',
		//                 },
		//             },
		//         },
		//     },
		// MuiButton: {
		//     root: {
		//         color: isSuccess(theme.palette.success.contrastText),
		//         backgroundColor: isSuccess(theme.palette.success.main),
		//         "&:hover": {
		//             backgroundColor: isSuccess(theme.palette.success.dark)
		//         }
		//     }
		// },
		MuiSnackbarContent: {
			root: {
				//color: white
			}
		},
		MuiListItemIcon: {
			root: {
				minWidth: 32,
			},
		},
		MuiIcon: {
			root: {
				//color: 'rgba(0, 0, 0, 0.75)',
			},
		},
		MuiLink: {
			root: {
				//color: primary,
			}
		},
		MuiDivider: {
			root: {
				marginBottom: 8,
				marginTop: 8,
			}
		},
		MuiToolbar: {
			gutters: {
				paddingLeft: 12,
				paddingRight: 12,
				'@media (min-width:600px)': {
					paddingLeft: 12,
					paddingRight: 12,
				}
			}
		},
		MuiTab: {
			root: {
				textTransform: 'none'
			}
		},
		MuiButton: {
			root: {
				textTransform: 'none'
			}
		},
		MuiListItem: {
			root: {
				paddingTop: "4px",
				paddingBottom: "4px",
			},
			dense: {
				paddingTop: "0px",
				paddingBottom: "0px",
			}
		},
		MuiListItemText: {
			multiline: {
				marginTop: 2,
				marginBottom: 2
			}
		},
		EditColumn: {
			cell: {
				paddingLeft: 0,
				textAlign: "left"
			},
			headingCell: {
				paddingLeft: 0,
				textAlign: "left"
			}
		},
		EditCell: {
			cell: {
				padding: "2px 4px 4px 4px"
			}
		},
		MuiTableCell: {
			root: {
				padding: "8px"
			},
			sizeSmall: {
				padding: "4px"
			}
		},
		MuiToggleButton: {
			sizeSmall: {
				padding: "4px 8px 4px 8px"
			}
		},
		MuiFormControl: {
			marginNormal: {
				marginTop: 8,
				marginBottom: 8
			}
		}
	}
	return theme
};

console.info("[theme]", createTheme())

export default createTheme