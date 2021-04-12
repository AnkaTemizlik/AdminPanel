import React from 'react';

import {
    TableEditColumn,
} from '@devexpress/dx-react-grid-material-ui';

export default ({ errors, ...props }) => {
    const { children } = props;
    return (
        <TableEditColumn.Cell {...props}>
            {React.Children.map(children, child => (
                child && child.props.id === 'commit'
                    ? React.cloneElement(child, { disabled: errors[props.tableRow.rowId] })
                    : child
            ))}
        </TableEditColumn.Cell>
    );
};