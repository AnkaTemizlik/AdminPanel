import React from 'react'
import SvgIcon from '@material-ui/core/SvgIcon'
import { Box } from '@material-ui/core';

export const getIconByName = (name) => {
	switch (name) {
		case "Equals": return Equal;
		case "Contains": return Contain;
		//case	"DoesNotContain":return Equal;
		case "StartsWith": return StartsWith;
		case "EndsWith": return EndsWith;
		case "DoesNotEqual": return NotEqual;
		case "IsLessThan": return LessThan;
		case "IsGreaterThan": return GreaterThan;
		case "IsLessThanOrEqualTo": return LessThanOrEqual;
		case "IsGreaterThanOrEqualTo": return GreaterThanOrEqual;
		case "IsBlank": return Blank;
		case "IsNotBlank": return NotBlank;
		case "IsBetween": return Between;
		default:
			return Equal;
	}
}

export const OperatorIcon = ({ name, ...opt }) => {
	let Icon = getIconByName(name)
	return <Icon {...opt} />
}

export const Equal = (props) => <SvgIcon {...props}>
	<path d="M19,10H5V8H19V10M19,16H5V14H19V16Z" />
</SvgIcon>

export const NotEqual = (props) => <SvgIcon {...props}>
	<path d="M14.08,4.61L15.92,5.4L14.8,8H19V10H13.95L12.23,14H19V16H11.38L9.92,19.4L8.08,18.61L9.2,16H5V14H10.06L11.77,10H5V8H12.63L14.08,4.61Z" />
</SvgIcon>

export const Contain = (props) => <SvgIcon {...props}>
	<path d="M2,3H8V5H4V19H8V21H2V3M7,17V15H9V17H7M11,17V15H13V17H11M15,17V15H17V17H15M22,3V21H16V19H20V5H16V3H22Z" />
</SvgIcon>

export const StartsWith = (props) => <SvgIcon {...props}>
	<path d="M11.14 4L6.43 16H8.36L9.32 13.43H14.67L15.64 16H17.57L12.86 4M12 6.29L14.03 11.71H9.96M4 18V15H2V20H22V18Z" />
</SvgIcon>
export const EndsWith = (props) => <SvgIcon {...props}>
	<path d="M11.14 4L6.43 16H8.36L9.32 13.43H14.67L15.64 16H17.57L12.86 4M12 6.29L14.03 11.71H9.96M20 14V18H2V20H22V14Z" />
</SvgIcon>

export const LessThan = (props) => <SvgIcon {...props}>
	<path d="M18.5,4.14L19.5,5.86L8.97,12L19.5,18.14L18.5,19.86L5,12L18.5,4.14Z" />
</SvgIcon>
export const GreaterThan = (props) => <SvgIcon {...props}>
	<path d="M5.5,4.14L4.5,5.86L15,12L4.5,18.14L5.5,19.86L19,12L5.5,4.14Z" />
</SvgIcon>

export const LessThanOrEqual = (props) => <SvgIcon {...props}>
	<path d="M18.5,2.27L5,10.14L18.5,18L19.5,16.27L8.97,10.14L19.5,4L18.5,2.27M5,20V22H20V20H5Z" />
</SvgIcon>

export const GreaterThanOrEqual = (props) => <SvgIcon {...props}>
	<path d="M6.5,2.27L20,10.14L6.5,18L5.5,16.27L16.03,10.14L5.5,4L6.5,2.27M20,20V22H5V20H20Z" />
</SvgIcon>

export const NotBlank = (props) => <SvgIcon {...props}>
	<path d="M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z" />
</SvgIcon>

export const Blank = (props) => <SvgIcon {...props}>
	<path d="M12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z" />
</SvgIcon>

export const Between = (props) => <SvgIcon {...props}>
	<path d="M3 15H5V19H19V15H21V19C21 20.1 20.1 21 19 21H5C3.9 21 3 20.1 3 19V15Z" />
</SvgIcon>
