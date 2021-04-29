import React from 'react';
import Gallery from 'devextreme-react/gallery';

export default ({ dataSource, defaultSelectedIndex, onSelectionChanged }) => {
	return <Gallery
		id="gallery"
		dataSource={dataSource.filter(s => s.type != "button")}
		defaultSelectedIndex={defaultSelectedIndex || 0}
		onSelectionChanged={(e) => onSelectionChanged(e.addedItems[0])}
		loop={false}
		showNavButtons={true}
		showIndicator={true}
		itemRender={(d) => <Item {...d} />}
		height={"100%"}
		width={"100%"}
	/>
}

function Item({ Name, Url, size, FileType, UpdateTime, CreationTime, Id }) {
	return (
		<div style={{
			width: "100%",
			height: "100%",
			backgroundPosition: "center",
			backgroundSize: "contain",
			backgroundRepeat: "no-repeat",
			backgroundImage: `url(${Url})`,
			display: 'flex',
		}}>
			<div className="dx-datagrid-columns-separator"
				style={{
					marginTop: 'auto',
					marginBottom: 0,
					bottom: 0,
					color: "white",
					padding: '4px',
					width: '100%',
					height: 48,
					textAlign: 'center',
					fontSize: 11
				}}>{`${Name}, ${size}, ${FileType}`}</div>
		</div>
	);
}