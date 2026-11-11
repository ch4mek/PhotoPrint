import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { CalculatedColumn, RenderHeaderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '../../types';

import { EDIT_ICON_CLASS_NAME } from '@/components/Grid/constants';

import { EditIconWrapper, HeaderCellContentWrapper } from './styles';

export const headerWithEditButton = <T extends GridCellDefaultProps>(
	{ column }: RenderHeaderCellProps<T, unknown>,
	displayEditIcon: boolean,
	onEditClick?: (column: CalculatedColumn<T>) => void,
	isMarkAsPast?: boolean
) => {
	const { name } = column;

	const handleEditClick = () => {
		onEditClick?.(column);
	};

	return (
		<HeaderCellContentWrapper isMarkAsPast={!!isMarkAsPast}>
			<Typography variant="s-600">{name}</Typography>

			{displayEditIcon && (
				<EditIconWrapper className={EDIT_ICON_CLASS_NAME}>
					<ActionButton onClick={handleEditClick}>
						<EditIcon />
					</ActionButton>
				</EditIconWrapper>
			)}
		</HeaderCellContentWrapper>
	);
};
