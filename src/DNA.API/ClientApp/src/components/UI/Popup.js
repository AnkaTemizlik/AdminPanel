import React from "react";
import { useTranslation } from "../../store/i18next";
import { Popup, ToolbarItem } from 'devextreme-react/popup';
import { Button } from 'devextreme-react/button';
import Box, { Item } from 'devextreme-react/box';
import { isNotEmpty } from "../../store/utils";
import ResponsiveBox, { Row, Col, Item as ResponsiveItem, Location } from 'devextreme-react/responsive-box';
import ModalComponent from "./Modal";

const PopupComponent = ({ children, visible, toolbarItems, title, onClose, ok, cancel, okText, cancelText, params, loading = false, ...rest }) => {
	const { t } = useTranslation();
	function screen(width) {
		return (width < 700) ? 'sm' : 'lg';
	}
	return <Popup
		visible={visible}
		showTitle={isNotEmpty(title)}
		title={t(title)}
		onHiding={() => onClose({ ...params, open: false })}
		dragEnabled={true}
		closeOnOutsideClick={true}
		{...rest}
	>
		{toolbarItems && toolbarItems.map((item, i) => {
			return <ToolbarItem key={i} {...item} />
		})}

		<ResponsiveBox
			singleColumnScreen="xs"
			screenByWidth={screen}>

			<Row ratio={1}></Row>
			<Row ratio={0}></Row>
			<Col ratio={1}></Col>

			<ResponsiveItem>
				<Location row={0} col={0} />
				{children}
			</ResponsiveItem>

			<ResponsiveItem>
				<Location row={1} col={0} />
				{(okText || cancelText) &&
					<Box direction="row"
						width="100%"
						align="end"
						crossAlign="center"
						height={42}>
						<Item ratio={0} baseSize={80}>
							<div>
								{cancelText && (
									<Button stylingMode="outlined"
										onClick={() => cancel({ ...params, open: false })}
										text={t(cancelText)}
									/>)}
							</div>
						</Item>
						<Item ratio={0} baseSize={80}>
							<div style={{ marginLeft: 4 }}>
								{okText && (
									<Button type="default"
										onClick={() => { ok({ ...params, open: false }) }}
										text={t(okText)}
									/>)}
							</div>
						</Item>
					</Box>}
			</ResponsiveItem>
		</ResponsiveBox>
	</Popup>
};

export default PopupComponent