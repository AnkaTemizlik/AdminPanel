import React, { useEffect, useState } from 'react';
import TagBox from 'devextreme-react/tag-box';

const TagBoxComponent = ({ data, column }) => {

    const [value, setValue] = useState(null)

    console.info("TagBoxComponent", column.dataSource, column.editWith)

    const onValueChanged = (e) => {
        setValue(e.value);
        data.setValue(e.value.join(','))
    }

    const onSelectionChanged = () => {
        data.component.updateDimensions();
    }

    useEffect(() => {
        let val = data && data.value ? data.value.split(',') : [];
        //console.success("TagBoxComponent useEffect", data.value, val)
        setValue(val)
    }, [data.value])

    // useEffect(() => {
    //     console.success("TagBoxComponent value", value)
    // }, [value])

    return <TagBox
        dataSource={column.dataSource}
        //defaultValue={value}
        value={value}
        valueExpr={column.editWith.valueExpr}
        displayExpr={column.editWith.displayExpr}
        //valueExpr="value"
        //displayExpr="value"
        showSelectionControls={true}
        maxDisplayedTags={3}
        showMultiTagOnly={false}
        applyValueMode="useButtons"
        searchEnabled={false}
        onValueChanged={onValueChanged}
        onSelectionChanged={onSelectionChanged} />

}

export default TagBoxComponent