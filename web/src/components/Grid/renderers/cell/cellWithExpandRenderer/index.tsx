import { ChevronIcon } from '@mms/mms-ui-library/icons/base';
import { RenderCellProps } from 'react-data-grid';

import { ChevronButton } from '@/components/ChevronButton';
import { CellWrapper, GridCellDefaultProps } from '@/components/Grid';

import { CellWithExpandRendererOptions, GridRowProps } from './types';

export const CellWithExpandRenderer = <
	T extends GridCellDefaultProps & GridRowProps
>(
	{ row, onRowChange, column: { key } }: RenderCellProps<T, unknown>,
	{ fieldToDisplay, theme }: CellWithExpandRendererOptions<T>
) => {
	const info = row[fieldToDisplay || (key as keyof T)] as string;

	const { hasStages, expanded, isMainInterview } = row;

	const handleExpand = () => {
		if (onRowChange) {
			onRowChange({ ...row, expanded: !row.expanded });
		}
	};

	return (
		<CellWrapper
			style={{
				display: 'flex',
				justifyContent: 'space-between',
				width: '100%',
			}}
		>
			{row.showCell ? '' : info}
			{isMainInterview && hasStages && (
				<ChevronButton
					onClick={handleExpand}
					chevronColor={theme?.palette['graphics-primary-2']}
					isActive={expanded}
					size="small"
				>
					<ChevronIcon />
				</ChevronButton>
			)}
		</CellWrapper>
	);
};
