import React, { useState, useEffect, useCallback } from "react";
import { ApiURL } from "../../../../store/axios";
import { supplant } from "../../../../store/utils";
import { Grid, Icon, Typography } from "@material-ui/core";
import Modal from "../../../../components/UI/Modal";
import Popup from "../../../../components/UI/Popup";
import FileUploader from "../../../../components/UI/FileUploader";
import { useTranslation } from "../../../../store/i18next";
import notify from 'devextreme/ui/notify'
import TileView from 'devextreme-react/tile-view';
import ContextMenu from 'devextreme-react/context-menu';
import Gallery from "../../../../components/UI/Gallery";

const TileViewComponent = ({ row, model }) => {
	const { t } = useTranslation()
	//const { token } = useSelector(state => state.auth)
	const [modalOpen, setModalOpen] = useState(false);
	const [galleryModalOpen, setGalleryModalOpen] = useState(false);
	const [error, setError] = useState(null);
	const [data, setData] = useState([]);
	const [selectedItem, setSelectedItem] = useState(1);
	const [uploadUrl, setUploadUrl] = useState(null);

	const handleModalConfirm = (e) => {
		setModalOpen(false);
		refresh()
	};

	const handleModalCancel = (params) => {
		setModalOpen(false);
	};

	const refresh = useCallback(() => {

		setUploadUrl(supplant(`${ApiURL.origin}${model.uploadUrl}`, row))

		let filter = []
		let parentRelationName = model.relationFieldNames[0]
		let modelRelationName = model.relationFieldNames[1];

		filter.push(modelRelationName)
		filter.push("=")
		filter.push(row[parentRelationName])
		model.screen.dataSource.load(`name=${model.name}&filter=${JSON.stringify(filter)}`)
			.then(status => {
				if (status.Success) {
					let items = status.Resource.Items.map(i => {
						let sizeRatio = i.Width / i.Height
						if (sizeRatio > 0.65 && sizeRatio < 1.35) {
							//no ratio (sequare)
						}
						else if (i.Width > i.Height)
							i.widthRatio = 2
						else
							i.heightRatio = 2
						i.size = i.Size > 1024
							? `${Math.round(((i.Size / 1024) + Number.EPSILON) * 100) / 100} MB`
							: `${i.Size} KB`
						i.Url = `${ApiURL.origin}${i.Url}`
						i.ThumbnailUrl = `${ApiURL.origin}${i.ThumbnailUrl}`
						return i
					})
					items.unshift({ type: "button", onClick: () => setModalOpen(true) })
					setData(items)
				}
				else
					setError(status.Message)
			})
	}, [model, row]);

	const deleteFile = useCallback((id) => {
		model.screen.dataSource.delete(id)
			.then(status => {
				if (status.Success) {
					notify(`Deleted`, 'success');
					refresh()
				}
				else
					notify(status.Message, 'error');
			})
	}, [model, refresh])

	useEffect(() => {
		refresh()
	}, [refresh]);

	useEffect(() => {
		if (error) {
			notify(error, "error")
			setError(null)
		}
	}, [error]);

	return <>
		<ContextMenu
			dataSource={[
				{ name: "Delete", text: t('Delete'), icon: 'trash' },
				{ name: "View", text: t('View in full screen'), icon: 'fullscreen' }
			]}
			width={200}
			target="#tileitem"
			onItemClick={(e) => {
				e.component.hide()
				var item = data && data[selectedItem]
				if (e.itemData.name == "Delete") {
					if (item)
						deleteFile(data[selectedItem].Id)
				}
				else if (e.itemData.name == "View") {
					if (item) {
						setSelectedItem(data.indexOf(item))
						setGalleryModalOpen(true)
					}
				}

			}}
			onShowing={(e) => {
				e.cancel = (e.jQEvent && e.jQEvent.target) && e.jQEvent.target.id != "item"
			}}
		/>

		<TileView
			id="tileitem"
			items={data}
			onItemClick={(e) => {
				let item = e.itemData
				if (item.type != "button") {
					setSelectedItem(data.indexOf(item))
					setGalleryModalOpen(true)
				}
			}}
			onItemContextMenu={(e) => {
				let item = e.itemData
				if (item.type != "button")
					setSelectedItem(data.indexOf(item))
			}}
			itemRender={(item) => {
				let defaultStyle = {
					width: "100%",
					height: "100%",
					backgroundPosition: "center",
					backgroundSize: "cover",
					display: "block"
				}
				if (item.type == "button") {
					return <div style={defaultStyle}>
						<Grid container justify="center" alignItems="center" direction="column"
							style={{
								cursor: "pointer",
								height: "100%"
							}}
							onClick={item.onClick}
						>
							<Icon style={{ fontSize: 48, opacity: "0.7" }}>upload</Icon>
							<Typography style={{ paddingTop: 12 }}>{t("Upload")}</Typography>
						</Grid>
					</div>
				}
				else
					return <div
						id="item"
						style={{
							...defaultStyle,
							color: "white",
							backgroundImage: `url(${item.ThumbnailUrl})`,
							display: 'flex',
							alignItems: 'flex-end',
							padding: 4,
							zIndex: 0
						}}
					>
					</div>
			}}
			height={400}
			baseItemHeight={120}
			baseItemWidth={120}
			width='100%'
			itemMargin={10}
		>
		</TileView>

		<Modal
			open={modalOpen}
			onClose={handleModalCancel}
			title={"Images"}
			ok={handleModalConfirm}
			okText="Close"
		>
			<FileUploader
				uploadUrl={uploadUrl}
			/>
		</Modal>

		<Popup
			visible={galleryModalOpen}
			onClose={() => setGalleryModalOpen(false)}
			title={"Images"}
			fullScreen={true}
			showCloseButton={true}
			toolbarItems={[
				{
					widget: "dxButton",
					location: "after",
					options: {
						icon: "trash"
					},
					onClick: () => deleteFile(data[selectedItem].Id)
				}
			]}
		>
			<Gallery
				dataSource={data}
				defaultSelectedIndex={selectedItem - 1}
				onSelectionChanged={(i) => setSelectedItem(data.indexOf(i))}
			/>
		</Popup>

	</>
}

export default TileViewComponent