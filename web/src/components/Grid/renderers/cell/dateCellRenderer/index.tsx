import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '../../types';

import { DateTag } from '@/components/DateTag';
import { TagTypes } from '@/components/DateTag/types';
import { InferSpecifiedKeysByValue } from '@/types/helpers';

export function dateCellRenderer<T extends GridCellDefaultProps>(
	{ row }: RenderCellProps<T>,
	dateKey: InferSpecifiedKeysByValue<T, string>,
	typeKey: InferSpecifiedKeysByValue<T, TagTypes>,
	dateTagWidth?: number
) {
	const date = row[dateKey] as string;
	const type = row[typeKey] as TagTypes;
	const showSkeleton = row.showSkeleton;

	return showSkeleton ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<DateTag date={date} type={type} width={dateTagWidth} />
	);
}
