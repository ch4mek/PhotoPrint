import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '../../types';

interface EditCellRendererOptions {
	isDisabled?: boolean;
	skeletonHeight?: string;
}

export const editCellRenderer = <T extends GridCellDefaultProps>(
	{ row: { id, handleEditClick, showSkeleton } }: RenderCellProps<T, unknown>,
	options?: EditCellRendererOptions
) => {
	const { isDisabled = false, skeletonHeight = 'auto' } = options || {};

	if (showSkeleton) {
		return <Skeleton animation="wave" width="100%" height={skeletonHeight} />;
	}

	const clickHandler = handleEditClick?.(id);

	if (!clickHandler || isDisabled) {
		return null;
	}

	return (
		<ActionButton disabled={isDisabled} onClick={clickHandler} size="small">
			<EditIcon />
		</ActionButton>
	);
};
