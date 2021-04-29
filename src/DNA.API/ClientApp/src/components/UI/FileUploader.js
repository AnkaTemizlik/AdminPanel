import React, { useState, useEffect, useCallback } from "react";
import { useSelector } from 'react-redux';
import withWidth from '@material-ui/core/withWidth';
import notify from 'devextreme/ui/notify'
import FileUploader from 'devextreme-react/file-uploader';

/// https://github.com/DevExpress-Examples/DataGrid---How-to-use-FileUploader-in-an-edit-form/blob/20.2.5%2B/React/src/App.js

const FileUploadComponent = ({ onUploadCompleted, width, uploadUrl }) => {
	const { token } = useSelector(state => state.auth)
	const [modalOpen, setModalOpen] = useState(false);
	const [error, setError] = useState(null);
	const [data, setData] = useState([]);
	const [parentId, setParentId] = useState(0);
	const [defaultValue, setDefaultValue] = useState(null)
	const [selectedFiles, setSelectedFiles] = useState([])
	const [retryButtonVisible, setRetryButtonVisible] = useState(false);
	const [previews, setPreviews] = useState([]);
	// let fileUploaderRef = React.createRef();

	const onUploaded = useCallback((e) => {
		console.info("TileView onUploaded", e)
		//cellInfo.setValue("images/employees/" + e.request.responseText);
		setRetryButtonVisible(false);
		onUploadCompleted && onUploadCompleted(e)
	}, [onUploadCompleted]);

	const onUploadError = useCallback(e => {
		console.info("TileView onUploadError", e)
		let xhttp = e.request;
		if (xhttp.status === 400) {
			e.message = e.error.responseText;
		}
		if (xhttp.readyState === 4 && xhttp.status === 0) {
			e.message = "Connection refused";
		}
		setRetryButtonVisible(true);
	}, []);

	// const onClick = e => {
	// 	// The retry UI/API is not implemented. Use a private API as shown at T611719.
	// 	const fileUploaderInstance = fileUploaderRef.current.instance;
	// 	for (let i = 0; i < fileUploaderInstance._files.length; i++) {
	// 		delete fileUploaderInstance._files[i].uploadStarted;
	// 	}
	// 	fileUploaderInstance.upload();
	// };

	useEffect(() => {
		if (error) {
			notify(error, "error")
			setError(null)
		}
	}, [error]);

	return <>
		<FileUploader multiple={true}
			accept={"image/*" + (width == "xs" || width == "sm" ? ";capture=camera" : "")}
			uploadMode="useButtons"
			//ref={fileUploaderRef}
			allowedFileExtensions={['.jpg', '.jpeg', '.gif', '.png']}
			uploadUrl={uploadUrl}
			uploadHeaders={{
				Authorization: 'Bearer ' + token
			}}
			uploadCustomData={{
				SelectedFiles: selectedFiles
			}}
			onUploaded={onUploaded}
			onUploadError={onUploadError}
			onValueChanged={(e) => {
				setSelectedFiles(e.value)
			}}
		/>
		{/* <div className="content" style={{ display: selectedFiles.length > 0 ? 'block' : 'none' }}>
			{selectedFiles.map((file, i) => {
				return <div className="selected-item" key={i}>
					<h4>{`Name: ${file.name}`}<br /></h4>
					<span>{`Size ${file.size}`}<br /></span>
					<span>{`Type ${file.size}`}<br /></span>
					<span>{`Last Modified Date: ${new Date(file.lastModifiedDate).toISOString()}`}</span>
				</div>
			})}
		</div> */}
	</>
}

export default withWidth()(FileUploadComponent);