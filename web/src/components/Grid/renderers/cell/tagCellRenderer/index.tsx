import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '../../types';

import { TagStyles } from '@/components/Grid/renderers/cell/tagCellRenderer/types';
import { Tag } from '@/components/Tag';
import { DEFAULT_TAG_HEIGHT } from '@/constants/commonStyles';

import { CellTagWrapper } from './styles';

export const tagCellRenderer = <T extends GridCellDefaultProps>(
	{ row, column: { key } }: RenderCellProps<T, unknown>,
	getTagStyles?: (row: T) => TagStyles | undefined,
	tagHeight = DEFAULT_TAG_HEIGHT,
	tagWidth?: number
) => {
	const info = row[key as keyof T] as string;
	const isLoading = row?.showSkeleton;
	const tagStyles = getTagStyles?.(row);

	if (isLoading) {
		return <Skeleton animation="wave" width="100%" />;
	}

	return info ? (
		<CellTagWrapper disabled={row.disabled}>
			<Tag height={tagHeight} width={tagWidth} {...tagStyles}>
				{info}
			</Tag>
		</CellTagWrapper>
	) : null;
};
