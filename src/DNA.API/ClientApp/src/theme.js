import Loadable from 'react-loadable'
import * as styles from '@material-ui/core/styles';

var Theme;
var ThemeName;

var LoadableDxTheme = Loadable({
	loader: () => {
		let dxCommon = import('devextreme/dist/css/dx.common.css')
		var t;
		console.log("ThemeName", ThemeName)
		if (ThemeName == "dark-blue")
			t = import('./assets/dx.material.dark-blue.css')
		else if (ThemeName == "red")
			t = import('./assets/dx.material.red.css')
		else if (ThemeName == "green")
			t = import('./assets/dx.material.green.css')
		else if (ThemeName == "orange")
			t = import('./assets/dx.material.orange.css')
		else
			t = import('./assets/dx.material.blue.css')
		let index = import('./index.css')
		return t
	},
	loading: () => null
})

const createTheme = (Plugin) => {

	if (Theme)
		return Theme;

	ThemeName = (Plugin && Plugin.Theme && Plugin.Theme.Name) || "blue";

	LoadableDxTheme.preload();

	let primary = '#318CE7'
	let secondary = '#005376'

	if (ThemeName == "dark-blue") {
		primary = '#005376'
		secondary = '#19851f'
	}
	else if (ThemeName == "red") {
		primary = '#8b0000'
		secondary = '#005376'
	}
	else if (ThemeName == "green") {
		primary = '#19851f'
		secondary = '#005376'
	}
	else if (ThemeName == "orange") {
		primary = '#D2691E'
		secondary = '#005376'
	}

	document.querySelector('meta[name="theme-color"]').setAttribute('content', primary);

	//const dark = '#323545'
	//const dark2 = '#272a3a'
	Theme = styles.createMuiTheme({
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


	Theme.overrides = {
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
		},
		// MuiOutlinedInput: {
		// 	input: {
		// 		padding: 12
		// 	}
		// }
	}
	return Theme
};

export default createTheme